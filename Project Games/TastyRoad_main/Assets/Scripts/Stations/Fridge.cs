using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;

    [SerializeField] private SpriteRenderer _targetIndicator;

    [Header("Start Data")]
    [SerializeField] private float _displayTime;
    private Coroutine _amountCoroutine;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    private void Start()
    {
        _foodIcon.AmountText_Transparency(true);

        TargetIndicator_Toggle(false);
    }

    // IInteractable
    public void Interact()
    {
        Give_Food();
    }

    // Show Current Food Amount
    private IEnumerator Show_CurrentAmount_Coroutine()
    {
        _foodIcon.AmountText_Transparency(false);

        yield return new WaitForSeconds(_displayTime);

        _foodIcon.AmountText_Transparency(true);
    }
    private void Show_CurrentAmount()
    {
        if (_amountCoroutine != null)
        {
            StopCoroutine(_amountCoroutine);
        }

        _amountCoroutine = StartCoroutine(Show_CurrentAmount_Coroutine());
    }

    // Give Food to Player
    private void Give_Food()
    {
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        if (playerIcon.hasFood == false)
        {
            _foodIcon.Update_Amount(-1);
            playerIcon.Assign_Food(_foodIcon.currentFoodData.foodScrObj);

            Show_CurrentAmount();
        }
    }

    // Export Food from Vehicle to Fridge System
    public FoodData_Controller FoodIcon()
    {
        return _foodIcon;
    }

    public void TargetIndicator_Toggle(bool toggleOn)
    {
        if (toggleOn == true)
        {
            TargetIndicator_Position();

            _targetIndicator.color = Color.white;

            return;
        }

        _targetIndicator.color = Color.clear;
    }

    private void TargetIndicator_Position()
    {
        if (_foodIcon.hasFood)
        {
            float foodPositionY = _foodIcon.currentFoodData.foodScrObj.centerPosition.y;
            _targetIndicator.transform.localPosition = new Vector2(0f, foodPositionY + 4.1f);

            return;
        }

        _targetIndicator.transform.localPosition = new Vector2(0f, 0.63f);
    }
}