using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationEvent : MonoBehaviour
{
    [SerializeField] private EventScrObj _eventScrObj;
    public EventScrObj eventScrObj => _eventScrObj;

    [Header("current population increase amount data")]
    [SerializeField] private int _increaseAmount;
    public int increaseAmount => _increaseAmount;


    // Get
    public Land CurrentLand()
    {
        LandEvents eventController = transform.parent.GetComponent<LandEvents>();
        Land currentLand = eventController.land;

        return currentLand;
    }


    // Functions
    public void Increase_Population(int increaseAmount)
    {
        CurrentLand().currentData.Update_Population(increaseAmount);
        CurrentLand().main.Update_UpdatePopulation();
    }

    public void Increase_BonusPopulaiton(int increaseAmount)
    {
        CurrentLand().currentData.Update_BonusPopulation(increaseAmount);
        CurrentLand().main.Update_UpdatePopulation();
    }
}
