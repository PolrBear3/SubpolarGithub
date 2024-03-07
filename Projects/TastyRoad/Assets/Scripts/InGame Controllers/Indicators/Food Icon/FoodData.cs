using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodState_Type { sliced, heated }

[System.Serializable]
public class FoodState_Data
{
    public FoodState_Type stateType;
    public int stateLevel;
}

[System.Serializable]
public class FoodData
{
    public Food_ScrObj foodScrObj;
    public int currentAmount;
    public List<FoodState_Data> stateData;

    public FoodData (Food_ScrObj food, int amount)
    {
        foodScrObj = food;
        currentAmount = amount;
    }

    public FoodData (FoodData currentData)
    {
        foodScrObj = currentData.foodScrObj;
        currentAmount = currentData.currentAmount;
        stateData = currentData.stateData;
    }
}
