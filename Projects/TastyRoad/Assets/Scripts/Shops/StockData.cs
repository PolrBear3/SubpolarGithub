using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockData
{
    private bool _unlocked;
    public bool unlocked => _unlocked;

    private bool _isDiscount;
    public bool isDiscount => _isDiscount;


    // New Sets
    public StockData()
    {

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
}
