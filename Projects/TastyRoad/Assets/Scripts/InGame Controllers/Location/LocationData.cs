using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationData
{
    public int worldNum;
    public int locationNum;

    public LocationData (int worldNum, int locationNum)
    {
        this.worldNum = worldNum;
        this.locationNum = locationNum;
    }

    public LocationData (LocationData data)
    {
        worldNum = data.worldNum;
        locationNum = data.locationNum;
    }
}
