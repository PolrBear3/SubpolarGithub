using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilized : PopulationEvent, ILandEventable
{
    [Header("")]
    [SerializeField] private EventScrObj _natureLove;
    [SerializeField] private EventScrObj _house;
    [SerializeField] private EventScrObj _building;

    [Header("")]
    [SerializeField] private int _houseIncreaseAmount;
    [SerializeField] private int _buildingIncreaseAmount;


    // ILandEventable
    public void Activate()
    {
        Construct_Building();
        Construct_House();

        Structure_PopulationIncrease();
    }


    // Event Functions
    private void Construct_House()
    {
        int constructorsCount = CurrentLand().currentData.Event_Count(eventScrObj);
        int availableBuildCount = CurrentLand().currentData.Event_Count(_natureLove) / 2;

        for (int i = 0; i < availableBuildCount; i++)
        {
            if (constructorsCount <= 0) return;
            constructorsCount--;

            // construct house
            CurrentLand().currentData.Update_Event(_house);

            // remove 2 trees
            for (int j = 0; j < 2; j++)
            {
                CurrentLand().currentData.Clear_Event(_natureLove);
                CurrentLand().events.Update_RemovedEvents_Animation();
            }

            // remove constructor
            CurrentLand().currentData.Clear_Event(eventScrObj);
        }
    }

    private void Construct_Building()
    {
        int constructorsCount = CurrentLand().currentData.Event_Count(eventScrObj);
        int availableBuildCount = CurrentLand().currentData.Event_Count(_house) / 2;

        for (int i = 0; i < availableBuildCount; i++)
        {
            if (constructorsCount <= 0) return;
            constructorsCount--;

            // construct building
            CurrentLand().currentData.Update_Event(_building);

            // remove 2 house
            for (int j = 0; j < 2; j++)
            {
                CurrentLand().currentData.Clear_Event(_house);
                CurrentLand().events.Update_RemovedEvents_Animation();
            }

            // remove constructor
            CurrentLand().currentData.Clear_Event(eventScrObj);
        }
    }

    private void Structure_PopulationIncrease()
    {
        int houseCount = CurrentLand().currentData.Event_Count(_house);
        int buildingCount = CurrentLand().currentData.Event_Count(_building);

        CurrentLand().currentData.Update_BonusPopulation(_houseIncreaseAmount * houseCount);
        CurrentLand().currentData.Update_BonusPopulation(_buildingIncreaseAmount * buildingCount);
    }
}
