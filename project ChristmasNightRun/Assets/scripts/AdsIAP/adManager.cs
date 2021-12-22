using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class adManager : MonoBehaviour, IUnityAdsListener
{
    string gameID = "4492207";

    public GameObject adButton;

    void Start()
    {
        Advertisement.Initialize(gameID);
    }

    void Update()
    {
        if (lifeForAds.currentLives <= 0)
        {
            adButton.SetActive(true);
        }
        else
        {
            adButton.SetActive(false);
        }
    }

    void OnEnable()
    {
        Advertisement.AddListener(this);
    }

    void OnDisable()
    {
        Advertisement.RemoveListener(this);
    }

    public void PlayAdforLife(string p)
    {
        Advertisement.Show(p);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(showResult == ShowResult.Finished)
        {
            lifeForAds.currentLives += 2;
            PlayerPrefs.SetInt("Lives", lifeForAds.currentLives);
        }
        else if(showResult == ShowResult.Failed)
        {
            Debug.Log("Ad failure");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
    }
    public void OnUnityAdsDidError(string message)
    {
    }
    public void OnUnityAdsDidStart(string placementId)
    {  
    }
}
