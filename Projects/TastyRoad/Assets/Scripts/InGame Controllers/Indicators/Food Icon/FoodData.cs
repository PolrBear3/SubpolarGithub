using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodState_Type { sliced, heated, rotten }

[System.Serializable]
public class FoodState_Data
{
    public FoodState_Type stateType;
    public int stateLevel;

    public FoodState_Data (FoodState_Type type, int level)
    {
        stateType = type;
        stateLevel = level;
    }
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
