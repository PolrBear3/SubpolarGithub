using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LandType { plain, dune }

[System.Serializable]
public class LandData
{
    [SerializeField] private LandType _type;
    public LandType type => _type;

    [SerializeField] private int _population;
    public int population => _population;


    // Constructors
    public LandData(LandType type)
    {
        _type = type;
    }
}