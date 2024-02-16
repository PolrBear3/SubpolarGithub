using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;

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
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _foodIcon.FoodIcon_Transparency(false);
        Update_FoodIcon_Position(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _foodIcon.FoodIcon_Transparency(true);
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
        if (_foodIcon.hasFood)
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
            _foodIcon.transform.localPosition = new Vector2(0f, _foodIcon.transform.localPosition.y);
        }
        else
        {
            _foodIcon.transform.localPosition = new Vector2(-0.5f, _foodIcon.transform.localPosition.y);
        }
    }

    // Swap Oven and Player Food
    private void Swap_Food()
    {
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        Food_ScrObj ovenFood = _foodIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> ovenStateData = new(_foodIcon.currentFoodData.stateData);

        Food_ScrObj playerFood = playerIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> playerStateData = new(playerIcon.currentFoodData.stateData);

        _foodIcon.Assign_Food(playerFood);
        _foodIcon.Assign_State(playerStateData);

        playerIcon.Assign_Food(ovenFood);
        playerIcon.Assign_State(ovenStateData);
    }

    // Food Heating System
    private IEnumerator Heat_Food_Coroutine()
    {
        while (_foodIcon.hasFood == true)
        {
            yield return new WaitForSeconds(_heatIncreaseTime);

            _foodIcon.Update_State(FoodState_Type.heated, 1);
        }
    }
    private void Heat_Food()
    {
        if (_heatCoroutine != null)
        {
            StopCoroutine(_heatCoroutine);
        }

        if (_foodIcon.hasFood == true)
        {
            _heatCoroutine = StartCoroutine(Heat_Food_Coroutine());
        }
    }
}
