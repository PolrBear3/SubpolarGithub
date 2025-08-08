using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroceryNPC_Data
{
    [ES3Serializable] private int _questCompleteCount;
    public int questCompleteCount => _questCompleteCount;
    
    [ES3Serializable] private bool _questComplete;
    public bool questComplete => _questComplete;
    
    [ES3Serializable] private List<FoodData> _unlockDatas;
    public List<FoodData> unlockDatas => _unlockDatas;


    // New
    public GroceryNPC_Data(HashSet<FoodData> startingDatas)
    {
        _questCompleteCount = 0;
        _questComplete = false;
        _unlockDatas = new(startingDatas);
    }
    
    public GroceryNPC_Data(GroceryNPC_Data loadData)
    {
        _questCompleteCount = loadData.questCompleteCount;
        _questComplete = loadData.questComplete;
        _unlockDatas = loadData.unlockDatas;
    }
    
    
    // Control
    public void Update_QuestCompleteCount(int updateValue)
    {
        _questCompleteCount += updateValue;

        if (_questCompleteCount >= 0) return;
        _questCompleteCount = 0;
    }

    public void Toggle_QuestCompleteState(bool toggle)
    {
        _questComplete = toggle;
    }
    
    
    public void Unlock_FoodData(Food_ScrObj foodScrObj)
    {
        foreach (FoodData data in _unlockDatas)
        {
            if (foodScrObj != data.foodScrObj) continue;
            
            int currentAmount = data.currentAmount;
            int setAmount = Mathf.Clamp(currentAmount + 1, 0, foodScrObj.unlockAmount);
            
            data.Set_Amount(setAmount);
            UnlockNew_FoodData(foodScrObj);
            
            return;
        }
        
        _unlockDatas.Add(new(foodScrObj));
    }
    
    public void UnlockNew_FoodData(Food_ScrObj checkFoodScrObj)
    {
        FoodData checkData = Unlocked_FoodData(checkFoodScrObj);
        
        if (checkData == null) return;
        if (checkData.currentAmount < checkFoodScrObj.unlockAmount) return;

        foreach (Food_ScrObj unlockFood in checkFoodScrObj.Unlocks())
        {
            if (Unlocked_FoodData(unlockFood) != null) continue;
            _unlockDatas.Add(new(unlockFood, 0));
        }
    }
    
    
    /// <returns>
    /// Unlock count (currentAmount) highest to lowest values to datas
    /// </returns>
    public List<FoodData> Unlocked_FoodDatas()
    {
        List<FoodData> unlockedDatas = new(_unlockDatas);

        for (int i = 0; i < unlockedDatas.Count - 1; i++)
        {
            int maxIndex = i;

            // compare and arrange most to least unlock count datas
            for (int j = i + 1; j < unlockedDatas.Count; j++)
            {
                if (unlockedDatas[j].currentAmount <= unlockedDatas[maxIndex].currentAmount) continue;

                maxIndex = j;
            }

            if (maxIndex == i) continue;

            FoodData temp = unlockedDatas[i];

            unlockedDatas[i] = unlockedDatas[maxIndex];
            unlockedDatas[maxIndex] = temp;
        }

        return unlockedDatas;
    }

    public List<FoodData> MaxUnlocked_FoodDatas()
    {
        List<FoodData> maxDatas = new();
        
        for (int i = 0; i < _unlockDatas.Count; i++)
        {
            if (_unlockDatas[i].currentAmount < _unlockDatas[i].foodScrObj.unlockAmount) continue;
            maxDatas.Add(_unlockDatas[i]);
        }
        
        return maxDatas;
    }
    

    private FoodData Unlocked_FoodData(Food_ScrObj foodScrObj)
    {
        foreach (FoodData data in _unlockDatas)
        {
            if (foodScrObj != data.foodScrObj) continue;
            return data;
        }
        return null;
    }

    public bool FoodData_UnlockMaxed(Food_ScrObj foodScrObj)
    {
        FoodData data = Unlocked_FoodData(foodScrObj);
        
        if (data == null) return false;
        if (data.currentAmount < foodScrObj.unlockAmount) return false;
        
        return true;
    }
    public bool FoodData_UnlockMaxed(List<Food_ScrObj> foodScrObjs)
    {
        for (int i = 0; i < foodScrObjs.Count; i++)
        {
            if (FoodData_UnlockMaxed(foodScrObjs[i])) continue;
            return false;
        }
        return true;
    }
}