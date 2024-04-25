using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;

    private Station_Controller _controller;

    [Header("")]
    [SerializeField] private Sprite inActiveSprite;
    [SerializeField] private Material _lit;

    [Header("")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Material _emissionGlow;
    [SerializeField] private GameObject _light;

    [Header("")]
    [SerializeField] private float _heatIncreaseTime;
    private Coroutine _heatCoroutine;



    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        if (gameObject.TryGetComponent(out Station_Controller station)) { _controller = station; }
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
        if (_controller.Food_Icon().hasFood)
        {
            _spriteRenderer.sprite = activeSprite;
            _spriteRenderer.material = _emissionGlow;
            _light.SetActive(true);
        }
        else
        {
            _spriteRenderer.sprite = inActiveSprite;
            _spriteRenderer.material = _lit;
            _light.SetActive(false);
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



    // Swap Current and Player Food
    public void Basic_SwapFood()
    {
        // swap data with player
        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;
        _controller.Food_Icon().Swap_Data(playerFoodIcon);

        // show player food data
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();

        // show table food data
        _controller.Food_Icon().Show_Icon();
        _controller.Food_Icon().Show_Condition();

        UnInteract();
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