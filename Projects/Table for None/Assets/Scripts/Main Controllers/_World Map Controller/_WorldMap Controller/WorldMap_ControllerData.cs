using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldMap_ControllerData
{
    [ES3Serializable] private WorldMap_Data _currentData;
    public WorldMap_Data currentData => _currentData;
    
    [ES3Serializable] private HashSet<WorldMap_Data> _visitedDatas = new();
    public HashSet<WorldMap_Data> visitedDatas => _visitedDatas;
    
    [ES3Serializable] private bool _newLocation;
    public bool newLocation => _newLocation;
    
    
    // New
    public WorldMap_ControllerData(WorldMap_Data newData)
    {
        _currentData = newData;
    }
    
    
    // Gets
    public bool CurrentLocation_Visited()
    {
        return _visitedDatas.Contains(_currentData);
    }
    
    
    // Datas
    public void Set_WorldMapData(WorldMap_Data setData)
    {
        _currentData = setData;
    }

    public void Toggle_NewLocation(bool toggle)
    {
        _newLocation = toggle;
    }
}