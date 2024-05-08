using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationEvent : MonoBehaviour, ILandEventable
{
    [SerializeField] private EventScrObj _eventScrObj;
    public EventScrObj eventScrObj => _eventScrObj;

    [Header("current population increase amount data")]
    [SerializeField] private int _increaseAmount;
    public int increaseAmount => _increaseAmount;


    // ILandEventable
    public void Activate()
    {
        Increase_Population(_increaseAmount);
    }


    //
    public Land CurrentLand()
    {
        LandEvents eventController = transform.parent.GetComponent<LandEvents>();
        Land currentLand = eventController.land;

        return currentLand;
    }

    public void Increase_Population(int increaseAmount)
    {
        CurrentLand().currentData.Update_Population(increaseAmount);
        CurrentLand().main.Update_UpdatePopulation();
    }
}
