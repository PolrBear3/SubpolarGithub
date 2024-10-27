using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot_Data
{
    [SerializeField][ES3Serializable] public bool hasItem;

    [SerializeField][ES3Serializable] public bool bookMarked;
    [SerializeField][ES3Serializable] public bool isLocked;

    [SerializeField][ES3Serializable] public int currentAmount;


    [SerializeField][ES3Serializable] private FoodData _foodData;
    public FoodData foodData => _foodData;

    [SerializeField][ES3Serializable] private Food_ScrObj _currentFood;
    public Food_ScrObj currentFood => _currentFood;


    [SerializeField][ES3Serializable] private StationData _stationData;
    public StationData stationData => _stationData;

    [SerializeField][ES3Serializable] private Station_ScrObj _currentStation;
    public Station_ScrObj currentStation => _currentStation;


    // Constructors
    public ItemSlot_Data()
    {
        hasItem = false;
        currentAmount = 0;
    }

    public ItemSlot_Data(ItemSlot_Data data)
    {
        hasItem = data.hasItem;
        currentAmount = data.currentAmount;

        bookMarked = data.bookMarked;
        isLocked = data.isLocked;

        _foodData = data.foodData;
        _currentFood = data.currentFood;

        _stationData = data.stationData;
        _currentStation = data.currentStation;
    }


    public ItemSlot_Data(FoodData data)
    {
        if (data == null) return;

        hasItem = true;
        currentAmount = data.currentAmount;

        _foodData = new(data);
        _currentFood = _foodData.foodScrObj;
    }

    public ItemSlot_Data(StationData data)
    {
        if (data == null) return;

        hasItem = true;
        currentAmount = 1;

        _stationData = new(data);
        _currentStation = _stationData.stationScrObj;
    }


    // Data Control Functions
    public void Empty_Item()
    {
        hasItem = bookMarked = isLocked = false;

        currentAmount = 0;

        _currentFood = null;
        _foodData = null;

        _stationData = null;
        _currentStation = null;
    }


    public void Update_FoodData(FoodData data)
    {
        if (data == null)
        {
            _foodData = null;
            _currentFood = null;

            return;
        }

        _foodData = new(data);
        _currentFood = _foodData.foodScrObj;
    }

    public void Update_StationData(StationData data)
    {
        if (data == null)
        {
            _stationData = null;
            _currentStation = null;

            return;
        }

        _stationData = new(data);
        _currentStation = _stationData.stationScrObj;
    }


    public void Assign_Amount(int assignAmount)
    {
        currentAmount = assignAmount;

        _foodData.Set_Amount(assignAmount);
    }

    public void Update_Amount(int updateAmount)
    {
        currentAmount += updateAmount;

        _foodData.Update_Amount(updateAmount);
    }
}
