using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMountain : PopulationEvent, ILandEventable
{
    // ILandEventable
    public void Activate()
    {
        if (ConditionCheck() == false) return;

        Activate_Event();
    }


    // Check
    private bool ConditionCheck()
    {
        // check if already has this event
        if (CurrentLand().currentData.Has_Event(eventScrObj)) return false;

        // check if current tile is mountain
        if (CurrentLand().currentData.type != LandType.mountain) return false;

        MainController main = CurrentLand().main;

        // check if there are mountains on the side
        List<Vector2> sideLandPositions = new();

        sideLandPositions.Add(new Vector2(-1f, 0f));
        sideLandPositions.Add(new Vector2(1f, 0f));

        List<Land> sideLands = main.OffSet_Lands(CurrentLand(), sideLandPositions);
        if (sideLands.Count < 2) return false;

        foreach (var land in sideLands)
        {
            if (land.currentData.type != LandType.mountain) return false;
        }

        return true;
    }


    // Functions
    private void Activate_Event()
    {
        MainController main = CurrentLand().main;

        List<Vector2> sideLandPositions = new();

        sideLandPositions.Add(new Vector2(-1f, 0f));
        sideLandPositions.Add(new Vector2(1f, 0f));

        List<Land> sideLands = main.OffSet_Lands(CurrentLand(), sideLandPositions);

        // remove side lands
        foreach (var land in sideLands)
        {
            land.Remove_CurrentLand();
        }

        //
        CurrentLand().currentData.Update_Population(increaseAmount);
        CurrentLand().currentData.Update_Event(eventScrObj);
    }
}
