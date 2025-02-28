using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    void Interact();
    void Hold_Interact();

    void UnInteract();
}

public interface ISignal
{
    void Signal();
}


[System.Serializable]
public struct UnityEvent_Data
{
    [Range(0, 100)] public float probability;
    public UnityEvent action;
}

[System.Serializable]
public class Multiple_PositionData
{
    [SerializeField] private Vector2[] _positionData;
    public Vector2[] positionData => _positionData;
}


public class Data_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private WorldData[] _worldData;
    public WorldData[] worldData => _worldData;

    [Header("")]
    public List<Station_ScrObj> stations = new();
    public List<GameObject> characters = new();

    [Header("")]
    [SerializeField] private Food_ScrObj _goldenNugget;
    public Food_ScrObj goldenNugget => _goldenNugget;

    [Header("")]
    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();

    [Header("")]
    [SerializeField] private Multiple_PositionData[] _positionDatas;
    public Multiple_PositionData[] positionDatas => _positionDatas;


    // Get World and Location
    public WorldData World_Data(int targetWorldNum)
    {
        if (targetWorldNum < 0 || targetWorldNum > worldData.Length - 1) return null;

        return _worldData[targetWorldNum];
    }

    public int LocationCount_inWorld(int targetWorldNum)
    {
        if (targetWorldNum < 0 || targetWorldNum > _worldData.Length - 1) return 0;

        for (int i = 0; i < _worldData.Length; i++)
        {
            if (targetWorldNum != i) continue;
            return _worldData[i].locations.Length;
        }

        return 0;
    }


    // Get Station
    public Station_ScrObj Station_ScrObj(int stationID)
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stationID != stations[i].id) continue;
            return stations[i];
        }
        return null;
    }
    public Station_ScrObj Station_ScrObj(Station_ScrObj stationScrObj)
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stationScrObj != stations[i]) continue;
            return stations[i];
        }
        return null;
    }
    public Station_ScrObj Station_ScrObj(string stationName)
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stationName != stations[i].stationName) continue;
            return stations[i];
        }
        return null;
    }

    /// <returns>
    /// Random Station
    /// </returns>
    public Station_ScrObj Station_ScrObj()
    {
        Station_ScrObj randStation = stations[Random.Range(0, stations.Count)];
        return randStation;
    }


    // Get All Food
    public List<Food_ScrObj> AllFoods()
    {
        List<Food_ScrObj> allFoods = new();

        // add all raw foods
        for (int i = 0; i < rawFoods.Count; i++)
        {
            allFoods.Add(rawFoods[i]);
        }

        // add all cooked foods
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            allFoods.Add(cookedFoods[i]);
        }

        return allFoods;
    }

    public Food_ScrObj Food()
    {
        return AllFoods()[Random.Range(0, AllFoods().Count)];
    }
    public Food_ScrObj Food(int foodID)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodID != rawFoods[i].id) continue;
            return rawFoods[i];
        }

        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodID != cookedFoods[i].id) continue;
            return cookedFoods[i];
        }

        return null;
    }
    public Food_ScrObj Food(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodScrObj != rawFoods[i]) continue;
            return rawFoods[i];
        }

        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodScrObj != cookedFoods[i]) continue;
            return cookedFoods[i];
        }

        return null;
    }

    public Food_ScrObj Food(List<FoodData> ingredientDatas)
    {
        for (int i = 0; i < AllFoods().Count; i++)
        {
            if (!AllFoods()[i].Ingredients_Match(ingredientDatas)) continue;
            return AllFoods()[i];
        }

        return null;
    }


    // Get Raw Food
    public Food_ScrObj RawFood()
    {
        return rawFoods[Random.Range(0, rawFoods.Count)];
    }
    public Food_ScrObj RawFood(int foodID)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodID != rawFoods[i].id) continue;
            return rawFoods[i];
        }
        return null;
    }
    public Food_ScrObj RawFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodScrObj != rawFoods[i]) continue;
            return rawFoods[i];
        }
        return null;
    }

    public bool Is_RawFood(Food_ScrObj foodScrObj)
    {
        if (foodScrObj == null) return false;

        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodScrObj == rawFoods[i]) return true;
        }
        return false;
    }


    // Get Cooked Food
    /// <summary>
    /// Check if State Data Matches (checks if visitor data matches home data)
    /// </summary>
    public bool FoodCondition_DataMatch(List<FoodCondition_Data> visitorData, List<FoodCondition_Data> homeData)
    {
        // state count match check
        if (visitorData.Count != homeData.Count) return false;

        int matchCount = homeData.Count;

        for (int i = 0; i < visitorData.Count; i++)
        {
            for (int j = 0; j < homeData.Count; j++)
            {
                // type check 
                if (visitorData[i].type != homeData[j].type) continue;
                // level check
                if (visitorData[i].level != homeData[j].level) continue;

                // match found !
                matchCount--;
            }
        }

        if (matchCount > 0) return false;
        else return true;
    }


    public Food_ScrObj CookedFood()
    {
        return cookedFoods[Random.Range(0, cookedFoods.Count)];
    }
    public Food_ScrObj CookedFood(int foodID)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodID != cookedFoods[i].id) continue;
            return cookedFoods[i];
        }
        return null;
    }
    public Food_ScrObj CookedFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodScrObj != cookedFoods[i]) continue;
            return cookedFoods[i];
        }
        return null;
    }

    public Food_ScrObj CookedFood(List<FoodData> ingredientDatas)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (!cookedFoods[i].Ingredients_Match(ingredientDatas)) continue;
            return cookedFoods[i];
        }

        return null;
    }


    // Get Position Data
    public List<Vector2> Centered_PositionDatas(Vector2 centerPos, int dataNum)
    {
        dataNum = Mathf.Clamp(dataNum, 0, _positionDatas.Length - 1);

        Vector2[] dataPositions = _positionDatas[dataNum].positionData;
        List<Vector2> centeredPositions = new();

        foreach (Vector2 pos in dataPositions)
        {
            centeredPositions.Add(centerPos + pos);
        }

        return centeredPositions;
    }
}