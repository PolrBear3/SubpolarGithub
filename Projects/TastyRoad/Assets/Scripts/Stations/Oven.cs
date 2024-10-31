using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODUnity;

public class Oven : MonoBehaviour, IInteractable
{
    [SerializeField] private Station_Controller _stationController;

    [Header("")]
    [SerializeField] private Sprite inActiveSprite;
    [SerializeField] private Sprite activeSprite;

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
    private void Start()
    {
        Heat_Food();
        Update_CurrentVisual();

        Update_FoodIcon_Position(false);
    }


    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _stationController.Food_Icon().ShowIcon_LockToggle(false);
        Update_FoodIcon_Position(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _stationController.Food_Icon().ShowIcon_LockToggle(true);
        Update_FoodIcon_Position(false);
    }


    // IInteractable
    public void Interact()
    {
        Basic_SwapFood();

        Heat_Food();
        Update_CurrentVisual();
    }

    public void UnInteract()
    {

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
        if (_stationController.Food_Icon().hasFood)
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


    // FoodIcon Position Update
    private void Update_FoodIcon_Position(bool playerDetected)
    {
        if (playerDetected == true)
        {
            _stationController.Food_Icon().transform.localPosition = new Vector2(0f, _stationController.Food_Icon().transform.localPosition.y);
        }
        else
        {
            _stationController.Food_Icon().transform.localPosition = new Vector2(-0.5f, _stationController.Food_Icon().transform.localPosition.y);
        }
    }


    //
    public void Basic_SwapFood()
    {
        FoodData_Controller playerFoodIcon = _stationController.detection.player.foodIcon;

        // check if food exist
        if (_stationController.Food_Icon().hasFood == false && playerFoodIcon.hasFood == false) return;

        // swap data with player
        _stationController.Food_Icon().Swap_Data(playerFoodIcon);

        // show player food data
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();

        // show table food data
        _stationController.Food_Icon().Show_Icon();
        _stationController.Food_Icon().Show_Condition();

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
    }



    // Food Heating System
    private void Heat_Food()
    {
        if (_heatCoroutine != null)
        {
            StopCoroutine(_heatCoroutine);
            _heatCoroutine = null;
        }

        if (_stationController.Food_Icon().hasFood == false) return;

        Audio_Controller.instance.Play_OneShot("Oven_switch", transform.position);

        _heatCoroutine = StartCoroutine(Heat_Food_Coroutine());
    }
    private IEnumerator Heat_Food_Coroutine()
    {
        FoodData_Controller foodIcon = _stationController.Food_Icon();

        while (foodIcon.hasFood)
        {
            yield return new WaitForSeconds(_heatIncreaseTime);

            foodIcon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.heated));
            foodIcon.Show_Condition();

            // durability
            _stationController.data.Update_Durability(-1);
            _stationController.maintenance.Update_DurabilityBreak();
        }
    }
}