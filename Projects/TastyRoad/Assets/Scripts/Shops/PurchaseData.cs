using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseData
{
    [ES3Serializable] private int _price;
    public int price => _price;

    [ES3Serializable] private bool _purchased;
    public bool purchased => _purchased;
    
    
    // new
    public PurchaseData(int price)
    {
        _price = price;
        _purchased = false;
    }
    
    
    // Data Control
    public void Toggle_PurchaseState(bool toggle)
    {
        _purchased = toggle;
    }
}