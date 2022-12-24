using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    void Activate_Event();
}

[System.Serializable]
public struct Event_Amount
{
    public float percentage;
    public int health;
    public int dayPassed;
    public int fullGrownDay;
    public int watered;
    public int bonusPoints;
}

public class Event_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public GameObject events;
    private List<GameObject> allEvents = new List<GameObject>();

    public Weather_ScrObj currentWeather;

    private void Start()
    {
        Set_All_Events();
    }

    // tool functions
    public bool Percentage_Setter(float percentage)
    {
        var value = (100 - percentage) * 0.01f;
        if (Random.value > value)
        {
            return true;
        }
        else return false;
    }

    // overall check update
    public void Set_Today_Weather()
    {
        var x = controller.timeSystem.currentSeason;

        int randomWeatherNum = Random.Range(0, 9);
        currentWeather = x.weatherPercentages[randomWeatherNum];
    }

    private void Set_All_Events()
    {
        for (int i = 0; i < events.transform.childCount; i++)
        {
            allEvents.Add(events.transform.GetChild(i).gameObject);
        }
    }
    public void Activate_All_Events()
    {
        for (int i = 0; i < allEvents.Count; i++)
        {
            if (allEvents[i].TryGetComponent(out IEvent e))
            {
                e.Activate_Event();
            }
        }
    }
}