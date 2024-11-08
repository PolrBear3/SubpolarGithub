using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBox : Stack_Table, IInteractable
{
    [Header("")]
    [SerializeField] private Food_ScrObj[] _plantableFoods;

    [Header("")]
    [SerializeField] private Sprite _unPlantedSprite;
    [SerializeField] private Sprite _plantedSprite;

    private bool _growthInProgress;


    // UnityEngine
    private new void Start()
    {
        if (stationController.Food_Icon().gameObject.TryGetComponent(out FoodData_RottenSystem rottenSystem))
        {
            rottenSystem.enabled = false;
        }

        Update_Sprite();

        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == false) return;
        if (foodIcon.headData.Current_ConditionData(FoodCondition_Type.rotten) == null) return;

        _growthInProgress = true;

        // subscriptions
        stationController.detection.EnterEvent += Update_AmountBar;
        stationController.detection.ExitEvent += Update_AmountBar;

        GlobalTime_Controller.TimeTik_Update += Update_Growth;
    }

    private new void OnDestroy()
    {
        // subscriptions
        stationController.detection.EnterEvent -= Update_AmountBar;
        stationController.detection.ExitEvent -= Update_AmountBar;

        GlobalTime_Controller.TimeTik_Update -= Update_Growth;
    }


    // IInteractable
    public new void Interact()
    {
        Plant();
        Harvest();

        Update_Sprite();
    }


    // Gets
    private bool Food_Plantable(Food_ScrObj checkFood)
    {
        for (int i = 0; i < _plantableFoods.Length; i++)
        {
            if (checkFood != _plantableFoods[i]) continue;
            return true;
        }
        return false;
    }


    // Viusal Update
    private void Update_Sprite()
    {
        if (stationController.Food_Icon().hasFood == false)
        {
            stationController.spriteRenderer.sprite = _unPlantedSprite;
            return;
        }

        stationController.spriteRenderer.sprite = _plantedSprite;
    }

    private void Update_AmountBar()
    {
        stationController.Food_Icon().amountBar.Toggle_BarColor(_growthInProgress);
        stationController.Food_Icon().Toggle_AmountBar(stationController.detection.player != null);
    }


    // Interact Functions
    private void Plant()
    {
        if (_growthInProgress == true) return;

        FoodData_Controller playerFoodIcon = stationController.mainController.Player().foodIcon;

        // check if player has food
        if (playerFoodIcon.hasFood == false) return;

        // check if player food is plantable
        if (Food_Plantable(playerFoodIcon.headData.foodScrObj) == false) return;

        // check if player food is rotten
        if (playerFoodIcon.headData.Current_ConditionData(FoodCondition_Type.rotten) == null) return;

        // check if food is not planted
        if (stationController.Food_Icon().hasFood == true) return;

        // plant player food
        Swap_Food();

        _growthInProgress = true;
        GlobalTime_Controller.TimeTik_Update += Update_Growth;

        Update_AmountBar();
        stationController.Food_Icon().Show_AmountBar();
    }


    private void Update_Growth()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        // increase amount +1
        foodIcon.headData.Update_Amount(1);
        foodIcon.Show_AmountBar();

        // check if growth complete
        if (foodIcon.amountBar.Is_MaxAmount())
        {
            // remove rotten state
            foodIcon.headData.Clear_Condition(FoodCondition_Type.rotten);
            foodIcon.Show_Condition();

            // stop growth
            _growthInProgress = false;
            GlobalTime_Controller.TimeTik_Update -= Update_Growth;

            Update_AmountBar();
            foodIcon.Show_AmountBar();
        }
    }

    private void Harvest()
    {
        if (_growthInProgress == true) return;

        FoodData_Controller playerFoodIcon = stationController.mainController.Player().foodIcon;

        if (playerFoodIcon.hasFood == true) return;
        // give player food
        Swap_Food();

        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == true) return;

        _growthInProgress = false;
        GlobalTime_Controller.TimeTik_Update -= Update_Growth;
    }
}
