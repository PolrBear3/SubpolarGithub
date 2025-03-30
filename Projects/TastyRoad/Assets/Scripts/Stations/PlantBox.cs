using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        stationController.detection.EnterEvent += Update_AmountBar;
        stationController.detection.ExitEvent += Update_AmountBar;

        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == false) return;
        if (foodIcon.currentData.Current_ConditionData(FoodCondition_Type.rotten) == null) return;

        _growthInProgress = true;
        GlobalTime_Controller.instance.OnTimeTik += Update_Growth;
    }

    private new void OnDestroy()
    {
        // subscriptions
        stationController.detection.EnterEvent -= Update_AmountBar;
        stationController.detection.ExitEvent -= Update_AmountBar;

        GlobalTime_Controller.instance.OnTimeTik -= Update_Growth;
    }


    // IInteractable
    public new void Interact()
    {
        Plant();
        Harvest();

        Update_Sprite();
    }

    public new void Hold_Interact()
    {
        if (_growthInProgress) return;
        if (stationController.Food_Icon().hasFood == false) return;

        // Harvest All
        Transfer_All();

        Update_Sprite();
        Update_AmountBar();
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
        stationController.Food_Icon().Toggle_SubDataBar(stationController.detection.player != null);
    }


    // Interact Functions
    private bool PlayerFood_Plantable()
    {
        if (_growthInProgress) return false;

        FoodData_Controller playerFoodIcon = Main_Controller.instance.Player().foodIcon;

        // check if player has food
        if (playerFoodIcon.hasFood == false) return false;

        // check if player food is rotten
        if (playerFoodIcon.currentData.Current_ConditionData(FoodCondition_Type.rotten) == null) return false;

        // check _plantableFoods contain player food
        for (int i = 0; i < _plantableFoods.Length; i++)
        {
            if (playerFoodIcon.currentData.foodScrObj != _plantableFoods[i]) continue;
            return true;
        }

        return false;
    }

    private void Plant()
    {
        if (_growthInProgress == true) return;

        if (stationController.Food_Icon().hasFood == true) return;

        if (PlayerFood_Plantable() == false) return;

        _growthInProgress = true;
        GlobalTime_Controller.instance.OnTimeTik += Update_Growth;

        Swap_Food();
        Update_AmountBar();
    }


    private void Update_Growth()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        // increase amount +1 growth
        foodIcon.Set_CurrentData(foodIcon.currentData);

        // check if growth complete
        if (foodIcon.DataCount_Maxed() == false) return;

        // remove rotten state
        for (int i = 0; i < foodIcon.AllDatas().Count; i++)
        {
            foodIcon.AllDatas()[i].Clear_Condition(FoodCondition_Type.rotten);
        }
        foodIcon.Show_Condition();

        // stop growth
        _growthInProgress = false;
        GlobalTime_Controller.instance.OnTimeTik -= Update_Growth;

        Update_AmountBar();
    }

    private void Harvest()
    {
        if (_growthInProgress) return;

        FoodData_Controller stationFoodIcon = stationController.Food_Icon();
        if (stationFoodIcon.hasFood == false) return;

        FoodData_Controller playerFoodIcon = Main_Controller.instance.Player().foodIcon;
        if (playerFoodIcon.DataCount_Maxed()) return;

        playerFoodIcon.Set_CurrentData(stationFoodIcon.currentData);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();
        playerFoodIcon.Toggle_SubDataBar(true);

        stationFoodIcon.Set_CurrentData(null);
        stationFoodIcon.Show_Icon();
        stationFoodIcon.Show_Condition();
        stationFoodIcon.Toggle_SubDataBar(true);
    }
}
