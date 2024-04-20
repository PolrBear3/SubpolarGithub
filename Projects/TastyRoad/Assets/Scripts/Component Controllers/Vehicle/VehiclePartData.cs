using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehiclePartData
{
    [SerializeField] private VehiclePart_ScrObj _partScrObj;
    public VehiclePart_ScrObj partScrObj => _partScrObj;

    [SerializeField] private bool _unlocked;
    public bool unlocked => _unlocked;

    public VehiclePartData (VehiclePartData data)
    {
        _partScrObj = data._partScrObj;
        _unlocked = data._unlocked;
    }
}