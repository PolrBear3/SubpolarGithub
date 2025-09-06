using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Data
{
    private int _totalToggleCount;
    public int totalToggleCount => _totalToggleCount;
    
    
    // Data
    public void Set_TotalToggleCount(int setValue)
    {
        _totalToggleCount = Mathf.Max(0, setValue);
    }
}
