using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelStatus_Data
{
    [ES3Serializable] private Level_ScrObj _level;
    public Level_ScrObj level => _level;

    [ES3Serializable] private float _completedTime;
    public float completedTime => _completedTime;
    
    [ES3Serializable] private bool _completed;
    public bool completed => _completed;
    
    
    // New
    public LevelStatus_Data(Level_ScrObj level, float completedTime)
    {
        _level = level;
        _completedTime = completedTime;
    }
    
    
    // Data Control
    public void Set_CompletedTime(float setValue)
    {
        _completedTime = setValue;
        _completed = true;
    }
}
