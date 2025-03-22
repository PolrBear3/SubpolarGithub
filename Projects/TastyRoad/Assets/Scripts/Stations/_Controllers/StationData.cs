using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StationWeight_Data
{
    public Station_ScrObj stationScrObj;
    [Range(0, 100)] public int weight;

    public StationWeight_Data(Station_ScrObj stationScrObj, int weight)
    {
        this.stationScrObj = stationScrObj;
        this.weight = weight;
    }
}

[System.Serializable]
public class StationData
{
    [SerializeField][ES3Serializable] private Station_ScrObj _stationScrObj;
    public Station_ScrObj stationScrObj => _stationScrObj;

    [SerializeField][ES3Serializable] private Vector2 _position;
    public Vector2 position => _position;

    [SerializeField][ES3Serializable] private int _durability;
    public int durability => _durability;

    [SerializeField][ES3Serializable] private int _amount;
    public int amount => _amount;


    // Constructors
    public StationData(StationData data)
    {
        if (data == null) return;

        _stationScrObj = data.stationScrObj;
        _position = data.position;
        _durability = data.durability;
        _amount = data.amount;
    }

    public StationData(Station_ScrObj station)
    {
        _stationScrObj = station;
        _durability = station.durability;
    }

    public StationData(Station_ScrObj station, Vector2 position)
    {
        _stationScrObj = station;
        _position = position;
        _durability = station.durability;
    }

    public StationData(Station_ScrObj station, int amount)
    {
        _stationScrObj = station;
        _amount = amount;
    }


    // Functions
    public void Update_Position(Vector2 updatePosition)
    {
        _position = updatePosition;
    }


    public void Set_Durability(int setValue)
    {
        _durability = setValue;
    }

    public void Update_Durability(int updateValue)
    {
        _durability += updateValue;
    }


    // Amount Control
    public void Set_Amount(int setValue)
    {
        _amount = setValue;
    }

    public void Update_Amount(int updateValue)
    {
        _amount += updateValue;
    }
}