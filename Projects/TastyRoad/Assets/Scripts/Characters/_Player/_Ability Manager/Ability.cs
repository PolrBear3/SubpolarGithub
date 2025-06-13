using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    [ES3Serializable][SerializeField] private Ability_ScrObj _abilityScrObj;
    public Ability_ScrObj abilityScrObj => _abilityScrObj;

    [ES3Serializable][SerializeField] private int _activationCount;
    public int activationCount => _activationCount;


    // New
    public Ability(Ability_ScrObj setAbility)
    {
        _abilityScrObj = setAbility;
    }


    // Data Updates
    public bool ActivationCount_Maxed()
    {
        return _activationCount >= _abilityScrObj.Max_ActivationCount();
    }
    
    public void Update_ActivationCount(int updateValue)
    {
        _activationCount += updateValue;
    }
}