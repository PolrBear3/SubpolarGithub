using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Level_ScrObj _levelScrObj;
    public Level_ScrObj levelScrObj => _levelScrObj;
    
    [Space(20)]
    [SerializeField] private Transform _spawnPoint;
    public Transform spawnPoint => _spawnPoint;
    
    [SerializeField] private SpriteRenderer[] _platforms;

    [Space(20)] 
    [SerializeField] private PressurePlate _exitPlate;
    public PressurePlate exitPlate => _exitPlate;

    [SerializeField] private Vector2 _exitDirection;
    public Vector2 exitDirection => _exitDirection;

    [Space(20)] 
    [SerializeField] private GameObject _tailIcon;
    
    [SerializeField] private bool _tailNeeded;
    public bool tailNeeded => _tailNeeded;
    
    [Space(10)] 
    [SerializeField] private GameObject _berryIcon;
    
    [SerializeField] private bool _berryNeeded;
    public bool berryNeeded => _berryNeeded;
    
    
    // MonoBehaviour
    private void Start()
    {
        _tailIcon.SetActive(_tailNeeded);
        _berryIcon.SetActive(_berryNeeded);
        
        // subscriptions
        _exitPlate.onPressAction += Level_Controller.instance.Complete_CurrentLevel;
    }

    private void OnDestroy()
    {
        // subscriptions
        _exitPlate.onPressAction -= Level_Controller.instance.Complete_CurrentLevel;
    }


    // Data
    public bool Position_OnPlatform(Transform checkPoint)
    {
        float outDistance = Level_Controller.instance.outPlatformDistance;;
        Vector2 checkPosition = checkPoint.position;
        
        for (int i = 0; i < _platforms.Length; i++)
        {
            Bounds bounds = _platforms[i].bounds;
            
            Vector2 closest = bounds.ClosestPoint(checkPosition);
            float distance = Vector2.Distance(checkPosition, closest);
            
            if (distance > outDistance) continue;
            return true;
        }
        
        return false;
    }

    public GameObject Target_Platform(Transform targetPosition)
    {
        for (int i = 0; i < _platforms.Length; i++)
        {
            Bounds bounds = _platforms[i].bounds;
            
            if (bounds.Contains(targetPosition.position) == false) continue;
            return _platforms[i].gameObject;
        }

        return Level_Controller.instance.gameObject;
    }
}
