using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Abiliy_ActivationData
{
    [SerializeField] private Sprite _activationIconSprite;
    public Sprite activationIconSprite => _activationIconSprite;
    
    [SerializeField] [Range(0, 100)] private int _maxAbilityPoint;
    public int maxAbilityPoint => _maxAbilityPoint;
}