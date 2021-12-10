using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lifeForAds : MonoBehaviour
{
    //Lives, LifeUpdateTime
    public Text currentLivesText;
    
    public static int maxLives = 2;  
    public static int currentLives;

    void Update()
    {
        currentLives = maxLives;
        currentLives = PlayerPrefs.GetInt("Lives", maxLives);
        PlayerPrefs.SetInt("Lives", currentLives);

        currentLivesText.text = "" + currentLives;
    }
}
