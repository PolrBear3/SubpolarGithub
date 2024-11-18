using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrieveBox : Stack_Table, IInteractable
{
    [SerializeField] private Sprite _boxOpen;
    [SerializeField] private Sprite _boxClosed;


    // UnityEngine
    private new void Start()
    {
        Sprite_Update();

        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent += AmountBar_Toggle;
        detection.ExitEvent += AmountBar_Toggle;
    }

    private new void OnDestroy()
    {
        Retrieve_CurrentFood();

        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent -= AmountBar_Toggle;
        detection.ExitEvent -= AmountBar_Toggle;
    }


    // IInteractable
    public new void Interact()
    {
        base.Interact();

        Sprite_Update();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();

        Sprite_Update();
    }


    //
    private void Sprite_Update()
    {
        //
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

    private void Retrieve_CurrentFood()
    {
        // 
        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == false) return;

        FoodData foodData = foodIcon.currentData;

        FoodMenu_Controller foodMenu = stationController.mainController.currentVehicle.menu.foodMenu;
        StationMenu_Controller stationMenu = stationController.mainController.currentVehicle.menu.stationMenu;

        // retrieve current food data
        foodMenu.Add_FoodItem(foodData.foodScrObj, foodData.currentAmount);

        // station retrieve restriction
        stationMenu.Remove_StationItem(stationController.stationScrObj, 1);
    }
}
