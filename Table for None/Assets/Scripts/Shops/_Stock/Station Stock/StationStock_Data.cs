using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationStock_Data
{
    [ES3Serializable] private StationData _stationData;
    public StationData stationData => _stationData;
    
    [ES3Serializable] private StockData _discountData;
    public StockData discountData => _discountData;
    
    
    // New
    public StationStock_Data(bool isDiscount)
    {
        _discountData = new StockData(isDiscount);
    }
    
    
    // Get
    public bool Station_Sold()
    {
        if (_stationData == null) return true;
        if (_stationData.stationScrObj != null) return false;
        
        _stationData = null;
        return true;
    }
    
    
    // Data
    public void Set_StationData(StationData stationData)
    {
        if (stationData == null || stationData.stationScrObj == null)
        {
            _stationData = null;
            return;
        }
        
        _stationData = new(stationData);
    }
}
