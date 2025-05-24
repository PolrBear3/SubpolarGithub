using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class TutorialQuest
{
    [SerializeField] private int _questGroupNum;
    public int questGroupNum => _questGroupNum;
    
    [Space(20)]
    [SerializeField] private string _questName;
    public string questName => _questName;
    
    [SerializeField] private string _description;
    public string description => _description;

    [SerializeField] private int _completeCount;
    public int completeCount => _completeCount;
    
    [SerializeField] [Range(0, 100)] private int _goldAmount;
    public int goldAmount => _goldAmount;
    
    [Space(20)]
    [SerializeField] private LocalizedString _localizedDescription;
    public LocalizedString localizedDescription => _localizedDescription;


    [ES3Serializable] private int _currentCompleteCount;
    public int currentCompleteCount => _currentCompleteCount;
    
    
    //
    public string Description()
    {
        if (_localizedDescription == null) return _description;
        if (string.IsNullOrEmpty(_localizedDescription.TableReference) && string.IsNullOrEmpty(_localizedDescription.TableEntryReference)) return _description;

        return _localizedDescription.GetLocalizedString();
    }


    public void Load_Current(int loadValue)
    {
        _currentCompleteCount = Mathf.Clamp(loadValue, 0, completeCount);
    }
    
    public int Update_CompleteCount(int updateValue)
    {
        _currentCompleteCount += updateValue;
        
        return _currentCompleteCount;
    }
}
