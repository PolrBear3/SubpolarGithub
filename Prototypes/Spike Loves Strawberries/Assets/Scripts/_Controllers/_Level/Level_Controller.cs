using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Controller : MonoBehaviour
{
    public static Level_Controller instance;
    
    [Space(20)] 
    [SerializeField] private Level _mainLevel;
    
    [Space(20)] 
    [SerializeField] private Level_ScrObj[] _levels;
    [SerializeField] private LevelStatus_Indicator[] _statusIndicators;
    
    [Space(20)]
    [SerializeField][Range(0, 10)] private float _outPlatformDistance;
    public float outPlatformDistance => _outPlatformDistance;
    
    [Space(20)]
    [SerializeField] private Spike _player;
    public Spike player => _player;

    [Space(20)] 
    [SerializeField] private InfoBox_Data _infoData;


    private LevelController_Data _data;
    
    private Level _currentLevel;
    public Level currentLevel => _currentLevel;

    private bool _levelRemoving;
    public bool levelRemoving => _levelRemoving;

    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;

        List<LevelStatus_Data> completedDatas = new(ES3.Load("LevelController_Datas", new List<LevelStatus_Data>()));
        _data = new(completedDatas);
        
        if (_data.completedDatas.Count > 0) return;
        _data.completedDatas.Add(new(_levels[0], 0f));
    }

    private void Start()
    {
        Set_MainLevel();
        
        // subscrptions
        Main_InputSystem inputSystem = Main_InputSystem.instance;
        
        inputSystem.OnCancelInput += Exit_CurrentLevel;
        inputSystem.OnHoldCancelInput += Restart_CurrentLevel;
    }

    private void OnDestroy()
    {
        // subscrptions
        Main_InputSystem inputSystem = Main_InputSystem.instance;
        
        inputSystem.OnCancelInput -= Exit_CurrentLevel;
        inputSystem.OnHoldCancelInput -= Restart_CurrentLevel;
    }

    private void OnApplicationQuit()
    {
        ES3.Save("LevelController_Datas", _data.completedDatas);
    }


    // Level Status
    private void Update_LevelUnlocks()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            LevelStatus_Data previousData = i > 0 ? _data.Completed_LevelData(_levels[i - 1]) : null;
            
            if (previousData == null) continue;
            if (previousData.completed == false) continue;
            if (_data.Completed_LevelData(_levels[i]) != null) continue;
            
            _data.completedDatas.Add(new(_levels[i], 0f));
        }
    }
    
    private void Update_LevelIndications()
    {
        for (int i = 0; i < _statusIndicators.Length; i++)
        {
            bool completed = i < _levels.Length && _data.Completed_LevelData(_levels[i]) != null;
            _statusIndicators[i].Toggle_Lock(!completed);

            if (completed == false) continue;
            _statusIndicators[i].Set_Time(_data.Completed_LevelData(_levels[i]).completedTime);
        }
    }
    
    
    // Level Control
    private void Set_MainLevel()
    {
        _data.Set_CurrentLevel(_mainLevel.levelScrObj);
        _currentLevel = _mainLevel;
        
        Reset_Player();
        
        Update_LevelUnlocks();
        Update_LevelIndications();

        Camera_Controller camera = Camera_Controller.instance;
        camera.Set_CameraPosition(_currentLevel.transform.position);
        
        InfoBox_Controller.instance.Update_InfoText(_infoData.InfoString());
    }

    private void Exit_CurrentLevel()
    {
        if (_currentLevel.levelScrObj == _mainLevel.levelScrObj) return;

        StartCoroutine(LevelRemove_Coroutine(_currentLevel.gameObject));
        Set_MainLevel();
    }
    
    
    private void Set_CurrentLevel()
    {
        if (_data.currentLevel == null) return;

        Vector2 levelSetPos = Camera_Controller.instance.targetPosition;
        
        GameObject levelPrefab = Instantiate(_data.currentLevel._levelPrefab, levelSetPos, Quaternion.identity);
        _currentLevel = levelPrefab.GetComponent<Level>();

        Reset_Player();

        if (_currentLevel.infoData == null)
        {
            InfoBox_Controller.instance.infoBoxPanel.gameObject.SetActive(false);
            return;
        }
        InfoBox_Controller.instance.Update_InfoText(_currentLevel.infoData.InfoString(), _currentLevel.infoData.setPosition);
    }
    public void Set_CurrentLevel(Level_ScrObj setLevel)
    {
        if (setLevel == null) return;
        if (_data.Completed_LevelData(setLevel) == null) return;
        if (_data.currentLevel == setLevel) return;

        _data.Set_CurrentLevel(setLevel);
        
        Camera_Controller.instance.Update_CameraPosition(currentLevel.exitDirection);
        Set_CurrentLevel();
    }

    public void Restart_CurrentLevel()
    {
        if (_currentLevel.levelScrObj == _mainLevel.levelScrObj) return;

        _data.Set_CurrentLevel(_currentLevel.levelScrObj);
        Camera_Controller.instance.Update_CameraPosition(currentLevel.exitDirection);

        StartCoroutine(LevelRemove_Coroutine(_currentLevel.gameObject));
        Set_CurrentLevel();
    }

    private void Reset_Player()
    {
        _player.transform.position = _currentLevel.spawnPoint.position;
        _player.transform.rotation = Quaternion.identity;
        
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


    private int CurrentLevel_IndexNum()
    {
        int indexNum = 0;

        for (int i = 0; i < _levels.Length; i++)
        {
            if (_currentLevel.levelScrObj != _levels[i])
            {
                indexNum++;
                continue;
            }
            return indexNum;
        }
        return indexNum;
    }
    
    public void Complete_CurrentLevel()
    {
        if (_currentLevel.exitPlate.detection.playerDetected == false) return;
        if (Tail_Check() == false || Barry_Check() == false) return;
        
        _data.Complete_CurrentLevel(_currentLevel.levelTime);
        Level_ScrObj updateLevel = _mainLevel.levelScrObj;
 
        for (int i = 0; i < _levels.Length; i++)
        {
            if (i >= _levels.Length - 1 || CurrentLevel_IndexNum() >= _levels.Length - 1)
            {
                StartCoroutine(LevelRemove_Coroutine(_currentLevel.gameObject));
                Set_MainLevel();
                return;
            }
            
            if (_data.Completed_LevelData(_levels[i + 1]) != null) continue;
            
            updateLevel = _levels[i + 1];
            break;
        }

        _data.Set_CurrentLevel(updateLevel);
        Camera_Controller.instance.Update_CameraPosition(currentLevel.exitDirection);

        StartCoroutine(LevelRemove_Coroutine(_currentLevel.gameObject));
        Set_CurrentLevel();
    }
    private IEnumerator LevelRemove_Coroutine(GameObject previousLevel)
    {
        if (previousLevel == null) yield break;
        
        _levelRemoving = true;
        
        yield return new WaitForSeconds(Camera_Controller.instance.tweenSpeed);
        Destroy(previousLevel);

        yield return null;
        _levelRemoving = false;
    }
}
