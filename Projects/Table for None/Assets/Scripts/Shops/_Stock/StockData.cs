using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockData
{
    [SerializeField][ES3Serializable] private bool _unlocked;
    public bool unlocked => _unlocked;

    [SerializeField][ES3Serializable] private bool _isDiscount;
    public bool isDiscount => _isDiscount;
    
    [SerializeField][ES3Serializable] private bool _isComplete;
    public bool isComplete => _isComplete;


    // Constructors
    public StockData(StockData data)
    {
        if (data == null) return;

        _unlocked = data.unlocked;
        _isDiscount = data.isDiscount;
        _isComplete = data.isComplete;
    }

    public StockData(bool unlockToggle)
    {
        _unlocked = unlockToggle;
    }


    // Functions
    public void Toggle_UnLock(bool toggle)
    {
        _unlocked = toggle;
    }

    public void Toggle_Discount(bool toggle)
    {
        _isDiscount = toggle;
    }

    public void Toggle_Complete(bool toggle)
    {
        _isComplete = toggle;
    }
}
