using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Outline", typeof(UniversalRenderPipeline))]
public class OutlineSettings : VolumeComponent, IPostProcessComponent
{
    public IntParameter thickness = new(1);
    public ColorParameter colour = new ColorParameter(Color.black);
    [Tooltip("Depth, Normal, Luminance")]
    public Vector3Parameter sensitivities = new Vector3Parameter(new Vector3(200.0f, 4.0f, 0.5f));

    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return true;
    }
}
