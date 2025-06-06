using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class PixelRendererFeature : ScriptableRendererFeature
{
    private PixelRenderPass _pass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(_pass);
    }
    public override void Create()
    {
        _pass = new();
    }
}

public class PixelRenderPass : ScriptableRenderPass
{
    private Material _mat;
    private PixelSettings settings;
    private RenderTargetIdentifier colourBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");
    private int pixelScreenheight, pixelScreenwidth;

    public PixelRenderPass()
    {
        VolumeStack volumes = VolumeManager.instance.stack;
        settings = volumes.GetComponent<PixelSettings>();
        renderPassEvent = settings.renderPassEvent;
        if (!_mat)
            _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/Pixel");
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        colourBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
        pixelScreenheight = settings.screenHeight.value;
        pixelScreenwidth = (int)(pixelScreenheight * renderingData.cameraData.camera.aspect + 0.5f);
        _mat.SetVector("_BlockCount", new Vector2(pixelScreenwidth, pixelScreenheight));
        _mat.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenwidth, 1.0f / pixelScreenheight));
        _mat.SetVector("_HalfBlockSize", new Vector2(0.5f/pixelScreenwidth, 0.5f / pixelScreenheight));
        desc.height = pixelScreenheight;
        desc.width = pixelScreenwidth;
        cmd.GetTemporaryRT(pixelBufferID, desc, FilterMode.Point);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
        base.OnCameraSetup(cmd, ref renderingData);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!settings.IsActive())
            return;

        CommandBuffer cmd = CommandBufferPool.Get("PixelRenderFeature");
        using (new ProfilingScope(cmd, new ProfilingSampler("Pixel Pass")))
        {
            Blit(cmd, colourBuffer, pixelBuffer, _mat);
            Blit(cmd, pixelBuffer, colourBuffer);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd != null)
            cmd.ReleaseTemporaryRT(pixelBufferID);
        base.OnCameraCleanup(cmd);
    }
}