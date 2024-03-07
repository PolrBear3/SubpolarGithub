using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weather_Box : MonoBehaviour
{
    public Text dayText;
    public Image weatherImage;
    public Text percentageText;

    public void Update_WeatherBox(int day, Sprite weatherSprite, float percentage)
    {
        dayText.text = "day " + day.ToString();
        weatherImage.sprite = weatherSprite;
        percentageText.text = percentage + "%".ToString();
    }
}
