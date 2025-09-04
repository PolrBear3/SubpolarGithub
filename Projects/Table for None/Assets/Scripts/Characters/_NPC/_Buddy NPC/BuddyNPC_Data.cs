using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuddyNPC_Data
{
    [ES3Serializable] private FoodData _automateFoodData;
    public FoodData automateFoodData => _automateFoodData;

    [ES3Serializable] private List<FoodData> _collectedIngredients = new();
    public List<FoodData> collectedIngredients => _collectedIngredients;

    [ES3Serializable] private int _maxMergeCount;
    public int maxMergeCount => _maxMergeCount;
    
    [ES3Serializable] private int _currentMergeCount;
    public int currentMergeCount => _currentMergeCount;
    
    
    // New
    public BuddyNPC_Data(FoodData automateFoodData, int maxMergeCount)
    {
        _automateFoodData = automateFoodData;
        
        _maxMergeCount = maxMergeCount;
        _currentMergeCount = _maxMergeCount;
    }
    
    
    // Get
    public bool Ingredient_Collected(Food_ScrObj ingredient)
    {
        List<Food_ScrObj> requiredIngredients = new(_automateFoodData.foodScrObj.Ingredients());
        int collectedCount = 0;
        
        foreach (FoodData data in _collectedIngredients)
        {
            if (data.foodScrObj != ingredient) continue;
            collectedCount++;
        }

        for (int i = 0; i < collectedCount; i++)
        {
            if (!requiredIngredients.Remove(ingredient)) break;
        }

        return !requiredIngredients.Contains(ingredient);
    }
    
    public bool Ingredients_Collected()
    {
        if (_collectedIngredients.Count == 0) return false;
        
        List<Food_ScrObj> ingredients = _automateFoodData.foodScrObj.Ingredients();

        for (int i = 0; i < ingredients.Count; i++)
        {
            if (Ingredient_Collected(ingredients[i])) continue;

            return false;
        }

        return true;
    }
    
    public List<FoodCondition_Data> Calculated_ConditionDatas()
    {
        List<FoodData> ingredients = _automateFoodData.foodScrObj.Conditioned_Ingredients();
        List<FoodCondition_Data> conditionDatas = new();

        for (int i = 0; i < ingredients.Count; i++)
        {
            for (int j = 0; j < ingredients[i].conditionDatas.Count; j++)
            {
                FoodCondition_Data conditionData = ingredients[i].conditionDatas[j];
                bool hasCondition = false;
                
                for (int k = 0; k < conditionDatas.Count; k++)
                {
                    if (conditionData.type != conditionDatas[k].type) continue;
                    
                    conditionDatas[k].level += conditionData.level;
                    hasCondition = true;
                    
                    break;
                }
                
                if (hasCondition) continue;
                conditionDatas.Add(new(conditionData.type, conditionData.level));
            }
        }
        return conditionDatas;
    }
    
    
    // Data Control
    public void Remove_CollectedIngredient(Food_ScrObj ingredient)
    {
        for (int i = 0; i < _collectedIngredients.Count; i++)
        {
            if (ingredient != _collectedIngredients[i].foodScrObj) continue;
            
            _collectedIngredients.RemoveAt(i);
            return;
        }
    }

    public void Update_MergeCount(int updateValue)
    {
        _currentMergeCount += updateValue;
    }
}
