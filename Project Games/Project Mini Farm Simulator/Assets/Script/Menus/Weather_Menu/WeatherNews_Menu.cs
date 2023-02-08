using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeatherNews_Menu_UI
{
    public RectTransform panel;
    public Animator icon;
}

public class WeatherNews_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public WeatherEstimation_System system;

    public WeatherNews_Menu_UI ui;
    public Weather_Box[] weatherBoxes;

    [HideInInspector]
    public bool newWeatherChecked;

    public void Open()
    {
        // icon blink inactive
        Set_NewsIcon_Blink(true);

        // update weather news
        Update_WeatherNews();

        // leantween open
        LeanTween.move(ui.panel, new Vector2(0f, 104.85f), 0.75f).setEase(LeanTweenType.easeInOutQuint);
    }
    public void Close()
    {
        // leantween close
        LeanTween.move(ui.panel, new Vector2(500f, 104.85f), 0.75f).setEase(LeanTweenType.easeInOutQuint);
    }

    public void Set_NewsIcon_Blink(bool weatherChecked)
    {
        if (weatherChecked)
        {
            newWeatherChecked = true;
            ui.icon.SetBool("newWeather", false);
        }
        else
        {
            newWeatherChecked = false;
            ui.icon.SetBool("newWeather", true);
        }
    }
    
    private void Update_WeatherNews()
    {
        var weatherEst = system.estimateWeathers;
        int currentDay = controller.timeSystem.currentInGameDay;
        
        for (int i = 0; i < weatherBoxes.Length; i++)
        {
            int dayCount = i + 1;

            if (dayCount >= 7) break;

            weatherBoxes[i].Update_WeatherBox(currentDay + dayCount, weatherEst[dayCount].weather.fadeBackgroundUI, weatherEst[dayCount].percentage);
        }
    }
}
