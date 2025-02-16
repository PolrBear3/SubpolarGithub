using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Stack_Table, IInteractable
{
    [Header("")]
    [SerializeField] private GameObject _rat;


    // MonoBehaviour
    private new void Start()
    {
        base.Start();

        stationController.Food_Icon().ShowIcon_LockToggle(true);
    }


    // IInteractable
    public new void Interact()
    {
        Trash_Food();
        Empty_MaxAmount();

        AmountBar_Toggle();
    }

    public new void Hold_Interact()
    {
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


    private void Empty_MaxAmount()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (!trashFoodIcon.DataCount_Maxed()) return;

        trashFoodIcon.Update_AllDatas(null);
    }
}