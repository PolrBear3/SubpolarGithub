using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationData
{
    [SerializeField][ES3Serializable] private Station_ScrObj _stationScrObj;
    public Station_ScrObj stationScrObj => _stationScrObj;

    [SerializeField][ES3Serializable] private Vector2 _position;
    public Vector2 position => _position;

    [SerializeField][ES3Serializable] private int _durability;
    public int durability => _durability;


    // Constructors
    public StationData(StationData data)
    {
        if (data == null) return;

        _stationScrObj = data.stationScrObj;
        _position = data.position;
        _durability = data.durability;
    }

    public StationData(Station_ScrObj stationScrObj)
    {
        _stationScrObj = stationScrObj;
        _durability = stationScrObj.durability;
    }

    public StationData(Station_ScrObj station, Vector2 position)
    {
        _stationScrObj = station;
        _position = position;
        _durability = station.durability;
    }


    // Functions
    public void Update_Position(Vector2 updatePosition)
    {
        _position = updatePosition;
    }

    public void Update_Durability(int updateValue)
    {
        _durability += updateValue;
    }
}