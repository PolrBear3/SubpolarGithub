using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;



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



    // Give Food to Player
    private void Give_Food()
    {
        FoodData_Controller playerIcon = _stationController.detection.player.foodIcon;

        if (playerIcon.hasFood == false)
        {
            playerIcon.Assign_Food(_foodIcon.currentFoodData.foodScrObj);

            _foodIcon.Update_Amount(-1);
            _foodIcon.Show_AmountBar();
        }
    }
}