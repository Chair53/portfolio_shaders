using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlineRendererFeature : ScriptableRendererFeature
{
    private OutlineRenderPass _pass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
    public override void Create()
    {
        _pass = new();
    }
}

public class OutlineRenderPass : ScriptableRenderPass
{
    private Material _mat;
    private int thicknessID = Shader.PropertyToID("_Thickness");
    private int colourID = Shader.PropertyToID("_Colour");

    public OutlineRenderPass()
    {
        profilingSampler = new(nameof(OutlineRenderPass));
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (!_mat)
            _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/Outline");
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        base.OnCameraCleanup(cmd);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType is CameraType.Preview or CameraType.Reflection) return;

        CommandBuffer cmd = CommandBufferPool.Get("OutlineRenderFeature");
        VolumeStack volumes = VolumeManager.instance.stack;
        OutlineSettings outlineSettings = volumes.GetComponent<OutlineSettings>();
        if (outlineSettings.IsActive())
        {
            _mat.SetColor("_Colour", outlineSettings.colour.value);
            _mat.SetFloat("_Thickness", outlineSettings.thickness.value);
            _mat.SetVector("_Sensitivities", outlineSettings.sensitivities.value);

            Blitter.BlitTexture(cmd, Vector2.one, _mat, 0);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}