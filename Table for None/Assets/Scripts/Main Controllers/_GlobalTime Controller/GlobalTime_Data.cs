using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalTime_Data
{
    [ES3Serializable] private int _timeCount;
    public int timeCount => _timeCount;

    [ES3Serializable] private TimePhase _timePhase;
    public TimePhase timePhase => _timePhase;
    
    
    // New
    public GlobalTime_Data(TimePhase timePhase)
    {
        _timePhase = timePhase;
    }
    
    
    // Data
    public void Set_TimeCount(int setValue)
    {
        _timeCount = setValue;
    }

    public void Set_TimePhase(TimePhase setPhase)
    {
        _timePhase = setPhase;
    }
}
