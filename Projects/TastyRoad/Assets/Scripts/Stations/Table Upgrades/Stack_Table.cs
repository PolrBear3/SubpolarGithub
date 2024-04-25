using UnityEngine;
using System.Collections;

public class Stack_Table : Table, IInteractable
{
    // IInteractable
    public new void Interact()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        FoodData playerData = playerIcon.currentData;

        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData tableData = tableIcon.currentData;

        // if player food has a condition
        if (playerIcon.hasFood == true && playerData.conditionDatas.Count > 0) return;

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
    public void Swap_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData tableData = tableIcon.currentData;

        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        // decrease amount
        if (playerIcon.hasFood == false && tableData.currentAmount > 1)
        {
            // update table data
            tableData.Update_Amount(-1);

            if (tableData.currentAmount <= 0)
            {
                tableIcon.Set_CurrentData(null);
            }

            tableIcon.Show_Icon();
            tableIcon.Show_AmountBar();

            // give player
            playerIcon.Set_CurrentData(new FoodData(tableData.foodScrObj));
            playerIcon.currentData.Set_Condition(tableData.conditionDatas);

            playerIcon.Show_Icon();
            playerIcon.Show_Condition();

            return;
        }

        // swap
        Basic_SwapFood();
    }



    public void Stack_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData tableData = tableIcon.currentData;

        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        if (tableIcon.currentData.currentAmount >= 6)
        {
            // tableIcon.Show_AmountBar();
            return;
        }

        // stack
        tableData.Update_Amount(1);
        tableIcon.Show_AmountBar();

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
    }
}
