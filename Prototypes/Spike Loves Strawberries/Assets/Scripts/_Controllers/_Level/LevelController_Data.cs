using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelController_Data
{
    private Level_ScrObj _currentLevel;
    public Level_ScrObj currentLevel => _currentLevel;

    private List<LevelStatus_Data> _completedDatas = new();
    public List<LevelStatus_Data> completedDatas => _completedDatas;


    // New
    public LevelController_Data(List<LevelStatus_Data> completedDatas)
    {
        _completedDatas = completedDatas;
    }
    
    
    // Data
    public void Set_CurrentLevel(Level_ScrObj level)
    {
        _currentLevel = level;
    }
    
    
    public LevelStatus_Data Completed_LevelData(Level_ScrObj completedLevel)
    {
        for (int i = 0; i < _completedDatas.Count; i++)
        {
            if (completedLevel != _completedDatas[i].level) continue;
            return _completedDatas[i];
        }
        return null;
    }
    
    public void Complete_CurrentLevel(float completeTime)
    {
        LevelStatus_Data data = Completed_LevelData(_currentLevel);
        
        if (data != null)
        {
            if (data.completed && completeTime >= data.completedTime) return;
            data.Set_CompletedTime(completeTime);
            
            return;
        }
        _completedDatas.Add(new(_currentLevel, completeTime));
    }
}