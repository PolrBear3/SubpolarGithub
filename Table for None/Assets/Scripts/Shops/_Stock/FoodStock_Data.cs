using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodStock_Data
{
    [ES3Serializable] private StockData _stockData;
    public StockData stockData => _stockData;
    
    [ES3Serializable] private PurchaseData _stockPurchaseData;
    public PurchaseData stockPurchaseData => _stockPurchaseData;

    [ES3Serializable] private List<FoodData> _stockedFoodDatas = new();
    public List<FoodData> stockedFoodDatas => _stockedFoodDatas;
    
    
    // New
    public FoodStock_Data(StockData data)
    {
        _stockData = new(data);
    }
    
    
    // Get
    public bool Stock_Purchased()
    {
        if (_stockData.unlocked == false) return false;
        return _stockPurchaseData != null && _stockPurchaseData.purchased;
    }

    public FoodData Recent_StockedFood()
    {
        if (_stockedFoodDatas.Count <= 0) return null;
        return _stockedFoodDatas[0];
    }
    
    
    // Current Data
    public void Set_StockData(StockData data)
    {
        if (data == null) return;

        _stockData = new(data);
    }

    public void Set_PurchaseData(PurchaseData data)
    {
        if (data == null)
        {
            _stockPurchaseData = new(false);
            return;
        }
        
        _stockPurchaseData = new(data.price);
        _stockPurchaseData.Toggle_PurchaseState(data.purchased);
    }

    public void Set_StockedFoodData(List<FoodData> datas)
    {
        if (datas == null)
        {
            _stockedFoodDatas = null;
            return;
        }
        _stockedFoodDatas = datas;
    }
    public void Set_StockedFoodData(FoodData data)
    {
        if (data == null) return;
        
        _stockedFoodDatas.Add(data);
    }
}
