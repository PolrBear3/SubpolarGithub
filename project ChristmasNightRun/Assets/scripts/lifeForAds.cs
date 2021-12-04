using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lifeForAds : MonoBehaviour
{
    //Lives, LifeUpdateTime
    public Text currentLivesText;
    public Text LifeupdateTimeText;
    
    public int maxLives = 5;
    public float lifeReplenishTime = 600f;
    public static int currentLives;
    public double timerForLife;

    public string showLifeTimeInMinutes()
    {
        float timeLeft = lifeReplenishTime - (float)timerForLife;
        int min = Mathf.FloorToInt(timeLeft / 60);
        int sec = Mathf.FloorToInt(timeLeft % 60);
        return min + ":" + sec.ToString("00");
    }

    void Update()
    {
        if (currentLives < maxLives)
        {
            timerForLife += Time.deltaTime;

            if (timerForLife > lifeReplenishTime)
            {
                currentLives++;
            }
        }

        currentLivesText.text = ((int)currentLives).ToString();
        LifeupdateTimeText.text = showLifeTimeInMinutes();
    }
     
    void UpdateLives(double timerToAdd)
    {
        if (currentLives < maxLives)
        {
            int livesToAdd = Mathf.FloorToInt((float)timerToAdd / lifeReplenishTime);
            timerForLife = (float)timerToAdd % lifeReplenishTime;
            currentLives += livesToAdd;
            if (currentLives > maxLives)
            {
                currentLives = maxLives;
                timerForLife = 0;
            }
        }
        PlayerPrefs.SetString("LifeUpdateTime", System.DateTime.Now.ToString());
    }

    System.DateTime timeOfPause;
    
    void OnAppplicationPause(bool isPause)
    {
        if (isPause)
        {
            timeOfPause = System.DateTime.Now;
        }
        else
        {
            if(timeOfPause == default(System.DateTime))
            {
                timeOfPause = System.DateTime.Now;
            }
            float timerToAdd = (float)(System.DateTime.Now - timeOfPause).TotalSeconds;
            timerForLife += timerToAdd;
            UpdateLives(timerForLife);

        }
    }

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Lives"))
        {
            PlayerPrefs.SetString("LifeUpdateTime", System.DateTime.Now.ToString());
        }

        currentLives = PlayerPrefs.GetInt("Lives", maxLives);

        if (currentLives < maxLives)
        {
            float timerToAdd = (float)(System.DateTime.Now - System.Convert.ToDateTime(PlayerPrefs.GetString("LifeUpdateTime"))).TotalSeconds;
            UpdateLives(timerToAdd);
        }
    }
}
