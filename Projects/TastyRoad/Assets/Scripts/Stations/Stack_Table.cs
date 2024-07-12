using UnityEngine;
using System.Collections;

public class Stack_Table : Table, IInteractable
{
    // UnityEngine
    private void Start()
    {
        stationController.Food_Icon().Show_AmountBar_Duration();
    }


    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.Food_Icon().ShowAmountBar_LockToggle(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.Food_Icon().ShowAmountBar_LockToggle(true);
    }



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

        // if both does not have food, don't swap
        if (tableIcon.hasFood == false && playerIcon.hasFood == false) return;

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

            // sound
            Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);

            return;
        }

        // swap
        Basic_SwapFood();
        tableIcon.Show_AmountBar();
    }


    public void Stack_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData tableData = tableIcon.currentData;

        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        if (tableIcon.currentData.currentAmount >= 6) return;

        // stack
        tableData.Update_Amount(1);
        tableIcon.Show_AmountBar();

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
    }
}
