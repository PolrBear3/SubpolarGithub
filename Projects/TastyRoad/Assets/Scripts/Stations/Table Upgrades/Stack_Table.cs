using UnityEngine;
using System.Collections;

public class Stack_Table : Table, IInteractable
{
    // IInteractable
    public new void Interact()
    {
        FoodData_Controller playerIcon = detection.player.foodIcon;
        FoodData playerData = playerIcon.currentFoodData;

        if (playerData.stateData.Count > 0) return;

        if (playerIcon.hasFood == false || playerData.foodScrObj != foodIcon.currentFoodData.foodScrObj)
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
        FoodData tableData = foodIcon.currentFoodData;
        FoodData_Controller playerIcon = detection.player.foodIcon;

        if (playerIcon.hasFood == false && tableData.currentAmount > 1)
        {
            // give
            foodIcon.Update_Amount(-1);
            foodIcon.Show_AmountBar();

            playerIcon.Assign_Food(tableData.foodScrObj);
        }

        if (tableData.currentAmount > 1)
        {
            foodIcon.Show_AmountBar();
            return;
        }

        // swap
        Basic_SwapFood();
    }

    private void Stack_Food()
    {
        if (foodIcon.currentFoodData.currentAmount >= foodIcon.maxAmount)
        {
            foodIcon.Show_AmountBar();
            return;
        }

        // stack
        foodIcon.Update_Amount(1);
        foodIcon.Show_AmountBar();

        detection.player.foodIcon.Clear_Food();
    }
}
