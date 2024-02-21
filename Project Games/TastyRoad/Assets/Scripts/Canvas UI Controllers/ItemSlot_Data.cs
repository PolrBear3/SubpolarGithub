using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot_Data
{
    public bool hasItem;
    public bool bookMarked;

    public int currentAmount;

    public Food_ScrObj currentFood;
    public Station_ScrObj currentStation;
}
