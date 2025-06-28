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

    [Space(20)] 
    [SerializeField] private InfoBox_Data _infoData;
    public InfoBox_Data infoData => _infoData;

    
    private float _levelTime;
    public float levelTime => _levelTime;
    
    private Coroutine _timeCroutine;
    
    
    // MonoBehaviour
    public void Start()
    {
        _tailIcon.SetActive(_tailNeeded);
        _berryIcon.SetActive(_berryNeeded);

        Start_Timer();
        
        // subscriptions
        _exitPlate.onPressAction += Level_Controller.instance.Complete_CurrentLevel;
    }

    public void OnDestroy()
    {
        Stop_Timer();
        
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
    
    
    // Timer
    public void Stop_Timer()
    {
        if (_timeCroutine == null) return;
        
        StopCoroutine(_timeCroutine);
        _timeCroutine = null;
    }
    
    private void Start_Timer()
    {
        _timeCroutine = StartCoroutine(Timer_Coroutine());
    }
    private IEnumerator Timer_Coroutine()
    {
        while (true)
        {
            _levelTime += Time.deltaTime;
            yield return null;
        }
    }
}
