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

            int randomWeatherNum = Random.Range(0, 9);
            //weatherData.weather = currentSeason.weatherPercentages[randomWeatherNum];

            weatherData.season = currentSeason;

            //estimateWeathers.Add(weatherData);
        }
    }
    public void Next_Estimate_Weathers()
    {
        estimateWeathers.RemoveAt(0);

        var currentSeason = controller.timeSystem.currentSeason;

        weatherData weatherData;

        int randomWeatherNum = Random.Range(0, 9);
        //weatherData.weather = currentSeason.weatherPercentages[randomWeatherNum];

        float randomPercentageNum = Random.Range(10, 100);
        weatherData.percentage = randomPercentageNum;

        weatherData.season = currentSeason;

        //estimateWeathers.Add(weatherData);
    }
}
