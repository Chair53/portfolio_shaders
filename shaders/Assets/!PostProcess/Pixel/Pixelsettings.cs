using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Pixel", typeof(UniversalRenderPipeline))]
public class PixelSettings : VolumeComponent, IPostProcessComponent
{
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public IntParameter screenHeight = new(144);
    public bool IsActive()
    {
        return screenHeight.value > 0 && active;
    }

    public bool IsTileCompatible()
    {
        return true;
    }
}
