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
        // days

        // weather image

        // percentage
    }
}
