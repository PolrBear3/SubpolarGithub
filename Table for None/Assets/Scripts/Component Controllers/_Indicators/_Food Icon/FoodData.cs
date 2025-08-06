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


public enum FoodCondition_Type { sliced, heated, frozen, rotten }

[System.Serializable]
public class FoodCondition_Data
{
    public FoodCondition_Type type;
    public int level;

    // New
    public FoodCondition_Data(FoodCondition_Type type)
    {
        this.type = type;
        level = 1;
    }

    public FoodCondition_Data(FoodCondition_Type type, int level)
    {
        this.type = type;
        this.level = Mathf.Clamp(level, 1, 3);
    }
}


[System.Serializable]
public class FoodData
{
    [SerializeField][ES3Serializable] private Food_ScrObj _foodScrObj;
    public Food_ScrObj foodScrObj => _foodScrObj;

    [SerializeField][ES3Serializable] private int _currentAmount;
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

    public void Update_Condition(FoodCondition_Data updateCondition)
    {
        if (updateCondition == null) return;
        if (updateCondition.level <= 0) return;

        // check if condition exists
        for (int i = 0; i < _conditionDatas.Count; i++)
        {
            if (updateCondition.type != _conditionDatas[i].type) continue;

            // set max level limits
            int calculatedLevel = _conditionDatas[i].level + updateCondition.level;
            if (calculatedLevel > 3) return;

            _conditionDatas[i].level = calculatedLevel;
            return;
        }

        // if not add new type
        _conditionDatas.Add(new(updateCondition.type, updateCondition.level));
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


    public bool Conditions_Match(List<FoodCondition_Data> conditionsToCompare)
    {
        return Conditions_MatchCount(conditionsToCompare) == _conditionDatas.Count;
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

    public int ConditionLevel_MatchCount(List<FoodCondition_Data> conditionsToCompare)
    {
        int matchCount = 0;

        foreach (var condition in conditionsToCompare)
        {
            foreach (var currentCondition in _conditionDatas)
            {
                if (condition.type != currentCondition.type) continue;
                
                matchCount += Mathf.Min(condition.level, currentCondition.level);
                break;
            }
        }
        
        return matchCount;
    }
    

    public bool Has_Condition(FoodCondition_Type targetCondition)
    {
        return Current_ConditionData(targetCondition) != null;
    }

    public int Current_ConditionLevel(FoodCondition_Type targetCondition)
    {
        FoodCondition_Data targetData = Current_ConditionData(targetCondition);

        if (targetData == null) return 0;
        return targetData.level;
    }
}