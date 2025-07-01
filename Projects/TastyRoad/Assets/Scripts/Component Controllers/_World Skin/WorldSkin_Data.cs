using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSkin_Data
{
    [SerializeField] private Sprite[] _skinSprites;
    public Sprite[] skinSprites => _skinSprites;
}