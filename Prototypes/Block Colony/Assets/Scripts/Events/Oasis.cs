using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oasis : PopulationEvent, ILandEventable
{
    // ILandEventable
    public new void Activate()
    {
        if (ConditionCheck() == false) return;

        // event activation
        SurroundingDesert_PopulationEvent();
    }


    // Check
    private bool ConditionCheck()
    {
        // check if current tile is water
        if (CurrentLand().currentData.type != LandType.water) return false;

        // check if there are 4 surrounding lands
        List<Land> surroundingLands = CurrentLand().main.CrossSurrounding_Lands(CurrentLand());
        if (surroundingLands.Count < 4) return false;

        // check if 4 surrounding lands are desert
        for (int i = 0; i < surroundingLands.Count; i++)
        {
            if (surroundingLands[i].currentData.type != LandType.desert) return false;
        }

        // condition success
        return true;
    }


    // Functions
    private void SurroundingDesert_PopulationEvent()
    {
        List<Land> surroundingLands = CurrentLand().main.CrossSurrounding_Lands(CurrentLand());

        for (int i = 0; i < surroundingLands.Count; i++)
        {
            if (surroundingLands[i].currentData.Has_Event(eventScrObj)) continue;

            surroundingLands[i].currentData.Update_Population(increaseAmount);
            surroundingLands[i].currentData.Update_Event(eventScrObj);
        }
    }
}
