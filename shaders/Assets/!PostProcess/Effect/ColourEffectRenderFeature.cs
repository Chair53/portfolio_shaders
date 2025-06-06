using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class ColourEffectRendererFeature : ScriptableRendererFeature
{
    private ColourEffectRenderPass _pass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
    public override void Create()
    {
        _pass = new();
    }
}

public class ColourEffectRenderPass : ScriptableRenderPass
{
    private Material _mat;
    private RTHandle src, dest;
    public ColourEffectRenderPass()
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        Shader shader = Shader.Find("Custom Post-Processing/Colour Effect");
        _mat = new Material(shader);
        src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        VolumeStack volumes = VolumeManager.instance.stack;
        ColourEffectSettings settings = volumes.GetComponent<ColourEffectSettings>();
        if (!settings.IsActive())
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        cmd.Clear();
        dest = src;
        _mat.SetColor(Shader.PropertyToID("_OverlayColour"), settings.colour.value);
        _mat.SetFloat(Shader.PropertyToID("_Intensity"), settings.intensity.value);
        _mat.SetInteger(Shader.PropertyToID("_Effect"), (int)settings.effect.value);

        cmd.Blit(dest, src, _mat, 0);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}