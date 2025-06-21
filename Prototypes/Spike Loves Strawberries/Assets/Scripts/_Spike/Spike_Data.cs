using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Data
{
    private bool _hasTail;
    public bool hasTail => _hasTail;
    
    
    // New
    public Spike_Data()
    {
        _hasTail = true;
    }
}
