using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityManager_Data
{
    [ES3Serializable] private List<Ability> _abilityDatas = new();
    public List<Ability> abilityDatas => _abilityDatas;
    
    [ES3Serializable] private int _abilityPoint;
    public int abilityPoint => _abilityPoint;
    
    
    // New
    public AbilityManager_Data(int abilityPoint)
    {
        _abilityPoint = abilityPoint;
    }
    
    
    // Data Control
    public void Set_AbilityPoint(int abilityPoint)
    {
        _abilityPoint = abilityPoint;
    }
    
    
    // _abilityDatas
    public Ability AbilityData(Ability_ScrObj targetAbility)
    {
        for (int i = 0; i < _abilityDatas.Count; i++)
        {
            if (targetAbility != _abilityDatas[i].abilityScrObj) continue;
            return _abilityDatas[i];
        }
        return null;
    }

    public int Ability_ActivationCount(Ability_ScrObj searchAbility)
    {
        Ability searchAbilityData = AbilityData(searchAbility);
        if (searchAbilityData == null) return 0;
        
        return searchAbilityData.activationCount;
    }
}
