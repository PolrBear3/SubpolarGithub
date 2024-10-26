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

    private FoodData _foodData;
    public FoodData foodData => _foodData;


    public Station_ScrObj currentStation;

    private StationData _stationData;
    public StationData stationData => _stationData;


    // Constructors
    public ItemSlot_Data()
    {
        hasItem = false;
        currentAmount = 0;
    }

    public ItemSlot_Data(ItemSlot_Data data)
    {
        hasItem = data.hasItem;

        bookMarked = data.bookMarked;
        isLocked = data.isLocked;

        currentAmount = data.currentAmount;

        currentFood = data.currentFood;
        currentStation = data.currentStation;
    }

    public ItemSlot_Data(Food_ScrObj food, int amount)
    {
        hasItem = true;

        currentFood = food;
        currentAmount = amount;
    }

    public ItemSlot_Data(Station_ScrObj station, int amount)
    {
        hasItem = true;

        currentStation = station;
        currentAmount = amount;
    }


    // Data Control Functions
    public void Empty_Item()
    {
        hasItem = bookMarked = isLocked = false;

        currentAmount = 0;

        currentFood = null;
        currentStation = null;
    }


    public void Update_FoodData(FoodData data)
    {
        if (data == null) return;

        _foodData = new(data);
    }

    public void Update_StationData(StationData data)
    {
        if (data == null) return;

        _stationData = new(data);
    }
}
