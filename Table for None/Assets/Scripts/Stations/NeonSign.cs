using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class NeonSign_DefaultData
{
    [SerializeField] private Sprite _sprite;
    public Sprite sprite => _sprite;
    
    [SerializeField] private Color _color;
    public Color color => _color;
}

public class NeonSign : MonoBehaviour
{
    [SerializeField] private Station_Controller _controller;

    [Space(20)] 
    [SerializeField] private SpriteRenderer _signSR;
    [SerializeField] private Light2D _light;
    [SerializeField] private Material[] _materials;

    [Space(20)] 
    [SerializeField] private NeonSign_DefaultData[] _defaultDatas;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _glowValue;
    [SerializeField][Range(0, 10)] private float _lightValue;

    
    private MaterialPropertyBlock _materialBlock;
    private bool _isToggled;


    // MonoBehaviour
    private void Awake()
    {
        _materialBlock = new();
    }

    private void Start()
    {
        _controller.iInteractable.OnInteract += Toggle;
    }

    private void OnDestroy()
    {
        _controller.iInteractable.OnInteract -= Toggle;
    }


    // Main
    private void Toggle()
    {
        _isToggled = !_isToggled;
        
        Toggle_Effects(_isToggled);
    }
    
    private void Toggle_Effects(bool toggle)
    {
        _signSR.GetPropertyBlock(_materialBlock);

        _materialBlock.SetFloat("_Glow", toggle ? _glowValue : 0f);
        _light.intensity = toggle ? _lightValue : 0f;

        if (toggle) _materialBlock.SetColor("_GlowColor", Color.white);

        _signSR.SetPropertyBlock(_materialBlock);
    }
}
