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
    private void Start()
    {
        if (stationController.Food_Icon().gameObject.TryGetComponent(out FoodData_RottenSystem rottenSystem))
        {
            rottenSystem.enabled = false;
        }

        Update_Sprite();

        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == false) return;
        if (foodIcon.currentData.Current_ConditionData(FoodCondition_Type.rotten) == null) return;

        _growthInProgress = true;
        GlobalTime_Controller.TimeTik_Update += Update_Growth;
    }

    private new void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= Update_Growth;
    }


    // IInteractable
    public new void Interact()
    {
        Plant();
        Harvest();

        Update_Sprite();
    }


    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.Food_Icon().ShowAmountBar_LockToggle(false);
        Update_AmountBar();
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


    // Functions
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
        if (_growthInProgress == true)
        {
            stationController.Food_Icon().Toggle_BarColor(false);
            return;
        }

        stationController.Food_Icon().Toggle_BarColor(true);
    }


    private void Plant()
    {
        if (_growthInProgress == true) return;

        FoodData_Controller playerFoodIcon = stationController.mainController.Player().foodIcon;

        // check if player has food
        if (playerFoodIcon.hasFood == false) return;

        // check if player food is plantable
        if (Food_Plantable(playerFoodIcon.currentData.foodScrObj) == false) return;

        // check if player food is rotten
        if (playerFoodIcon.currentData.Current_ConditionData(FoodCondition_Type.rotten) == null) return;

        // check if food is not planted
        if (stationController.Food_Icon().hasFood == true) return;

        // plant food
        Swap_Food();

        _growthInProgress = true;
        GlobalTime_Controller.TimeTik_Update += Update_Growth;

        Update_AmountBar();
        stationController.Food_Icon().Show_AmountBar();
    }

    private void Update_Growth()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        int maxAmount = foodIcon.amountBarSprites.Length;

        // increase amount +1
        foodIcon.currentData.Update_Amount(1);
        foodIcon.Show_AmountBar();

        // check if growth complete
        if (foodIcon.currentData.currentAmount >= maxAmount)
        {
            // remove rotten state
            foodIcon.currentData.Clear_Condition(FoodCondition_Type.rotten);

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
