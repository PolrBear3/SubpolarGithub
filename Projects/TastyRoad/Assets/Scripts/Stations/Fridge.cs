using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : Stack_Table, IInteractable
{
    // UnityEngine
    private void Start()
    {
        Deactivate_RottenSystem();
    }



    // IInteractable
    public new void Interact()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        FoodData playerData = playerIcon.currentData;

        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData tableData = tableIcon.currentData;

        bool bothHaveFood = playerIcon.hasFood && tableIcon.hasFood;

        if (bothHaveFood && playerData.foodScrObj == tableData.foodScrObj)
        {
            Stack_Food();
        }
        else
        {
            Swap_Food();
        }
    }



    //
    private void Deactivate_RottenSystem()
    {
        if (!stationController.Food_Icon().gameObject.TryGetComponent(out FoodData_RottenSystem rottenSystem)) return;
        rottenSystem.enabled = false;
    }
}