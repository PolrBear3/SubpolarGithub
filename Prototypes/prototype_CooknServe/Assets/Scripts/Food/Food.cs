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

public class Food : MonoBehaviour
{
    [HideInInspector] public Food_ScrObj foodScrObj;
    [HideInInspector] public List<FoodState_Data> data = new List<FoodState_Data>();

    //
    public void Set_Food(Food_ScrObj setFood)
    {
        foodScrObj = setFood;
    }
    public void Update_State(FoodState_Type updateType, int updateLevel)
    {
        // existing
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].stateType == updateType)
            {
                data[i].stateLevel = updateLevel;
                return;
            }
        }

        // new
        FoodState_Data newData = new FoodState_Data();
        newData.stateType = updateType;
        newData.stateLevel = updateLevel;
        data.Add(newData);
    }
}