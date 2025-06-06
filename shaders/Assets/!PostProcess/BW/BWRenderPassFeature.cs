using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BWRenderPassFeature : ScriptableRendererFeature
{
    private BWPass bwPass;

    //when feature created
    public override void Create()
    {
        //create an instance of bwpass for rendering
        bwPass = new BWPass();
    }
    //add render passes to renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //enqueue custom render pass
        renderer.EnqueuePass(bwPass);
    }
}

//custom render pass class
class BWPass : ScriptableRenderPass
{
    Material _mat; //bw effect mat
    int bwId = Shader.PropertyToID("_Temp"); //id for temp render target
    RenderTargetIdentifier src, bw; //render target identifiers

    public BWPass()
    {
        if (!_mat)
            _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/B&W Post-Processing");
        //set render pass event to execute before post-processing
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    //called when setting up cameara for rendering
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //get camera target descriptor
        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
        //set source render target identifier
        src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        //create temp render target and get identifier
        cmd.GetTemporaryRT(bwId, desc, FilterMode.Bilinear);
        bw = new RenderTargetIdentifier(bwId);
    }
    //execute custom render pass
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer commandBuffer = CommandBufferPool.Get("BWRenderPassFeature");
        VolumeStack volumes = VolumeManager.instance.stack;
        BlackAndWhitePostProcess bwPP = volumes.GetComponent<BlackAndWhitePostProcess>();

        if (bwPP.IsActive())
        {
            _mat.SetFloat("_blend", (float)bwPP.blendIntensity);
            //apply bw effect to temp render target
            Blit(commandBuffer, src, bw, _mat, 0);
            //blit result back to source render target
            Blit(commandBuffer, bw, src);
        }

        context.ExecuteCommandBuffer(commandBuffer);
        CommandBufferPool.Release(commandBuffer);
    }
    //cleaning up camera after rendering
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(bwId);
    }
}
