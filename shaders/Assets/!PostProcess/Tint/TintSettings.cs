using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Tint", typeof(UniversalRenderPipeline))]
public class TintSettings : VolumeComponent, IPostProcessComponent
{
    public FloatParameter intensity = new FloatParameter(1);
    public ColorParameter colour = new ColorParameter(Color.white);

    public bool IsActive()
    {
        return intensity.value > 0;
    }

    public bool IsTileCompatible()
    {
        return true;
    }
}
