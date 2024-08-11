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
    public Location_ScrObj locationScrObj;

    [Header("")]
    public Vector2 spawnRangeX;
    public Vector2 spawnRangeY;

    [Header("")]
    public Vector2 spawnIntervalTimeRange;

    [Header("")]
    public List<MaxSpawn_TimePoint> maxSpawnTimePoints;

    [SerializeField] private AnimatorOverrideController[] _npcSkinOverrides;
    public AnimatorOverrideController[] npcSkinOverrides => _npcSkinOverrides;


    public LocationData()
    {

    }

    public LocationData (LocationData data)
    {
        locationScrObj = data.locationScrObj;

        spawnRangeX = data.spawnRangeX;
        spawnRangeY = data.spawnRangeY;

        spawnIntervalTimeRange = data.spawnIntervalTimeRange;

        maxSpawnTimePoints = data.maxSpawnTimePoints;
    }
}