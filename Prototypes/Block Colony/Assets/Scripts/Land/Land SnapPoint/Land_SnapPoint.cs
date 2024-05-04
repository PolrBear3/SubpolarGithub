using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land_SnapPoint : MonoBehaviour
{
    private SnapPointData _currentData;
    public SnapPointData currentData => _currentData;


    // Event Trigger component
    public void OnPointerClick()
    {
        
    }


    // Set Functions
    public void Set_CurrentData(SnapPointData setData)
    {
        _currentData = setData;
    }
}
