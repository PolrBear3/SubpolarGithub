using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trash : Stack_Table, IInteractable
{
    [Header("")]
    [SerializeField] private UnityEvent_Data[] _rewardDropActions;

    [Header("")]
    [SerializeField] private Station_ScrObj[] _dropStations;


    // MonoBehaviour
    private new void Start()
    {
        base.Start();

        stationController.Food_Icon().ShowIcon_LockToggle(true);
    }


    // IInteractable
    public new void Interact()
    {
        if (Drop_Reward())
        {
            PlayAnimation_TrashFood();
            AmountBar_Toggle();

            return;
        }

        PlayAnimation_TrashFood();
        Trash_Food();

        AmountBar_Toggle();
    }

    public new void Hold_Interact()
    {
        PlayAnimation_TrashFood();
        Trash_AllFood();

        AmountBar_Toggle();
    }


    // Trash Food
    private bool TrashFood_Available()
    {
        FoodData_Controller playerFoodIcon = stationController.detection.player.foodIcon;
        if (!playerFoodIcon.hasFood) return false;

        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (trashFoodIcon.DataCount_Maxed()) return false;

        return true;
    }


    private bool Trash_Food()
    {
        if (!TrashFood_Available()) return false;

        FoodData_Controller playerFoodIcon = stationController.detection.player.foodIcon;
        FoodData_Controller trashIcon = stationController.Food_Icon();

        FoodData trashData = new(playerFoodIcon.currentData);

        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();
        playerFoodIcon.Toggle_SubDataBar(true);

        trashIcon.Set_CurrentData(trashData);

        return true;
    }

    private void Trash_AllFood()
    {
        int repeatAmount = stationController.detection.player.foodIcon.AllDatas().Count;

        for (int i = 0; i < repeatAmount; i++)
        {
            if (!Trash_Food()) return;
        }
    }


    private void PlayAnimation_TrashFood()
    {
        if (!TrashFood_Available()) return;

        stationController.animController.Play_Animation("TransparencyBlinker_play");
    }

    private void Empty_MaxAmount()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (!trashFoodIcon.DataCount_Maxed()) return;

        trashFoodIcon.Update_AllDatas(null);
    }


    // Reward
    private bool RewardDrop_Available()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (!trashFoodIcon.DataCount_Maxed()) return false;

        return true;
    }

    private List<Vector2> RewardDrop_Positions()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller currentLocation = main.currentLocation;

        List<Vector2> centeredPositions = main.dataController.Centered_PositionDatas(transform.position, 1);
        List<Vector2> dropPositions = new();

        for (int i = 0; i < centeredPositions.Count; i++)
        {
            if (main.Position_Claimed(centeredPositions[i])) continue;
            if (currentLocation.Restricted_Position(centeredPositions[i])) continue;

            dropPositions.Add(centeredPositions[i]);
        }

        return dropPositions;
    }


    private UnityEvent RandomWeight_DropAction()
    {
        if (_rewardDropActions.Length <= 0) return null;

        float totalWeight = 0f;

        // Calculate total weight
        foreach (var action in _rewardDropActions)
        {
            totalWeight += action.probability;
        }

        if (totalWeight <= 0) return null;

        // Select a weighted random action
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var action in _rewardDropActions)
        {
            cumulativeWeight += action.probability;

            if (randValue > cumulativeWeight) continue;
            return action.action;
        }

        return null;
    }

    private bool Drop_Reward()
    {
        if (!RewardDrop_Available()) return false;

        List<Vector2> dropPositions = RewardDrop_Positions();
        if (dropPositions.Count <= 0) return false;

        Vector2 dropPos = dropPositions[0];

        ItemDropper item = stationController.itemDropper;
        item.Set_DropPosition(dropPos);

        RandomWeight_DropAction()?.Invoke();
        Empty_MaxAmount();

        return true;
    }


    // Reward Drops
    public void Drop_Food()
    {
        ItemDropper item = stationController.itemDropper;

        // set food drop amount //

        Food_ScrObj randFood = item.Weighted_RandomFood();
        FoodDrop dropFood = item.Drop_Food(new FoodData(randFood));

        // set rotten condition level //

        FoodCondition_Data condition = new(FoodCondition_Type.rotten, 3);
        dropFood.foodIcon.currentData.Update_Condition(condition);
    }

    public void Drop_Stations()
    {
        Debug.Log("Drop_Stations");
    }
}