using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationShopNPC_Data
{
    [ES3Serializable] private List<StationData> _archiveDatas = new();
    public List<StationData> archiveDatas => _archiveDatas;
    
    [ES3Serializable] private List<StationStock_Data> _stationStockDatas = new();
    public List<StationStock_Data> stationStockDatas => _stationStockDatas;
    
    
    [ES3Serializable] private bool _scrapCollected;
    public bool scrapCollected => _scrapCollected;
    
    [ES3Serializable] private Station_ScrObj _buildingStation;
    public Station_ScrObj buildingStation => _buildingStation;


    // New
    public StationShopNPC_Data(List<StationData> archiveDatas)
    {
        _archiveDatas = archiveDatas;
    }
    
    
    // Action Datas
    public void Reset_ActionState()
    {
        _scrapCollected = false;
        _buildingStation = null;
    }
    
    public void Set_ScrapCollectState()
    {
        _scrapCollected = true;
    }
    
    public void Set_BuildingStation(Station_ScrObj buildStation)
    {
        _buildingStation = buildStation;
    }
}
