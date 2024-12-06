using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODUnity;

public class Oven : Table, IInteractable
{
    [Header("")]
    [SerializeField] private SpriteRenderer _heatEmission;
    [SerializeField] private Light2D _light;

    private Coroutine _emissionCoroutine;

    [Header("")]
    [SerializeField] private float _heatIncreaseTime;
    [SerializeField][Range(0, 10)] private float _emissionValue;
    [SerializeField][Range(0, 10)] private float _lightValue;

    private Coroutine _heatCoroutine;


    // UnityEngine
    private new void Start()
    {
        base.Start();

        Heat_Food();
        Update_CurrentVisual();

        FoodIcon_PositionToggle();

        // subscriptions
        stationController.detection.EnterEvent += FoodIcon_PositionToggle;
        stationController.detection.ExitEvent += FoodIcon_PositionToggle;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        stationController.detection.EnterEvent -= FoodIcon_PositionToggle;
        stationController.detection.ExitEvent -= FoodIcon_PositionToggle;
    }


    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();
        FoodIcon_PositionToggle();

        Heat_Food();
        Update_CurrentVisual();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();
        FoodIcon_PositionToggle();

        Heat_Food();
        Update_CurrentVisual();
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
        if (stationController.Food_Icon().hasFood)
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

    private void FoodIcon_PositionToggle()
    {
        float posY = stationController.Food_Icon().transform.localPosition.y;

        if (stationController.detection.player != null)
        {
            stationController.Food_Icon().transform.localPosition = new Vector2(0f, posY);
            stationController.Food_Icon().Show_Icon();
            return;
        }

        stationController.Food_Icon().transform.localPosition = new Vector2(-0.5f, posY);
        stationController.Food_Icon().Hide_Icon();
    }


    // Food Heating System
    private void Heat_Food()
    {
        if (_heatCoroutine != null)
        {
            StopCoroutine(_heatCoroutine);
            _heatCoroutine = null;
        }

        if (stationController.Food_Icon().hasFood == false) return;

        // oven switch
        Audio_Controller.instance.Play_OneShot(gameObject, 2);

        _heatCoroutine = StartCoroutine(Heat_Food_Coroutine());
    }
    private IEnumerator Heat_Food_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (foodIcon.hasFood)
        {
            yield return new WaitForSeconds(_heatIncreaseTime);

            foodIcon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.heated));
            foodIcon.Show_Condition();

            // durability
            stationController.data.Update_Durability(-1);
            stationController.maintenance.Update_DurabilityBreak();
        }
    }
}