using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlurRenderFeature : ScriptableRendererFeature
{
    private GaussianBlurPass gaussianBlurPass;

    public override void Create()
    {
        gaussianBlurPass = new();
        name = "Gaussian Blur";
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(gaussianBlurPass);
    }
}

class GaussianBlurPass : ScriptableRenderPass
{
    private Material material;
    private GaussianBlurPostProcess blurPostProcess;
    private RenderTargetIdentifier src;
    private RenderTargetHandle dest;
    private int texID;

    public GaussianBlurPass()
    {
        if (!material)
            material = CoreUtils.CreateEngineMaterial("Custom Post-Processing/Gaussian Blur");
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        blurPostProcess = VolumeManager.instance.stack.GetComponent<GaussianBlurPostProcess>();
        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
        src = renderingData.cameraData.renderer.cameraColorTargetHandle;
    }
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if (blurPostProcess == null || !blurPostProcess.IsActive()) return;

        texID = Shader.PropertyToID("_MainTex");
        dest = new RenderTargetHandle();
        dest.id = texID;
        cmd.GetTemporaryRT(texID, cameraTextureDescriptor);
        base.Configure(cmd, cameraTextureDescriptor);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (blurPostProcess == null || !blurPostProcess.IsActive()) return;

        CommandBuffer cmd = CommandBufferPool.Get("Custom/Gaussian Blur");
        int gridSize = Mathf.CeilToInt(blurPostProcess.blurIntensity.value * 6.0f);
        if (gridSize % 2 == 0)
            gridSize += 1; //want odd

        material.SetInteger("_GridSize", gridSize);
        material.SetFloat("_Spread", blurPostProcess.blurIntensity.value);
        cmd.Blit(src, texID, material, 0);
        cmd.Blit(texID, src, material, 1);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
