using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureLove : PopulationEvent, ILandEventable
{
    // ILandEventable
    public new void Activate()
    {
        // event activation
        Increase_BonusPopulaiton(TreeCount());
    }


    // Increase Amount Calculation
    private int TreeCount()
    {
        return CurrentLand().currentData.Event_Count(eventScrObj);
    }
}