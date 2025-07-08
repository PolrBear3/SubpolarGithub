using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODUnity;
using System.Linq;

public class Oven : Table
{
    [Header("")]
    [SerializeField] private SpriteRenderer _heatEmission;
    [SerializeField] private Light2D _light;

    private Coroutine _emissionCoroutine;

    [Header("")]
    [SerializeField] private float _heatIncreaseTime;
    [SerializeField][Range(0, 10)] private float _emissionValue;
    [SerializeField][Range(0, 10)] private float _lightValue;

    [Header("")] 
    [SerializeField] private AmountBar _heatDelayBar;
    
    private Coroutine _heatCoroutine;
    

    // UnityEngine
    private new void Start()
    {
        Heat_Food();
        Update_CurrentVisual();
        
        // subscription
        stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;
        
        IInteractable_Controller interactable = stationController.iInteractable;

        interactable.OnInteract += Basic_SwapFood;
        interactable.OnInteract += Heat_Food;
        interactable.OnInteract += Update_CurrentVisual;
        
        interactable.OnHoldInteract += Transfer_CurrentFood;
        interactable.OnHoldInteract += Heat_Food;
        interactable.OnHoldInteract += Update_CurrentVisual;
    }

    private new void OnDestroy()
    {
        // subscription
        stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;
        
        IInteractable_Controller interactable = stationController.iInteractable;

        interactable.OnInteract -= Basic_SwapFood;
        interactable.OnInteract -= Heat_Food;
        interactable.OnInteract -= Update_CurrentVisual;
        
        interactable.OnHoldInteract -= Transfer_CurrentFood;
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
        // active
        if (HeatFood_Available())
        {
            _light.intensity = _lightValue;
            _heatEmission.color = Color.white;

            LeanTween.value(gameObject, 0f, _emissionValue, 2f).setOnUpdate((float val) =>
            {
                _heatEmission.material.SetFloat("_Glow", val);
            });
        }
        // inactive
        else
        {
            LeanTween.value(gameObject, _heatEmission.material.GetFloat("_Glow"), 0f, 2f).setOnUpdate((float val) =>
            {
                _heatEmission.material.SetFloat("_Glow", val);
            });

            yield return new WaitForSeconds(2f);

            _light.intensity = 0f;
            _heatEmission.color = Color.clear;
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