using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct weatherData
{
    public float percentage;
    public Weather_ScrObj weather;
    public Season_ScrObj season;
}

public class WeatherEstimation_System : MonoBehaviour
{
    [SerializeField] private MainGame_Controller controller;

    public List<weatherData> estimateWeathers = new List<weatherData>();

    public void StartSet_Estimate_Weathers()
    {
        var currentSeason = controller.timeSystem.currentSeason;

        for (int i = 0; i < 6; i++)
        {
            weatherData weatherData;

            float randomPercentageNum = Random.Range(10, 100);
            weatherData.percentage = randomPercentageNum;

            weatherData.weather = controller.eventSystem.Set_Weather();

            weatherData.season = currentSeason;

            estimateWeathers.Add(weatherData);
        }
    }
    public void Next_Estimate_Weathers()
    {
        estimateWeathers.RemoveAt(0);

        var currentSeason = controller.timeSystem.currentSeason;

        weatherData weatherData;
        weatherData.weather = controller.eventSystem.Set_Weather();

        float randomPercentageNum = Random.Range(10, 100);
        weatherData.percentage = randomPercentageNum;

        weatherData.season = currentSeason;

        estimateWeathers.Add(weatherData);
    }
}
