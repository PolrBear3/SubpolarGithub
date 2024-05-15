using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMountain : PopulationEvent, ILandEventable
{
    [SerializeField] private int _sideIncreaseAmount;

    // ILandEventable
    public void Activate()
    {
        Activate_SideEvent();

        if (ConditionCheck() == false) return;

        Activate_Event();
    }


    // Check and Get
    private bool HasEvent()
    {
        // check if already has this event
        if (CurrentLand().currentData.Has_Event(eventScrObj)) return true;
        else return false;
    }

    private bool ConditionCheck()
    {
        // check if already has this event
        if (HasEvent()) return false;

        // check if current tile is mountain
        if (CurrentLand().currentData.type != LandType.mountain) return false;

        if (SideLands().Count < 2) return false;

        foreach (var land in SideLands())
        {
            if (land.currentData.type != LandType.mountain) return false;
            if (land.currentData.Has_Event(eventScrObj)) return false;
        }

        return true;
    }

    private List<Land> SideLands()
    {
        MainController main = CurrentLand().main;

        List<Vector2> sideLandPositions = new();

        sideLandPositions.Add(new Vector2(-1f, 0f));
        sideLandPositions.Add(new Vector2(1f, 0f));

        List<Land> sideLands = main.OffSet_Lands(CurrentLand(), sideLandPositions);

        return sideLands;
    }


    // Functions
    private void Activate_Event()
    {
        // remove side lands
        foreach (var land in SideLands())
        {
            land.Remove_CurrentLand();
        }

        //
        CurrentLand().currentData.Update_Population(increaseAmount);
        CurrentLand().currentData.Update_Event(eventScrObj);
    }

    private void Activate_SideEvent()
    {
        if (HasEvent() == false) return;

        foreach (var land in SideLands())
        {
            if (land.currentData.type != LandType.mountain) continue;

            land.currentData.Update_BonusPopulation(_sideIncreaseAmount);
        }
    }
}