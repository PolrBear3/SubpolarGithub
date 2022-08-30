using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public Weather_ScrObj currentWeather;

    public void Set_Today_Weather()
    {
        var x = controller.timeSystem.currentSeason;
        
        int randomWeatherNum = Random.Range(0, 9);
        currentWeather = x.weatherPercentages[randomWeatherNum];
    }

    // all events for weathers
}
