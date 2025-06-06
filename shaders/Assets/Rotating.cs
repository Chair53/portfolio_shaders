using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Rotating : MonoBehaviour
{
    [SerializeField] float spd = 5;
    [SerializeField] float zoom = 5;
    [SerializeField] float rot = 20;
    [SerializeField] Canvas ui;
    [SerializeField] Volume settings;
    private Camera cam;

    //
    BlackAndWhitePostProcess blackwhiteSettings;
    [SerializeField] Slider bwIntensitySlider;
    private void SetBW()
    {
        blackwhiteSettings.blendIntensity.value = bwIntensitySlider.value;
    }

    //
    GaussianBlurPostProcess gaussianBlurSettings;
    [SerializeField] Slider gbIntensitySlider;
    private void SetGB()
    {
        gaussianBlurSettings.blurIntensity.value = gbIntensitySlider.value;
    }

    //
    TintSettings tintSettings;
    [SerializeField] Slider tintIntensitySlider;
    [SerializeField] Slider tintRSlider;
    [SerializeField] Slider tintGSlider;
    [SerializeField] Slider tintBSlider;
    [SerializeField] Slider tintASlider;
    private void SetTint()
    {
        tintSettings.intensity.value = tintIntensitySlider.value;
        Color c = new(tintRSlider.value, tintGSlider.value, tintBSlider.value, tintASlider.value);
        tintSettings.colour.value = c;
    }

    //
    OutlineSettings outlineSettings;
    [SerializeField] Slider outlineThicknessSlider;
    [SerializeField] Slider outlineRSlider;
    [SerializeField] Slider outlineGSlider;
    [SerializeField] Slider outlineBSlider;
    [SerializeField] Slider outlineASlider;
    private void SetOutline()
    {
        outlineSettings.thickness.value = (int)outlineThicknessSlider.value;
        Color c = new(outlineRSlider.value, outlineGSlider.value, outlineBSlider.value, outlineASlider.value);
        outlineSettings.colour.value = c;
    }

    //
    ColourEffectSettings colourEffectSettings;
    [SerializeField] Slider colourEffectIntensitySlider;
    [SerializeField] Slider colourEffectRSlider;
    [SerializeField] Slider colourEffectGSlider;
    [SerializeField] Slider colourEffectBSlider;
    [SerializeField] Slider colourEffectASlider;
    [SerializeField] TMP_Dropdown colourEffectDropdown;
    private void SetColourEffect()
    {
        colourEffectSettings.intensity.value = colourEffectIntensitySlider.value;
        Color c = new(colourEffectRSlider.value, colourEffectGSlider.value, colourEffectBSlider.value, colourEffectASlider.value);
        colourEffectSettings.colour.value = c;
        colourEffectSettings.effect.value = (ColourEffectSettings.COLOUR_EFFECT)colourEffectDropdown.value;
    }

    //
    PixelSettings pixelSettings;
    [SerializeField] Slider pixelIntensitySlider;
    private void SetPixel()
    {
        pixelSettings.screenHeight.value = (int)pixelIntensitySlider.value;
    }

    //
    [SerializeField] Material waterMat;
    [SerializeField] Slider waterWaveSlider;
    [SerializeField] Slider waterNormalSlider;
    [SerializeField] Slider waterRefractionSlider;
    private void SetWater()
    {
        waterMat.SetFloat("_WaveSpeed", waterWaveSlider.value);
        waterMat.SetFloat("_NormalStrength", waterNormalSlider.value);
        waterMat.SetFloat("_RefractionSpeed", waterRefractionSlider.value);
    }

    //
    [SerializeField] Material grassMat;
    [SerializeField] Slider grassSpeedSlider;
    [SerializeField] Slider grassStrengthSlider;
    private void SetGrass()
    {
        grassMat.SetFloat("_WindSpeed", grassSpeedSlider.value);
        grassMat.SetFloat("_Bending", grassStrengthSlider.value);
    }

    //
    [SerializeField] Material starsMat;
    [SerializeField] Slider starsInnerRSlider;
    [SerializeField] Slider starsInnerGSlider;
    [SerializeField] Slider starsInnerBSlider;
    [SerializeField] Slider starsInnerASlider;
    [SerializeField] Slider starsOuterRSlider;
    [SerializeField] Slider starsOuterGSlider;
    [SerializeField] Slider starsOuterBSlider;
    private void SetStars()
    {
        starsMat.SetColor("_InnerColour", new Color(starsInnerRSlider.value, starsInnerGSlider.value, starsInnerBSlider.value, starsInnerASlider.value));
        starsMat.SetVector("_OuterColour", new Vector4(starsOuterRSlider.value, starsOuterGSlider.value, starsOuterBSlider.value, 1.0f));
    }

    private void Start()
    {
        cam = Camera.main;
        settings.profile.TryGet(out blackwhiteSettings);
        settings.profile.TryGet(out gaussianBlurSettings);
        settings.profile.TryGet(out tintSettings);
        settings.profile.TryGet(out outlineSettings);
        settings.profile.TryGet(out colourEffectSettings);
        settings.profile.TryGet(out pixelSettings);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * spd * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * spd * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * spd * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * spd * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
            transform.position += Vector3.up * spd * Time.deltaTime;
        else if (Input.GetKey(KeyCode.LeftShift))
            transform.position -= Vector3.up * spd * Time.deltaTime;

        Vector3 rotation = transform.rotation.eulerAngles;
        if (Input.GetKey(KeyCode.Q))
            rotation.y += Time.deltaTime * rot;
        else if (Input.GetKey(KeyCode.E))
            rotation.y -= Time.deltaTime * rot;
        if (Input.GetKey(KeyCode.Z))
            rotation.x += Time.deltaTime * rot;
        else if (Input.GetKey(KeyCode.X))
            rotation.x -= Time.deltaTime * rot;
        transform.rotation = Quaternion.Euler(rotation);

        if (Input.GetKey(KeyCode.KeypadPlus))
            cam.fieldOfView -= zoom * Time.deltaTime;
        else if (Input.GetKey(KeyCode.KeypadMinus))
            cam.fieldOfView += zoom * Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Escape))
            ui.gameObject.SetActive(!ui.gameObject.activeSelf);

        SetPostProcess();
        SetGraph();
    }

    private void SetPostProcess()
    {
        SetBW();
        SetGB();
        SetTint();
        SetOutline();
        SetColourEffect();
        SetPixel();
    }

    private void SetGraph()
    {
        SetWater();
        SetGrass();
        SetStars();
    }

}
