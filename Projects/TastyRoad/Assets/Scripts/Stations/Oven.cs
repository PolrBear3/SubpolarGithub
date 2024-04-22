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

        // _controller.Food_Icon().FoodIcon_Transparency(true);
        Update_FoodIcon_Position(false);
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        // _controller.Food_Icon().FoodIcon_Transparency(false);
        Update_FoodIcon_Position(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        // _controller.Food_Icon().FoodIcon_Transparency(true);
        Update_FoodIcon_Position(false);
    }



    // IInteractable
    public void Interact()
    {
        Swap_Food();
        Heat_Food();

        Update_CurrentVisual();
    }

    public void UnInteract()
    {

    }



    // Oven Visual Update
    private void Update_CurrentVisual()
    {
        /*
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
        */
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

    // Swap Oven and Player Food
    private void Swap_Food()
    {

    }

    // Food Heating System
    private void Heat_Food()
    {

    }
}
