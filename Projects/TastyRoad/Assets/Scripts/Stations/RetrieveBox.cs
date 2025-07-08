using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrieveBox : Stack_Table
{
    [SerializeField] private Sprite _boxOpen;
    [SerializeField] private Sprite _boxClosed;


    // UnityEngine
    private new void Start()
    {
        Sprite_Update();

        // subscriptions
        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent += AmountBar_Toggle;
        detection.ExitEvent += AmountBar_Toggle;
    }

    private new void OnDestroy()
    {
        Retrieve_CurrentFood();
        Remove_onRetrieve();

        // subscriptions
        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent -= AmountBar_Toggle;
        detection.ExitEvent -= AmountBar_Toggle;
    }


    // IInteractable
    private void Interact()
    {
        Perform_FoodInteraction();
        Sprite_Update();
    }

    private void Hold_Interact()
    {
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        bool hasCondition = playerIcon.hasFood && playerIcon.currentData.conditionDatas.Count > 0;
        if (hasCondition) return;

        // base.Hold_Interact();
        Sprite_Update();
    }


    //
    private void Sprite_Update()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        // current amount full
        if (foodIcon.DataCount_Maxed())
        {
            stationController.spriteRenderer.sprite = _boxClosed;
            foodIcon.Toggle_Height(true);
            return;
        }

        // non full
        stationController.spriteRenderer.sprite = _boxOpen;
        foodIcon.Toggle_Height(false);
    }


    private void Perform_FoodInteraction()
    {
        FoodData_Controller stationIcon = stationController.Food_Icon();
        FoodData_Controller playerIcon = stationController.detection.player.foodIcon;

        bool hasCondition = playerIcon.hasFood && playerIcon.currentData.conditionDatas.Count > 0;
        if (hasCondition) return;

        bool swapAvailable = playerIcon.hasFood == false || stationIcon.DataCount_Maxed();
        if (swapAvailable)
        {
            Swap_Food();
            return;
        }

        Stack_Food();
    }

    private void Retrieve_CurrentFood()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == false) return;

        Main_Controller main = Main_Controller.instance;

        FoodMenu_Controller foodMenu = main.currentVehicle.menu.foodMenu;
        StationMenu_Controller stationMenu = main.currentVehicle.menu.stationMenu;

        // retrieve current food data
        foreach (FoodData data in foodIcon.AllDatas())
        {
            foodMenu.Add_FoodItem(data.foodScrObj, 1);
        }

        // station retrieve restriction
        stationMenu.Remove_StationItem(stationController.stationScrObj, 1);
    }

    private void Remove_onRetrieve()
    {
        if (stationController.Food_Icon().hasFood == false) return;

        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        List<ItemSlot> currentSlots = menu.controller.slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.StationData_Match(stationController.data) == false) continue;

            currentSlots[i].Empty_ItemBox();
            return;
        }
    }
}
