using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Station_LoadData
{
    [ES3Serializable] private StationData _stationData;
    public StationData stationData => _stationData;

    [ES3Serializable] private List<FoodData> _foodDatas = new();
    public List<FoodData> foodDatas => _foodDatas;
    
    [ES3Serializable] private WorldMap_Data _worldMapData;
    public WorldMap_Data worldMapData => _worldMapData;
    
    
    // New
    public Station_LoadData(StationData stationData, List<FoodData> foodDatas, WorldMap_Data worldMapData)
    {
        _stationData = stationData;
        _foodDatas = foodDatas;
        _worldMapData = worldMapData;
    }
}
