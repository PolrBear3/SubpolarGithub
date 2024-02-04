using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [Header("Start Data")]
    [SerializeField] private float _displayTime;
    private Coroutine _amountCoroutine;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Station_Controller station)) { _stationController = station; }
    }

    private void Start()
    {
        _foodIcon.AmountBar_Transparency(true);
    }

    // IInteractable
    public void Interact()
    {
        Give_Food();
    }

    public void UnInteract()
    {

    }

    // Show Current Food Amount
    private IEnumerator Show_CurrentAmount_Coroutine()
    {
        _foodIcon.AmountBar_Transparency(false);

        yield return new WaitForSeconds(_displayTime);

        _foodIcon.AmountBar_Transparency(true);
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
        FoodData_Controller playerIcon = _stationController.detection.player.foodIcon;

        if (playerIcon.hasFood == false)
        {
            playerIcon.Assign_Food(_foodIcon.currentFoodData.foodScrObj);
            _foodIcon.Update_Amount(-1);

            Show_CurrentAmount();
        }
    }
}