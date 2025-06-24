using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spike_Data
{
    private bool _hasTail;
    public bool hasTail => _hasTail;

    private int _damageCount;
    public int damageCount => _damageCount;
    
    
    // New
    public Spike_Data(int maxDamageCount)
    {
        _hasTail = true;
        _damageCount = maxDamageCount;
    }
    
    
    // Data Control
    public void Toggle_HasTail(bool toggle)
    {
        _hasTail = toggle;
    }

    public void Set_DamageCount(int setValue)
    {
        _damageCount = setValue;
    }
}
