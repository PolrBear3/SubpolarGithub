using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Baggage_Data
{
    [SerializeField] private int _typeNum;
    public int typeNum => _typeNum;

    [SerializeField] private int _heatLevel;
    public int heatLevel => _heatLevel;

    [SerializeField] private float _detectChance;
    public float detectChance => _detectChance;

    public Baggage_Data(int typeNum, float detectChance)
    {
        _typeNum = typeNum;
        _heatLevel = Random.Range(0, 4);
        _detectChance = detectChance;
    }
}
