using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalEvent : PopulationEvent, ILandEventable
{
    [Header("")]
    [SerializeField] private LandType[] _discludeLands;

    [SerializeField] private float _activateChanceRate;


    // ILandEventable
    public void Activate()
    {
        if (ConditionCheck() == false) return;

        Increase_Population(increaseAmount);
        CurrentLand().currentData.Update_Event(eventScrObj);
    }


    //
    private bool ConditionCheck()
    {
        // if already activated last turn
        if (CurrentLand().currentData.Has_Event(eventScrObj))
        {
            CurrentLand().currentData.Clear_Events(eventScrObj);
            return false;
        }

        // if restricted land for activation
        for (int i = 0; i < _discludeLands.Length; i++)
        {
            if (CurrentLand().currentData.type == _discludeLands[i]) return false;
        }

        // activate percentage rate


        return true;
    }
}
