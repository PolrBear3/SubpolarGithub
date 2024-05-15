using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolume_Controller : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    private DepthOfField _depthField;
    private ColorAdjustments _colorAdjustments;

    [Header("Transition")]
    [SerializeField] private LeanTweenType _transitionType;

    [Header("Depth Of Field")]
    [SerializeField] private float _distance;
    [SerializeField] private float _defaultDistance;

    [Header("Color Adjustments")]
    [SerializeField] private float _exposure;
    [SerializeField] private float _defaultExposure;


    // MonoBehaviour
    private void Awake()
    {
        _volume.profile.TryGet(out _depthField);
        _volume.profile.TryGet(out _colorAdjustments);
    }


    // Screen Blur
    public void Blur_GameScreen_Toggle(bool toggleOn, float transitionTime)
    {
        if (toggleOn)
        {
            LeanTween.value(gameObject, _depthField.focusDistance.value, _distance, transitionTime).setOnUpdate(UpdateFocusDistance).setEase(_transitionType);
            return;
        }

        LeanTween.value(gameObject, _depthField.focusDistance.value, _defaultDistance, transitionTime).setOnUpdate(UpdateFocusDistance).setEase(_transitionType);
    }
    //
    private void UpdateFocusDistance(float newValue)
    {
        _depthField.focusDistance.value = newValue;
    }


    // Darken Screen
    public void Darken_GameScreen_Toggle(bool toggleOn, float transitionTime)
    {
        if (toggleOn)
        {
            LeanTween.value(gameObject, _colorAdjustments.postExposure.value, _exposure, transitionTime).setOnUpdate(UpdatepostExposure).setEase(_transitionType);
            return;
        }

        LeanTween.value(gameObject, _colorAdjustments.postExposure.value, _defaultExposure, transitionTime).setOnUpdate(UpdatepostExposure).setEase(_transitionType);
    }
    //
    private void UpdatepostExposure(float newValue)
    {
        _colorAdjustments.postExposure.value = newValue;
    }
}