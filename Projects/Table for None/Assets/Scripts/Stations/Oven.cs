using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODUnity;
using System.Linq;

public class Oven : Table
{
    [Space(20)]
    [SerializeField] private Sprite _heatEmissionSprite;
    [SerializeField] private Light2D _light;
    [SerializeField] private Material[] _materials;

    [Space(20)]
    [SerializeField] private float _heatIncreaseTime;

    [SerializeField][Range(0, 10)] private float _emissionValue;
    [SerializeField][Range(0, 10)] private float _lightValue;

    [Space(20)]
    [SerializeField] private AmountBar _heatDelayBar;

    
    private Sprite _defaultSprite;
    
    private Coroutine _emissionCoroutine;
    private Coroutine _heatCoroutine;


    // UnityEngine
    private new void Awake()
    {
        base.Awake();
        
        _defaultSprite = stationController.spriteRenderer.sprite;
    }
    
    private new void Start()
    {
        Heat_Food();
        Update_CurrentVisual();
        
        // subscription
        stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;
        
        IInteractable_Controller interactable = stationController.iInteractable;

        interactable.OnInteract += SwapFood;
        interactable.OnAction1 += PlaceFood;
        interactable.OnAction2 += TakeFood;
        
        interactable.OnInteract += Heat_Food;
        interactable.OnInteract += Update_CurrentVisual;
        
        interactable.OnAction1 += Heat_Food;
        interactable.OnAction1 += Update_CurrentVisual;
        
        interactable.OnAction2 += Heat_Food;
        interactable.OnAction2 += Update_CurrentVisual;

        interactable.OnHoldInteract += SwapFood;
        interactable.OnHoldInteract += Heat_Food;
        interactable.OnHoldInteract += Update_CurrentVisual;
    }

    private new void OnDestroy()
    {
        // subscription
        stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;
        
        IInteractable_Controller interactable = stationController.iInteractable;

        interactable.OnInteract -= SwapFood;
        interactable.OnAction1 -= PlaceFood;
        interactable.OnAction2 -= TakeFood;
        
        interactable.OnInteract -= Heat_Food;
        interactable.OnInteract -= Update_CurrentVisual;
        
        interactable.OnAction1 -= Heat_Food;
        interactable.OnAction1 -= Update_CurrentVisual;
        
        interactable.OnAction2 -= Heat_Food;
        interactable.OnAction2 -= Update_CurrentVisual;
        
        interactable.OnHoldInteract -= SwapFood;
        interactable.OnHoldInteract -= Heat_Food;
        interactable.OnHoldInteract -= Update_CurrentVisual;
    }


    // Oven Visual Update
    private void Update_CurrentVisual()
    {
        if (_emissionCoroutine != null)
        {
            StopCoroutine(_emissionCoroutine);
        }

        _emissionCoroutine = StartCoroutine(Update_CurrentVisual_Coroutine());
    }
    private IEnumerator Update_CurrentVisual_Coroutine()
    {
        SpriteRenderer sr = stationController.spriteRenderer;
        
        // active
        if (HeatFood_Available())
        {
            _light.intensity = _lightValue;
            
            sr.sprite = _heatEmissionSprite;
            sr.material = _materials[1];

            LeanTween.value(gameObject, 0f, _emissionValue, 2f).setOnUpdate((float val) =>
            {
                sr.material.SetFloat("_Glow", val);
            });
        }
        // inactive
        else
        {
            if (sr.sprite == _defaultSprite) yield break;
            
            LeanTween.value(gameObject, sr.material.GetFloat("_Glow"), 0f, 2f).setOnUpdate((float val) =>
            {
                sr.material.SetFloat("_Glow", val);
            });

            yield return new WaitForSeconds(2f);

            _light.intensity = 0f;
            
            sr.sprite = _defaultSprite;
            sr.material = _materials[0];
        }
    }
    
    
    // Food Heating System
    private bool HeatFood_Available()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        if (foodIcon.hasFood == false) return false;

        FoodCondition_Type[] restrictedConditions = foodIcon.currentData.foodScrObj.restrictedCondtions;
        if (restrictedConditions.Contains(FoodCondition_Type.heated)) return false;

        return true;
    }

    private void Heat_Food()
    {
        if (_heatCoroutine != null)
        {
            StopCoroutine(_heatCoroutine);
            _heatCoroutine = null;
        }

        if (!HeatFood_Available())
        {
            _heatDelayBar.Toggle(false);
            return;
        }
        _heatDelayBar.Toggle_BarColor(true);

        // oven switch
        Audio_Controller.instance.Play_OneShot(gameObject, 3);

        _heatCoroutine = StartCoroutine(Heat_Food_Coroutine());
    }
    private IEnumerator Heat_Food_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (foodIcon.hasFood)
        {
            _heatDelayBar.Set_Amount(0);
            _heatDelayBar.Toggle(true);
            
            float delayTikTime = _heatIncreaseTime / _heatDelayBar.maxAmount;
                
            while (_heatDelayBar.Is_MaxAmount() == false)
            {
                _heatDelayBar.Update_Amount(1);
                _heatDelayBar.Load();
                
                yield return new WaitForSeconds(delayTikTime);
            }

            foodIcon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.heated));
            foodIcon.Show_Condition();

            // durability
            Station_Maintenance maintenance = stationController.maintenance;
            
            maintenance.Update_Durability(-1);
            maintenance.Update_DurabilityBreak();
        }

        _heatDelayBar.Toggle(false);
        
        _heatCoroutine = null;
        yield break;
    }
}