using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class adManager : MonoBehaviour
{
#if UNITY_ANDROID
    string gameID = "4492207";
#endif

    public GameObject adButton;

    void Start()
    {
        Advertisement.Initialize(gameID);
    }

    void Update()
    {
        if (lifeForAds.currentLives == 0)
        {
            adButton.SetActive(true);
        }
        else
        {
            adButton.SetActive(false);
        }
    }

    public void PlayAd()
    {
        Advertisement.Show("Rewarded_Android");
        
        //after ad is watch it adds additional 1 life
        lifeForAds.currentLives += 2;
        PlayerPrefs.SetInt("Lives", lifeForAds.currentLives);
    }

    public void PlayAdforJoy()
    {
        Advertisement.Show("Rewarded_Android");
        coinText.AddCoinsforAd();
    }

}
