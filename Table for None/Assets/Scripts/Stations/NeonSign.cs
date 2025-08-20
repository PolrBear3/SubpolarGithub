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

    [Space(10)] 
    [SerializeField] [Range(0, 50)] private int _populationIncreaseValue;

    
    private MaterialPropertyBlock _materialBlock;
    
    private bool _isToggled;
    private int _currentDefaultIndex;


    // MonoBehaviour
    private void Awake()
    {
        _materialBlock = new();
    }

    private void Start()
    {
        _controller.Food_Icon().Hide_Icon();
        
        Load_SignSprite();
        Update_Effects();
        
        // subscriptions
        IInteractable_Controller interactable = _controller.iInteractable;
        
        interactable.OnHoldInteract += Update_SignSprite;
        interactable.OnHoldInteract += Update_Effects;
        
        interactable.OnInteract += Toggle;
    }

    private void OnDestroy()
    {
        // subscriptions
        IInteractable_Controller interactable = _controller.iInteractable;
        
        interactable.OnHoldInteract -= Update_SignSprite;
        interactable.OnHoldInteract -= Update_Effects;
        
        interactable.OnInteract -= Toggle;
    }


    // Gets
    private Color Current_EffectColor()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();
        
        if (foodIcon.hasFood == false) return _defaultDatas[_currentDefaultIndex].color;
        return foodIcon.currentData.foodScrObj.spriteColor;
    }
    
    
    // Main
    private void Toggle()
    {
        _isToggled = !_isToggled;
        Update_Effects(_isToggled);

        int updateValue = _isToggled ? _populationIncreaseValue : -_populationIncreaseValue;
        Main_Controller.instance.currentLocation.Update_AdditionalPopulation(updateValue);
    }
    
    
    // Updates
    private void Load_SignSprite()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();
        
        if (foodIcon.hasFood)
        {
            _signSR.sprite = foodIcon.currentData.foodScrObj.sprite;
            return;
        }
        _signSR.sprite = _defaultDatas[_currentDefaultIndex].sprite;
    }
    private void Update_SignSprite()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();
        FoodData_Controller playerFoodIcon = Main_Controller.instance.Player().foodIcon;

        foodIcon.Update_AllDatas(null);
        
        if (playerFoodIcon.hasFood)
        {
            Food_ScrObj playerFood = playerFoodIcon.currentData.foodScrObj;
            _signSR.sprite = playerFood.sprite;

            foodIcon.Set_CurrentData(new(playerFood));
            return;
        }

        _currentDefaultIndex = (_currentDefaultIndex + 1) % _defaultDatas.Length;
        _signSR.sprite = _defaultDatas[_currentDefaultIndex].sprite;
    }
    
    private void Update_Effects(bool toggle)
    {
        _signSR.GetPropertyBlock(_materialBlock);

        _materialBlock.SetFloat("_Glow", toggle ? _glowValue : 0f);
        _light.intensity = toggle ? _lightValue : 0f;

        if (toggle)
        {
            Color currentColor = Current_EffectColor();
            
            _materialBlock.SetColor("_GlowColor", currentColor);
            _light.color = currentColor;
        }

        _signSR.SetPropertyBlock(_materialBlock);
    }
    private void Update_Effects()
    {
        Update_Effects(_isToggled);
    }
}
