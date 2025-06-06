using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/B&W Post-Processing", typeof(UniversalRenderPipeline))]
public class BlackAndWhitePostProcess : VolumeComponent, IPostProcessComponent
{
    public FloatParameter blendIntensity = new FloatParameter(1.0f);
    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}
