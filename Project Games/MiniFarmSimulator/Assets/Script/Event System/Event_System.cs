using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    void Activate_Event();
}
public interface IEventResetable
{
    void Reset_Event();
}

[System.Serializable]
public struct Event_Data
{
    public float percentage;
    public int health;
    public int dayPassed;
    public int fullGrownDay;
    public int watered;
    public int bonusPoints;

    [HideInInspector]
    public bool activated;
}

[System.Serializable]
public class Event_System_Data
{
    public Weather_ScrObj currentWeather;
}

public class Event_System : MonoBehaviour
{
    public MainGame_Controller controller;
    public WeatherEstimation_System weatherSystem;
    public GameObject events;

    public Event_System_Data data;
    private List<GameObject> allEvents = new List<GameObject>();

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
    public Weather_ScrObj Set_Weather()
    {
        Season_ScrObj currentSeason = controller.timeSystem.currentSeason;
        Weather_ScrObj chosenWeather = null;

        // check for weather percentage under 0
        for (int i = 0; i < currentSeason.weathers.Length; i++)
        {
            if (currentSeason.weathers[i].percentage >= 1) continue;

            chosenWeather = currentSeason.weathers[i].weather;
            break;
        }

        // set weather
        while (chosenWeather == null)
        {
            for (int i = 0; i < currentSeason.weathers.Length; i++)
            {
                if (!Percentage_Setter(currentSeason.weathers[i].percentage)) continue;
                
                chosenWeather = currentSeason.weathers[i].weather;
                break;
            }
        }

        return chosenWeather;
    }

    // overall check update
    public void Set_Today_Weather()
    {
        Season_ScrObj currentSeason = controller.timeSystem.currentSeason;
        var currentWeatherData = weatherSystem.estimateWeathers[0];

        // weather news correct
        if (currentSeason == currentWeatherData.season && Percentage_Setter(currentWeatherData.percentage))
        {
            data.currentWeather = weatherSystem.estimateWeathers[0].weather;
        }
        // weather news incorrect
        else
        {
            data.currentWeather = Set_Weather();
        }
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
    public void Reset_All_Events()
    {
        for (int i = 0; i < allEvents.Count; i++)
        {
            if (allEvents[i].TryGetComponent(out IEventResetable e))
            {
                e.Reset_Event();
            }
        }
    }
}