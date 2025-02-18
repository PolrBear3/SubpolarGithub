using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Trash : Stack_Table, IInteractable
{
    // MonoBehaviour
    private new void Start()
    {
        base.Start();

        stationController.Food_Icon().ShowIcon_LockToggle(true);
    }


    // IInteractable
    public new void Interact()
    {
        PlayAnimation_TrashFood();
        Trash_Food();

        Empty_MaxAmount();

        AmountBar_Toggle();
    }

    public new void Hold_Interact()
    {
        PlayAnimation_TrashFood();
        Trash_AllFood();

        Empty_MaxAmount();

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
    private bool RewardDrop_PositionEmpty()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller currentLocation = main.currentLocation;

        List<Vector2> positions = main.dataController.Centered_PositionDatas(transform.position, 1);

        for (int i = 0; i < positions.Count; i++)
        {
            if (main.Position_Claimed(positions[i])) continue;
            if (currentLocation.Restricted_Position(positions[i])) continue;

            return true;
        }

        return false;
    }

    private bool RewardDrop_Available()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (trashFoodIcon.DataCount_Maxed()) return false;

        if (!RewardDrop_PositionEmpty()) return false;

        return true;
    }


    private Vector2 RewardDrop_Position()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller currentLocation = main.currentLocation;

        List<Vector2> positions = main.dataController.Centered_PositionDatas(transform.position, 1);

        for (int i = 0; i < positions.Count; i++)
        {
            if (main.Position_Claimed(positions[i])) continue;
            if (currentLocation.Restricted_Position(positions[i])) continue;

            return positions[i];
        }

        return Vector2.zero;
    }

    private void Drop_Reward()
    {
        if (!RewardDrop_Available()) return;

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        ItemDropper item = stationController.itemDropper;
    }
}