using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot_Data
{
    public bool hasItem;
    public bool bookMarked;

    public bool isLocked;

    public int currentAmount;

    public Food_ScrObj currentFood;
    public Station_ScrObj currentStation;


    //
    public ItemSlot_Data()
    {
        
    }

    public ItemSlot_Data (ItemSlot_Data data)
    {
        hasItem = data.hasItem;
        bookMarked = data.bookMarked;

        isLocked = data.isLocked;

        currentAmount = data.currentAmount;

        currentFood = data.currentFood;
        currentStation = data.currentStation;
    }
}
