using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaxSpawn_TimePoint
{
    [Range(0, 12)]
    public int timePoint;
    public int maxSpawnAmount;
}

[System.Serializable]
public class LocationData
{
    public int worldNum;
    public int locationNum;

    [Header("")]
    public Vector2 spawnRangeX;
    public Vector2 spawnRangeY;

    [Header("")]
    public Vector2 spawnIntervalTimeRange;

    [Header("")]
    public List<MaxSpawn_TimePoint> maxSpawnTimePoints;

    public LocationData (LocationData data)
    {
        worldNum = data.worldNum;
    }
}
