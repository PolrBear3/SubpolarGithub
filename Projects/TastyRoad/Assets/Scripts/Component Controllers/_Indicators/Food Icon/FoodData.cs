using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FoodWeight_Data
{
    public Food_ScrObj foodScrObj;
    [Range(0, 100)] public int weight;

    public FoodWeight_Data(Food_ScrObj foodScrObj, int weight)
    {
        this.foodScrObj = foodScrObj;
        this.weight = weight;
    }
}


public enum FoodCondition_Type { sliced, heated, rotten }

[System.Serializable]
public class FoodCondition_Data
{
    public FoodCondition_Type type;
    public int level;

    public FoodCondition_Data(FoodCondition_Type type)
    {
        this.type = type;
        level = 1;
    }

    public FoodCondition_Data(FoodCondition_Type type, int level)
    {
        this.type = type;

        if (level > 3) level = 3;

        this.level = level;
    }
}

[System.Serializable]
public class FoodData
{
    [SerializeField][ES3Serializable] private Food_ScrObj _foodScrObj;
    public Food_ScrObj foodScrObj => _foodScrObj;

    [ES3Serializable] private int _currentAmount;
    public int currentAmount => _currentAmount;

    [SerializeField][ES3Serializable] private int _tikCount;
    public int tikCount => _tikCount;

    [SerializeField][ES3Serializable] private List<FoodCondition_Data> _conditionDatas = new();
    public List<FoodCondition_Data> conditionDatas => _conditionDatas;


    // Constructors
    public FoodData(FoodData data)
    {
        if (data == null) return;

        _foodScrObj = data.foodScrObj;
        _currentAmount = data.currentAmount;
        _tikCount = data.tikCount;
        _conditionDatas = data.conditionDatas;
    }


    public FoodData(Food_ScrObj food)
    {
        _foodScrObj = food;
        _currentAmount = 1;

        _conditionDatas = new();
    }

    public FoodData(Food_ScrObj food, int amount)
    {
        _foodScrObj = food;
        _currentAmount = amount;

        _conditionDatas = new();
    }


    // Amount
    public void Set_Amount(int setAmount)
    {
        _currentAmount = setAmount;
    }

    public void Update_Amount(int updateAmount)
    {
        _currentAmount += updateAmount;
    }


    // TimTik Count
    public void Set_TikCount(int setAmount)
    {
        _tikCount = setAmount;

        if (_tikCount <= 0) _tikCount = 0;
    }

    public void Update_TikCount(int updateAmount)
    {
        _tikCount += updateAmount;

        if (_tikCount <= 0) _tikCount = 0;
    }


    // Condition
    public void Set_Condition(List<FoodCondition_Data> conditionDatas)
    {
        if (conditionDatas.Count <= 0) return;

        _conditionDatas = new(conditionDatas);
    }

    public void Update_Condition(FoodCondition_Data newCondition)
    {
        if (newCondition == null) return;

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


    public void Clear_Condition()
    {
        _conditionDatas.Clear();
    }
    public void Clear_Condition(FoodCondition_Type targetCondition)
    {
        for (int i = 0; i < _conditionDatas.Count; i++)
        {
            if (_conditionDatas[i].type != targetCondition) continue;
            _conditionDatas.RemoveAt(i);
        }
    }


    public FoodCondition_Data Current_ConditionData(FoodCondition_Type targetCondition)
    {
        for (int i = 0; i < _conditionDatas.Count; i++)
        {
            if (_conditionDatas[i].type != targetCondition) continue;
            return _conditionDatas[i];
        }

        return null;
    }

    public int Conditions_MatchCount(List<FoodCondition_Data> conditionsToCompare)
    {
        int matchCount = 0;

        foreach (var condition in conditionsToCompare)
        {
            foreach (var currentCondition in _conditionDatas)
            {
                if (condition.type == currentCondition.type && condition.level == currentCondition.level)
                {
                    matchCount++;
                    break;
                }
            }
        }

        return matchCount;
    }
}