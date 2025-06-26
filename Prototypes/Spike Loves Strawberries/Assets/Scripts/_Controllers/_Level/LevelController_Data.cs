using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelController_Data
{
    private Level_ScrObj _currentLevel;
    public Level_ScrObj currentLevel => _currentLevel;

    private List<Level_ScrObj> _completedLevels = new();
    public List<Level_ScrObj> completedLevels => _completedLevels;


    // New
    public LevelController_Data(Level_ScrObj startLevel)
    {
        _currentLevel = startLevel;
    }
    
    
    // Data
    public void Set_CurrentLevel(Level_ScrObj level)
    {
        _currentLevel = level;
    }
}