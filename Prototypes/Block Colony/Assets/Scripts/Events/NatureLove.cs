using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureLove : PopulationEvent, ILandEventable
{
    // ILandEventable
    public void Activate()
    {
        if (TreeCount() <= 0) return;

        // event activation
        CurrentLand().currentData.Update_BonusPopulation(TreeCount());
    }


    // Increase Amount Calculation
    private int TreeCount()
    {
        return CurrentLand().currentData.Event_Count(eventScrObj);
    }
}