using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Oven : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private Material _material;

    [SerializeField] private Station_Controller _controller;

    [Header("")]
    [SerializeField] private Sprite inActiveSprite;
    [SerializeField] private Sprite activeSprite;

    [Header("")]
    [SerializeField] private GameObject _heatEmission; 
    [SerializeField] private GameObject _light;

    private Coroutine _emissionCoroutine;

    [Header("")]
    [SerializeField] private float _heatIncreaseTime;
    private Coroutine _heatCoroutine;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        _material = _spriteRenderer.material;
    }

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

        _controller.Food_Icon().ShowIcon_LockToggle(false);
        Update_FoodIcon_Position(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _controller.Food_Icon().ShowIcon_LockToggle(true);
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
        if (_controller.Food_Icon().hasFood)
        {
            _light.SetActive(true);
            _heatEmission.SetActive(true);

            LeanTween.value(gameObject, 0f, 5f, 2f).setOnUpdate((float val) =>
            {
                _heatEmission.GetComponent<SpriteRenderer>().material.SetFloat("_Glow", val);
            });
        }
        // inactive
        else
        {
            LeanTween.value(gameObject, 5f, 0f, 2f).setOnUpdate((float val) =>
            {
                _heatEmission.GetComponent<SpriteRenderer>().material.SetFloat("_Glow", val);
            });

            yield return new WaitForSeconds(2f);

            _light.SetActive(false);
            _heatEmission.SetActive(false);
        }
    }


    // FoodIcon Position Update
    private void Update_FoodIcon_Position(bool playerDetected)
    {
        if (playerDetected == true)
        {
            _controller.Food_Icon().transform.localPosition = new Vector2(0f, _controller.Food_Icon().transform.localPosition.y);
        }
        else
        {
            _controller.Food_Icon().transform.localPosition = new Vector2(-0.5f, _controller.Food_Icon().transform.localPosition.y);
        }
    }


    //
    public void Basic_SwapFood()
    {
        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;

        // check if food exist
        if (_controller.Food_Icon().hasFood == false && playerFoodIcon.hasFood == false) return;

        // swap data with player
        _controller.Food_Icon().Swap_Data(playerFoodIcon);

        // show player food data
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();

        // show table food data
        _controller.Food_Icon().Show_Icon();
        _controller.Food_Icon().Show_Condition();

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
        Audio_Controller.instance.Play_OneShot("Oven_switch", transform.position);
    }



    // Food Heating System
    private void Heat_Food()
    {
        if (_heatCoroutine != null)
        {
            StopCoroutine(_heatCoroutine);
            _heatCoroutine = null;
        }

        if (_controller.Food_Icon().hasFood == false) return;

        Audio_Controller.instance.Play_OneShot("Oven_interaction", transform.position);

        _heatCoroutine = StartCoroutine(Heat_Food_Coroutine());
    }
    private IEnumerator Heat_Food_Coroutine()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();

        while (foodIcon.hasFood)
        {
            yield return new WaitForSeconds(_heatIncreaseTime);

            foodIcon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.heated));
            foodIcon.Show_Condition();
        }
    }
}