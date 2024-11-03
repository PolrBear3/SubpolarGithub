using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    [SerializeField][ES3Serializable] private Ability_ScrObj _abilityScrObj;
    public Ability_ScrObj abilityScrObj => _abilityScrObj;

    [SerializeField][ES3Serializable] private int _activationCount;
    public int activationCount => _activationCount;


    // new Constructors
    public Ability(Ability_ScrObj setAbility)
    {
        _abilityScrObj = setAbility;
        _activationCount = 1;
    }


    // Data Updates
    public void Update_ActivationCount(int updateValue)
    {
        _activationCount += updateValue;
    }
}