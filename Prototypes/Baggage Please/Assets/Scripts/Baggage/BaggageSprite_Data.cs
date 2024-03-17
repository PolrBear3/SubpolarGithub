using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaggageSprite_Data
{
    [SerializeField] private List<Sprite> _bagSprites = new();
    public List<Sprite> bagSprites => _bagSprites;
}
