using UnityEngine;
using System.Collections;

public class Stack_Table : Table, IInteractable
{
    [Header("")]
    [SerializeField][Range(0, 100)] private int _maxStackCount;


    // MonoBehaviour
    public new void Start()
    {
        base.Start();

        // subscriptions
        stationController.detection.EnterEvent += AmountBar_Toggle;
        stationController.detection.ExitEvent += AmountBar_Toggle;
    }

    public new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        stationController.detection.EnterEvent -= AmountBar_Toggle;
        stationController.detection.ExitEvent -= AmountBar_Toggle;
    }


    // IInteractable
    public new void Interact()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        bool maxStacked = tableIcon.AllDatas().Count >= _maxStackCount;
        bool swapAvailable = tableIcon.hasFood == false || playerIcon.hasFood == false;

        if (maxStacked || swapAvailable)
        {
            Swap_Food();
            return;
        }

        Stack_Food();
    }


    // Functions
    private void AmountBar_Toggle()
    {
        bool hasFood = stationController.Food_Icon().hasFood;
        bool playerDetected = stationController.detection.player != null;

        stationController.Food_Icon().amountBar.Toggle(hasFood && playerDetected);
    }


    public void Swap_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();

        // swap
        Basic_SwapFood();

        // amount bar update
        tableIcon.amountBar.Load_Custom(_maxStackCount, tableIcon.AllDatas().Count);
        tableIcon.amountBar.Toggle(tableIcon.hasFood);
    }

    public void Stack_Food()
    {
        FoodData_Controller tableIcon = stationController.Food_Icon();
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        // stack player food
        tableIcon.Set_CurrentData(playerIcon.currentData);
        tableIcon.Show_Icon();
        tableIcon.Show_Condition();
        tableIcon.amountBar.Load_Custom(_maxStackCount, tableIcon.AllDatas().Count);

        // empty player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
    }
}
