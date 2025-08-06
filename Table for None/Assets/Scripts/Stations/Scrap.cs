using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    private SpriteRenderer _sr;

    
    [Space(20)]
    [SerializeField] private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;
    
    [SerializeField] private VideoGuide_Trigger _videoGuideTrigger;

    [Space(20)]
    [SerializeField] private List<Sprite> _scrapSprites = new();


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _stationController = gameObject.GetComponent<Station_Controller>();
    }

    private void Start()
    {
        Set_RandomSprite();
        Update_LockInteraction();
        
        // subscriptions
        VideoGuide_Controller videoGuideController = VideoGuide_Controller.instance;
        
        videoGuideController.OnGuide_ActivationTrigger += Update_LockInteraction;
        videoGuideController.OnGuideTrigger += Update_LockInteraction;
    }

    private void OnDestroy()
    {
        // subscriptions
        VideoGuide_Controller videoGuideController = VideoGuide_Controller.instance;
        
        videoGuideController.OnGuide_ActivationTrigger -= Update_LockInteraction;
        videoGuideController.OnGuideTrigger -= Update_LockInteraction;
    }


    // Main
    private void Set_RandomSprite()
    {
        int randArrayNum = UnityEngine.Random.Range(0, _scrapSprites.Count);
        _sr.sprite = _scrapSprites[randArrayNum];
    }

    private void Update_LockInteraction()
    {
        VideoGuide_Controller videoGuideController = VideoGuide_Controller.instance;
        bool guideTriggered = videoGuideController.Guide_Triggered(_videoGuideTrigger.triggerGuide);

        _stationController.iInteractable.Toggle_Lock(guideTriggered);
    }
}
