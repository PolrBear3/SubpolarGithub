using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack_Table : Table
{
    // MonoBehaviour
    public new void Start()
    {
        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent += AmountBar_Toggle;
        detection.ExitEvent += AmountBar_Toggle;
        
        IInteractable_Controller interactable = stationController.iInteractable;
        FoodData_Controller foodIcon = stationController.Food_Icon();

        interactable.OnTriggerInteract += foodIcon.Show_Icon;
        interactable.OnTriggerInteract += foodIcon.Show_Condition;
        interactable.OnTriggerInteract += AmountBar_Toggle;
        
        interactable.OnInteract += Interact;
        interactable.OnHoldInteract += Transfer_All;
        
        interactable.OnAction1 += Stack_Food;
        interactable.OnAction2 += Pickup_Food;
        
        interactable.OnHoldInteract += AmountBar_Toggle;

        stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;
    }

    public new void OnDestroy()
    {
        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent -= AmountBar_Toggle;
        detection.ExitEvent -= AmountBar_Toggle;
        
        IInteractable_Controller interactable = stationController.iInteractable;
        FoodData_Controller foodIcon = stationController.Food_Icon();

        interactable.OnTriggerInteract -= foodIcon.Show_Icon;
        interactable.OnTriggerInteract -= foodIcon.Show_Condition;
        interactable.OnTriggerInteract -= AmountBar_Toggle;
        
        interactable.OnInteract -= Interact;
        interactable.OnHoldInteract -= Transfer_All;
        
        interactable.OnAction1 -= Stack_Food;
        interactable.OnAction2 -= Pickup_Food;
        
        interactable.OnHoldInteract -= AmountBar_Toggle;

        stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;
    }


    // IInteractable_Controller
    private void Interact()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        FoodData_Controller tableIcon = stationController.Food_Icon();

        bool swapAvailable = tableIcon.hasFood == false || playerIcon.hasFood == false;

        if (tableIcon.DataCount_Maxed() || swapAvailable)
        {
            Swap_Food();
            return;
        }

        Stack_Food();
    }


    // Functions
    public void AmountBar_Toggle()
    {
        bool hasFood = stationController.Food_Icon().hasFood;
        bool playerDetected = stationController.detection.player != null;

        stationController.Food_Icon().Toggle_SubDataBar(hasFood && playerDetected);
    }


    public void Swap_Food()
    {
        // swap
        SwapFood();

        FoodData_Controller tableIcon = stationController.Food_Icon();
        tableIcon.Toggle_SubDataBar(true);
    }

    public void Stack_Food()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        if (playerIcon.hasFood == false) return;
        
        FoodData_Controller tableIcon = stationController.Food_Icon();

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

        Audio_Controller audio = Audio_Controller.instance;
        
        if (tableIcon.DataCount_Maxed() == false)
        {
            // sound
            audio.Play_OneShot(gameObject, 1);
            return;
        }

        // sound
        audio.Play_OneShot(gameObject, 2);
    }

    public void Pickup_Food()
    {
        FoodData_Controller stationIcon = stationController.Food_Icon();
        if (stationIcon.hasFood == false) return;
        
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        if (playerIcon.DataCount_Maxed()) return;
        
        playerIcon.Set_CurrentData(stationIcon.currentData);
        stationIcon.Set_CurrentData(null);
        
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        stationIcon.Show_Icon();
        stationIcon.Show_Condition();
        stationIcon.Toggle_SubDataBar(true);

        // sound
        Audio_Controller audio = Audio_Controller.instance;
        
        if (playerIcon.DataCount_Maxed() == false)
        {
            audio.Play_OneShot(gameObject, 1);
            return;
        }
        audio.Play_OneShot(gameObject, 2);
    }

    
    public void Transfer_All()
    {
        FoodData_Controller stationIcon = stationController.Food_Icon();
        if (stationIcon.hasFood == false) return;

        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;
        if (playerIcon.DataCount_Maxed()) return;

        int transferAmount = stationIcon.AllDatas().Count;

        for (int i = 0; i < transferAmount; i++)
        {
            playerIcon.Set_CurrentData(stationIcon.currentData);
            stationIcon.Set_CurrentData(null);

            if (playerIcon.DataCount_Maxed()) break;
        }

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        stationIcon.Show_Icon();
        stationIcon.Show_Condition();
        stationIcon.Toggle_SubDataBar(true);

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 2);
    }
}
