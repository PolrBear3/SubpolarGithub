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
    [ES3NonSerializable] public Vector2 spawnRangeX;
    [ES3NonSerializable] public Vector2 spawnRangeY;

    [Header("")]
    [ES3NonSerializable] public Vector2 spawnIntervalTimeRange;

    [Header("")]
    [SerializeField][ES3NonSerializable] private SpriteRenderer _screenArea;
    public SpriteRenderer screenArea => _screenArea;

    [SerializeField][ES3NonSerializable] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    [Header("")]
    [ES3NonSerializable] public List<MaxSpawn_TimePoint> maxSpawnTimePoints;

    [SerializeField][ES3NonSerializable] private AnimatorOverrideController[] _npcSkinOverrides;
    public AnimatorOverrideController[] npcSkinOverrides => _npcSkinOverrides;

    [Header("")]
    [SerializeField][ES3NonSerializable] private StationWeight_Data[] _stationDrops;
    public StationWeight_Data[] stationDrops => _stationDrops;


    // Constructors
    public LocationData(LocationData data)
    {
        locationScrObj = data.locationScrObj;

        spawnRangeX = data.spawnRangeX;
        spawnRangeY = data.spawnRangeY;

        spawnIntervalTimeRange = data.spawnIntervalTimeRange;

        maxSpawnTimePoints = data.maxSpawnTimePoints;
    }


    // Data
    public Station_ScrObj WeightRandom_Station()
    {
        // get total wieght
        float totalWeight = 0;

        foreach (StationWeight_Data data in _stationDrops)
        {
            totalWeight += data.weight;
        }

        // track values
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        // get random according to weight
        for (int i = 0; i < _stationDrops.Length; i++)
        {
            cumulativeWeight += _stationDrops[i].weight;

            if (randValue >= cumulativeWeight) continue;
            return _stationDrops[i].stationScrObj;
        }

        return null;
    }
}