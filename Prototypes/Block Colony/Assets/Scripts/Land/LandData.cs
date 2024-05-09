using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LandType { plain, water, desert, mountain }

[System.Serializable]
public class LandData
{
    [SerializeField] private Land_SnapPoint _snapPoint;
    public Land_SnapPoint snapPoint => _snapPoint;

    [SerializeField] private LandType _type;
    public LandType type => _type;

    [SerializeField] private int _population;
    public int population => _population;

    [SerializeField] private int _bonusPopulation;
    public int bonusPopulation => _bonusPopulation;

    [SerializeField] private List<EventScrObj> _currentEvents = new();
    public List<EventScrObj> currentEvents => _currentEvents;


    // Constructors
    public LandData(Land_SnapPoint setSnapPoint, LandType type)
    {
        _snapPoint = setSnapPoint;
        _type = type;

        _population = 1;
    }


    // Population Functions
    public void Update_Population(int updateAmount)
    {
        _population += updateAmount;
    }

    public void Update_BonusPopulation(int updateAmount)
    {
        _population -= _bonusPopulation;
        _bonusPopulation = 0;
        _bonusPopulation += updateAmount;

        Update_Population(_bonusPopulation);
    }


    // Current Event Functions
    public bool Has_Event(EventScrObj findEvent)
    {
        for (int i = 0; i < _currentEvents.Count; i++)
        {
            if (findEvent != _currentEvents[i]) continue;
            return true;
        }
        return false;
    }

    public int Event_Count(EventScrObj findEvent)
    {
        int findCount = 0;

        for (int i = 0; i < _currentEvents.Count; i++)
        {
            if (findEvent != _currentEvents[i]) continue;
            findCount++;
        }
        return findCount;
    }


    public void Update_Event(EventScrObj updateEvent)
    {
        _currentEvents.Add(updateEvent);
    }

    public void Clear_Events()
    {
        _currentEvents.Clear();
    }
}