using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/Colour Effect", typeof(UniversalRenderPipeline))]
public class ColourEffectSettings : VolumeComponent, IPostProcessComponent
{
    public enum COLOUR_EFFECT
    {
        EFFECT_MULTIPLY = 0,
        EFFECT_COLOUR_BURN = 1,
        EFFECT_LINEAR_BURN =2,
        EFFECT_SCREEN =3,
        EFFECT_COLOUR_DODGE = 4,
        EFFECT_LINEAR_DODGE = 6
    };
    [Serializable]
    public sealed class ColourEffectParameter : VolumeParameter<COLOUR_EFFECT>
    {
        public ColourEffectParameter(COLOUR_EFFECT effect, bool overrideState = false) : base(effect, overrideState) { }
    }

    public ColourEffectParameter effect = new(COLOUR_EFFECT.EFFECT_MULTIPLY);

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
