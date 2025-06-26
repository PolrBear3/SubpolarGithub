using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Controller : MonoBehaviour
{
    public static Level_Controller instance;

    
    [Space(20)] 
    [SerializeField] private Level_ScrObj[] _levels;
    
    [Space(10)]
    [SerializeField][Range(0, 10)] private float _outPlatformDistance;
    public float outPlatformDistance => _outPlatformDistance;
    
    [Space(20)]
    [SerializeField] private Spike _player;
    public Spike player => _player;


    private LevelController_Data _data;
    
    private Level _currentLevel;
    public Level currentLevel => _currentLevel;
    
    private int _currentLevelIndex;

    public Action OnLevelUpdate;
    
    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;

        if (_levels.Length <= 0)
        {
            Debug.Log("Level List Empty!");
            return;
        }
        _data = new(_levels[0]);
    }

    private void Start()
    {
        Set_CurrentLevel();
    }
    
    
    // Level Control
    private void Set_CurrentLevel()
    {
        if (_data.currentLevel == null) return;

        Vector2 levelSetPos = Camera_Controller.instance.targetPosition;
        GameObject levelPrefab = Instantiate(_data.currentLevel._levelPrefab, levelSetPos, Quaternion.identity);
        
        _currentLevel = levelPrefab.GetComponent<Level>();

        _player.transform.position = _currentLevel.spawnPoint.position;
        _player.Toggle_TailDetachment(true);

        if (_player.data.hasItem == false) return;
        
        Destroy(_player.data.currentInteractable.gameObject);
        _player.data.Set_CurrentInteractable(null);
    }


    private bool Tail_Check()
    {
        if (_currentLevel.tailNeeded == false) return true;
        
        // check tail
        Spike_Data playerData = _player.data;
        if (playerData.hasTail) return true;

        // check plate
        List<GameObject> plateObjects = _currentLevel.exitPlate.detection.detectedObjects;

        for (int i = 0; i < plateObjects.Count; i++)
        {
            if (plateObjects[i].TryGetComponent(out Tail tail) == false) continue;
            return true;
        }

        // check player item
        if (playerData.hasItem == false) return false;
        if (playerData.currentInteractable.TryGetComponent(out Tail currentItem)) return true;

        return false;
    }

    private bool Barry_Check()
    {
        if (_currentLevel.berryNeeded == false) return true;
        
        // check plate
        List<GameObject> plateObjects = _currentLevel.exitPlate.detection.detectedObjects;

        for (int i = 0; i < plateObjects.Count; i++)
        {
            if (plateObjects[i].TryGetComponent(out Berry berry) == false) continue;
            return true;
        }
        
        // check player item
        Spike_Data playerData = _player.data;
        
        if (playerData.hasItem == false) return false;
        if (playerData.currentInteractable.TryGetComponent(out Berry currentItem)) return true;

        return false;
    }
    
    public void Complete_CurrentLevel()
    {
        if (_currentLevel.exitPlate.detection.playerDetected == false) return;
        if (Tail_Check() == false || Barry_Check() == false) return;
        
        _data.completedLevels.Add(_currentLevel.levelScrObj);
        Level_ScrObj updateLevel = _currentLevel.levelScrObj;
        
        for (int i = 0; i < _levels.Length; i++)
        {
            if (_data.completedLevels.Contains(_levels[i])) continue;
            updateLevel = _levels[i];
            break;
        }
        
        OnLevelUpdate?.Invoke();
        _data.Set_CurrentLevel(updateLevel);

        Camera_Controller.instance.Update_CameraPosition(currentLevel.exitDirection);

        StartCoroutine(LevelComplete_Coroutine(_currentLevel.gameObject));
        Set_CurrentLevel();
    }
    private IEnumerator LevelComplete_Coroutine(GameObject previousLevel)
    {
        yield return new WaitForSeconds(Camera_Controller.instance.tweenSpeed);
        Destroy(previousLevel);
    }
}
