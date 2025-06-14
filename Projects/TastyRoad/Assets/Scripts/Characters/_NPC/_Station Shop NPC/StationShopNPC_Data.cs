using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationShopNPC_Data
{
    [ES3Serializable] private List<StationData> _archiveDatas = new();
    public List<StationData> archiveDatas => _archiveDatas;
    
    [ES3Serializable] private bool _scrapCollected;
    public bool scrapCollected => _scrapCollected;
    
    [ES3Serializable] private Station_ScrObj _buildStation;
    public Station_ScrObj buildStation => _buildStation;


    // New
    public StationShopNPC_Data(List<StationData> archiveDatas)
    {
        _archiveDatas = archiveDatas;
    }
    
    
    // Data Control
    public void Reset_ActionState()
    {
        _scrapCollected = false;
        _buildStation = null;
    }
    
    public void Collect_Scrap()
    {
        _scrapCollected = true;
    }
    
    public void Build_Station(Station_ScrObj buildStation)
    {
        _buildStation = buildStation;
    }
}
