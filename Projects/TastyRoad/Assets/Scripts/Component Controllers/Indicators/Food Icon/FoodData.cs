using System.Collections.Generic;

public enum FoodCondition_Type { sliced, heated, rotten }

[System.Serializable]
public class FoodCondition_Data
{
    public FoodCondition_Type stateType;
    public int stateLevel;

    public FoodCondition_Data (FoodCondition_Type type, int level)
    {
        stateType = type;
        stateLevel = level;
    }
}

[System.Serializable]
public class FoodData
{
    private Food_ScrObj _foodScrObj;
    public Food_ScrObj foodScrObj => _foodScrObj;

    private int _currentAmount;
    public int currentAmount => _currentAmount;

    private List<FoodCondition_Data> _conditionDatas;
    public List<FoodCondition_Data> conditionDatas => _conditionDatas;



    // Constructors
    public FoodData(Food_ScrObj food)
    {
        _foodScrObj = food;
        _currentAmount = 1;
        _conditionDatas = new();
    }



    // Methods
    public void Update_Condition(List<FoodCondition_Data> conditionDatas)
    {
        _conditionDatas = conditionDatas;
    }

    public void Update_Condition(FoodCondition_Data newCondition)
    {
        // check if condition exists

        // if so level +1

        // if not add new
    }
}
