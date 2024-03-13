using UnityEngine;
using System.Collections;

public class Stack_Table : Table, IInteractable
{
    // IInteractable
    public new void Interact()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        FoodData playerData = playerIcon.currentFoodData;

        if (playerData.stateData.Count > 0) return;

        if (playerIcon.hasFood == false || playerData.foodScrObj != stationController.Food_Icon().currentFoodData.foodScrObj)
        {
            Swap_Food();
        }
        else
        {
            Stack_Food();
        }
    }



    //
    private void Swap_Food()
    {
        FoodData_Controller icon = stationController.Food_Icon();
        FoodData tableData = icon.currentFoodData;

        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        if (playerIcon.hasFood == false && tableData.currentAmount > 1)
        {
            // give
            icon.Update_Amount(-1);
            icon.Show_AmountBar();

            playerIcon.Assign_Food(tableData.foodScrObj);
        }

        if (tableData.currentAmount > 1)
        {
            icon.Show_AmountBar();
            return;
        }

        // swap
        Basic_SwapFood();
    }

    private void Stack_Food()
    {
        FoodData_Controller icon = stationController.Food_Icon();

        if (icon.currentFoodData.currentAmount >= icon.maxAmount)
        {
            icon.Show_AmountBar();
            return;
        }

        // stack
        icon.Update_Amount(1);
        icon.Show_AmountBar();

        stationController.detection.player.foodIcon.Clear_Food();
    }
}
