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
    }

    private new void OnDestroy()
    {
        Retrieve_CurrentFood();
    }


    // IInteractable
    public new void Interact()
    {
        Swap_Food();
        Stack_Food();

        Sprite_Update();
    }


    // Functions
    private new void Swap_Food()
    {
        //
        FoodData_Controller playerFoodIcon = stationController.detection.player.foodIcon;

        // check if player food has no conditions
        if (playerFoodIcon.hasFood && playerFoodIcon.currentData.conditionDatas.Count > 0) return;

        //
        FoodData_Controller foodIcon = stationController.Food_Icon();
        FoodData foodData = foodIcon.currentData;

        // check if current amount is less than 1
        if (foodIcon.hasFood == true && foodData.currentAmount > 1) return;

        //
        base.Swap_Food();
    }

    private new void Stack_Food()
    {
        //
        FoodData_Controller foodIcon = stationController.Food_Icon();
        FoodData foodData = foodIcon.currentData;

        // check if current amount is more than 1
        if (foodIcon.hasFood == false) return;

        //
        FoodData_Controller playerFoodIcon = stationController.detection.player.foodIcon;

        // check if player has food
        if (playerFoodIcon.hasFood == false) return;

        // check if current food and player food is same
        if (playerFoodIcon.currentData.foodScrObj != foodData.foodScrObj) return;

        // check if player food has no conditions
        if (playerFoodIcon.currentData.conditionDatas.Count > 0) return;

        //
        base.Stack_Food();
    }


    private void Sprite_Update()
    {
        //
        FoodData_Controller foodIcon = stationController.Food_Icon();
        FoodData foodData = foodIcon.currentData;

        // current amount full
        if (foodIcon.hasFood == true && foodData.currentAmount >= 6)
        {
            stationController.spriteRenderer.sprite = _boxClosed;
            return;
        }

        // non full
        stationController.spriteRenderer.sprite = _boxOpen;
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
