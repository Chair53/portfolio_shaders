using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class TintRendererFeature : ScriptableRendererFeature
{
    private TintRenderPass _pass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
    public override void Create()
    {
        _pass = new();
    }
}

public class TintRenderPass : ScriptableRenderPass
{
    private Material _mat;
    int tintID = Shader.PropertyToID("_Temp");
    RenderTargetIdentifier src, tint;
    public TintRenderPass()
    {
        if (!_mat)
            _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/Tint");
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
        src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        tint = new RenderTargetIdentifier(tintID);
        cmd.GetTemporaryRT(tintID, desc, FilterMode.Bilinear);
        base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tintID);
        base.OnCameraCleanup(cmd);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("TintRenderFeature");
        VolumeStack volumes = VolumeManager.instance.stack;
        TintSettings tintSettings = volumes.GetComponent<TintSettings>();
        if(tintSettings.IsActive())
        {
            _mat.SetColor("_OverlayColour", tintSettings.colour.value);
            _mat.SetFloat("_Intensity", tintSettings.intensity.value);

            Blit(cmd, src, tint, _mat, 0);
            Blit(cmd, tint, src, _mat, 0);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}