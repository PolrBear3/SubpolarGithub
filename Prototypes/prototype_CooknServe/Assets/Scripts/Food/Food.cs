using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food
{
    [HideInInspector] public Food_ScrObj foodScrObj;
    [HideInInspector] public List<FoodState_Data> data = new List<FoodState_Data>();

    // Get
    public FoodState_Data Get_FoodState_Data(FoodState_Type stateType)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].stateType != stateType) continue;
            return data[i];
        }
        return null;
    }

    // Custom
    public void Set_Food(Food_ScrObj setFood)
    {
        foodScrObj = setFood;
    }

    public void Update_State(FoodState_Type updateType, int updateLevel)
    {
        if (updateLevel <= 0) return;

        // existing
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].stateType != updateType) continue;

            data[i].stateLevel += updateLevel;
            return;
        }

        // new
        FoodState_Data newData = new FoodState_Data();
        newData.stateType = updateType;
        newData.stateLevel = updateLevel;
        data.Add(newData);
    }
    public void Shuffle_State()
    {
        for (int i = 0; i < data.Count - 1; i++)
        {
            int randomNum = Random.Range(i, data.Count);
            FoodState_Data stateData = data[i];
            data[i] = data[randomNum];
            data[randomNum] = stateData;
        }
    }
}