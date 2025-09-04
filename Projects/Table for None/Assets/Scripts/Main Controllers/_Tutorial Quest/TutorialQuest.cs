using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class TutorialQuest_Group
{
    [SerializeField] private TutorialQuest_ScrObj[] _quests;
    public TutorialQuest_ScrObj[] quests => _quests;
}

[System.Serializable]
public class TutorialQuest
{
    [ES3Serializable] private TutorialQuest_ScrObj _questScrObj;
    public TutorialQuest_ScrObj questScrObj => _questScrObj;
    
    private int _groupNum;
    public int groupNum => _groupNum;
    
    [ES3Serializable] private int _currentCompleteCount;
    public int currentCompleteCount => _currentCompleteCount;
    
    
    // New
    public TutorialQuest(TutorialQuest_ScrObj questScrObj, int groupNum)
    {
        _questScrObj = questScrObj;
        _groupNum = groupNum;
    }
    
    
    // Current Data
    public void Set_GroupNum(int groupNum)
    {
        _groupNum = groupNum;
    }
    
    
    public void Load_CompleteCount(int loadValue)
    {
        _currentCompleteCount = Mathf.Clamp(loadValue, 0, _questScrObj.completeCount);
    }
    
    public int Update_CompleteCount(int updateValue)
    {
        _currentCompleteCount += updateValue;
        
        return _currentCompleteCount;
    }
}
