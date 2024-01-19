using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    private SpriteRenderer _spriteRenderer;
    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;

    [SerializeField] private Sprite inActiveSprite;
    [SerializeField] private Sprite activeSprite;

    private Coroutine _heatCoroutine;

    [SerializeField] private float _heatIncreaseTime;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_detection.Has_Player() == true)
        {
            _foodIcon.FoodIcon_Transparency(false);

            Update_FoodIcon_Position(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_detection.Has_Player() == true)
        {
            _foodIcon.FoodIcon_Transparency(true);

            Update_FoodIcon_Position(false);
        }
    }

    // IInteractable
    public void Interact()
    {
        Swap_Food();
        Heat_Food();

        Update_CurrentSprite();
    }

    // Oven Sprite Update
    private void Update_CurrentSprite()
    {
        if (_foodIcon.hasFood)
        {
            _spriteRenderer.sprite = activeSprite;
        }
        else
        {
            _spriteRenderer.sprite = inActiveSprite;
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
