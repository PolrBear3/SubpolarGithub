using UnityEngine;
using System.Collections;

public class Stack_Table : Table, IInteractable
{
    // MonoBehaviour
    public new void Start()
    {
        base.Start();

        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent += AmountBar_Toggle;
        detection.ExitEvent += AmountBar_Toggle;
    }

    public new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent -= AmountBar_Toggle;
        detection.ExitEvent -= AmountBar_Toggle;
    }


    // IInteractable
    public new void Interact()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        bool maxStacked = tableIcon.subDatas.Count >= tableIcon.maxSubDataCount;
        bool swapAvailable = tableIcon.hasFood == false || playerIcon.hasFood == false;

        if (maxStacked || swapAvailable)
        {
            Swap_Food();
            return;
        }

        Stack_Food();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();

        AmountBar_Toggle();
    }


    // Functions
    private void AmountBar_Toggle()
    {
        bool hasFood = stationController.Food_Icon().hasFood;
        bool playerDetected = stationController.detection.player != null;

        stationController.Food_Icon().Toggle_SubDataBar(hasFood && playerDetected);
    }


    public void Swap_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();

        // swap
        Basic_SwapFood();

        tableIcon.Toggle_SubDataBar(true);
    }

    public void Stack_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        // stack player food
        tableIcon.Set_CurrentData(playerIcon.currentData);
        tableIcon.Show_Icon();
        tableIcon.Show_Condition();
        tableIcon.Toggle_SubDataBar(true);

        // empty player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
    }
}
