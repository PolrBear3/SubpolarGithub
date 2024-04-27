using System.Collections.Generic;
using UnityEngine;

public enum FoodCondition_Type { sliced, heated, rotten }

[System.Serializable]
public class FoodCondition_Data
{
    public FoodCondition_Type type;
    public int level;

    public FoodCondition_Data (FoodCondition_Type type)
    {
        this.type = type;
        level = 1;
    }

    public FoodCondition_Data (FoodCondition_Type type, int level)
    {
        this.type = type;

        if (level > 3) level = 3;

        this.level = level;
    }
}

[System.Serializable]
public class FoodData
{
    [SerializeField] [ES3Serializable] private Food_ScrObj _foodScrObj;
    public Food_ScrObj foodScrObj => _foodScrObj;

    [SerializeField] [ES3Serializable] private int _currentAmount;
    public int currentAmount => _currentAmount;

    [SerializeField] [ES3Serializable] private List<FoodCondition_Data> _conditionDatas;
    public List<FoodCondition_Data> conditionDatas => _conditionDatas;

    [SerializeField] [ES3Serializable] private int _timeTikCount;
    public int timeTikCount => _timeTikCount;



    // Constructors
    public FoodData(FoodData data)
    {
        _foodScrObj = data.foodScrObj;
        _currentAmount = data.currentAmount;
        _conditionDatas = data.conditionDatas;
        _timeTikCount = data.timeTikCount;
    }

    public FoodData(Food_ScrObj food)
    {
        _foodScrObj = food;
        _currentAmount = 1;
        _conditionDatas = new();
        _timeTikCount = 0;
    }



    // Functions
    public void Set_Amount(int setAmount)
    {
        _currentAmount = setAmount;
    }

    public void Update_Amount(int updateAmount)
    {
        _currentAmount += updateAmount;
    }



    public void Set_TimeTikCount(int setCount)
    {
        _timeTikCount = setCount;
    }



    public void Set_Condition(List<FoodCondition_Data> conditionDatas)
    {
        if (conditionDatas == null) return;

        _conditionDatas = conditionDatas;
    }

    public void Update_Condition(FoodCondition_Data newCondition)
    {
        // check if condition exists
        for (int i = 0; i < _conditionDatas.Count; i++)
        {
            // if so +level
            if (newCondition.type != _conditionDatas[i].type) continue;

            // set max level
            int calculatedLevel = _conditionDatas[i].level + newCondition.level;
            if (calculatedLevel > 3) return;

            _conditionDatas[i].level = calculatedLevel;
            return;
        }

        // if not add new type
        _conditionDatas.Add(new(newCondition.type, newCondition.level));
    }
}