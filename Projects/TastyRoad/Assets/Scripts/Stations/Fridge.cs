using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : Stack_Table//, IInteractable
{
    // IInteractable
    public new void Interact()
    {
        /*
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        FoodData playerData = playerIcon.currentData;

        if (playerData.stateData.Count > 0) return;

        if (playerIcon.hasFood == false || playerData.foodScrObj != stationController.Food_Icon().currentData.foodScrObj)
        {
            Swap_Food();

            restrict food decay
            stationController.Food_Icon().rottenSystem.UpdateDecay_Toggle(false); //
        }
        else
        {
            Stack_Food();
        }
        */
    }
}