using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour
{
    private LandData _currentData;
    public LandData currentData => _currentData;


    // Set Functions
    public void Set_CurrentData(LandData setData)
    {
        _currentData = setData;
    }
}