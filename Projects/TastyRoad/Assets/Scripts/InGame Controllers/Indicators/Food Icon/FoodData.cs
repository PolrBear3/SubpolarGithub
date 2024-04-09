using System.Collections.Generic;

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
    public int currentTikTime;

    public FoodData (Food_ScrObj food, int amount)
    {
        foodScrObj = food;
        currentAmount = amount;
    }

    public FoodData (FoodData currentData)
    {
        foodScrObj = currentData.foodScrObj;
        stateData = currentData.stateData;
        currentAmount = currentData.currentAmount;
        currentTikTime = currentData.currentTikTime;
    }
}
