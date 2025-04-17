using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TimePhase_Population
{
    public TimePhase timePhase;

    [Range(0, 100)] public int maxPopulation;
}

[System.Serializable]
public class LocationData
{
    public Location_ScrObj locationScrObj;

    
    [Header("")]
    [ES3NonSerializable] public Vector2 spawnRangeX;
    [ES3NonSerializable] public Vector2 spawnRangeY;

    [Header("")]
    [ES3NonSerializable] public float spawnIntervalTime;
 
    
    [Header("")]
    [SerializeField][ES3NonSerializable] private SpriteRenderer _screenArea;
    public SpriteRenderer screenArea => _screenArea;

    [SerializeField][ES3NonSerializable] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    
    [Header("")]
    [ES3NonSerializable] public TimePhase_Population[] populationData;

    [SerializeField][ES3NonSerializable] private AnimatorOverrideController[] _npcSkinOverrides;
    public AnimatorOverrideController[] npcSkinOverrides => _npcSkinOverrides;

    [Header("")] 
    [SerializeField][ES3NonSerializable][Range(0, 100)] private int _maxFoodOrderCount;
    public int maxFoodOrderCount => _maxFoodOrderCount;
    
    [ES3NonSerializable] private int _currentFoodOrderCount;
    public int currentFoodOrderCount => _currentFoodOrderCount;
    
    
    [Header("")]
    [SerializeField][ES3NonSerializable] private FoodWeight_Data[] _ingredientUnlocks;
    public FoodWeight_Data[] ingredientUnlocks => _ingredientUnlocks;

    [SerializeField][ES3NonSerializable] private StationWeight_Data[] _stationDrops;
    public StationWeight_Data[] stationDrops => _stationDrops;


    // Constructors
    public LocationData(LocationData data)
    {
        locationScrObj = data.locationScrObj;

        spawnRangeX = data.spawnRangeX;
        spawnRangeY = data.spawnRangeY;

        spawnIntervalTime = data.spawnIntervalTime;

        populationData = data.populationData;
    }


    // Area Datas
    public bool Within_SpawnRange(Vector2 position)
    {
        bool withinX = position.x >= spawnRangeX.x && position.x <= spawnRangeX.y;
        bool withinY = position.y >= spawnRangeY.x && position.y <= spawnRangeY.y;

        return withinX && withinY;
    }


    // Food and Station Datas
    public Food_ScrObj WeightRandom_Food(List<FoodWeight_Data> data)
    {
        if (data.Count <= 0) return null;

        // get total wieght
        float totalWeight = 0;

        foreach (FoodWeight_Data foodData in data)
        {
            totalWeight += foodData.weight;
        }

        // track values
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        // get random according to weight
        for (int i = 0; i < data.Count; i++)
        {
            cumulativeWeight += data[i].weight;

            if (randValue >= cumulativeWeight) continue;
            return data[i].foodScrObj;
        }

        return null;
    }
    public Food_ScrObj WeightRandom_Food()
    {
        List<FoodWeight_Data> ingredientUnlocks = new();

        foreach (FoodWeight_Data data in _ingredientUnlocks)
        {
            ingredientUnlocks.Add(data);
        }

        return WeightRandom_Food(ingredientUnlocks);
    }

    /// <returns>
    /// higher cost data from _ingredientUnlocks Food_ScrObj compare to compareFood
    /// </returns>
    public Food_ScrObj WeightRandom_Food(Food_ScrObj compareFood)
    {
        List<FoodWeight_Data> data = new();

        for (int i = 0; i < _ingredientUnlocks.Length; i++)
        {
            if (compareFood.price < _ingredientUnlocks[i].foodScrObj.price) continue;

            data.Add(_ingredientUnlocks[i]);
        }

        if (data.Count <= 0) return compareFood;

        return WeightRandom_Food(data);
    }


    public Station_ScrObj WeightRandom_Station()
    {
        // get total wieght
        float totalWeight = 0;

        foreach (StationWeight_Data data in _stationDrops)
        {
            totalWeight += data.weight;
        }

        // track values
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        // get random according to weight
        for (int i = 0; i < _stationDrops.Length; i++)
        {
            cumulativeWeight += _stationDrops[i].weight;

            if (randValue >= cumulativeWeight) continue;
            return _stationDrops[i].stationScrObj;
        }

        return null;
    }


    // Time Phase Max Spawn Data
    public TimePhase_Population Max_PopulationData(TimePhase timePhase)
    {
        for (int i = 0; i < populationData.Length; i++)
        {
            if (timePhase != populationData[i].timePhase) continue;

            return populationData[i];
        }

        return populationData[0];
    }
    
    
    // NPC
    public void SetCurrent_FoodOrderCount(int setValue)
    {
        _currentFoodOrderCount = setValue;
    }
}