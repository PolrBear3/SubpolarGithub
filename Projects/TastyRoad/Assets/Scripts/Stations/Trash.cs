using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private Detection_Controller _detection;

    private List<FoodData> _trashedFoodData = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // IInteractable
    public void Interact()
    {
        Trash_Food();
    }

    public void UnInteract()
    {

    }

    // Trash Food
    private void Trash_Food()
    {
        FoodData_Controller playerFood = _detection.player.foodIcon;

        if (playerFood.hasFood == true)
        {
            // Save_TrashedFood_Data(playerFood.currentFoodData);

            playerFood.Clear_Food();
            playerFood.Clear_State();
        }
    }

    // Save Trashed Data
    private void Save_TrashedFood_Data(FoodData data)
    {
        /*
        FoodData trashedData = new();

        trashedData.foodScrObj = data.foodScrObj;
        trashedData.currentAmount = data.currentAmount;
        trashedData.stateData = new(data.stateData);

        _trashedFoodData.Add(trashedData);
        
        if (_trashedFoodData.Count >= 11)
        {
            _trashedFoodData.RemoveAt(0);
        }
        */
    }
}