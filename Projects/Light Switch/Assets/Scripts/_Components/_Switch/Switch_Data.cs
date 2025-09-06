using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Switch_Data
{
    private Switch_SrcObj _switchSrcObj;
    public Switch_SrcObj switchSrcObj => _switchSrcObj;
    
    
    // New
    public Switch_Data(Switch_SrcObj switchSrcObj)
    {
        _switchSrcObj = switchSrcObj;
    }
    
    
    // Data
}
