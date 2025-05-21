using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialQuest_Controller : MonoBehaviour, ISaveLoadable
{
    public static TutorialQuest_Controller instance;
    
    
    [Space(20)]
    [SerializeField] private GameObject _questBox;
    [SerializeField] private TextMeshProUGUI _questText;

    [Space(20)] 
    [SerializeField] private TutorialQuest[] _quests;

    
    private List<TutorialQuest> _currentQuests = new();
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        Update_QuestText();

        Input_Controller input = Input_Controller.instance;
        input.OnActionMapUpdate += () => _questBox.gameObject.SetActive(input.Current_ActionMapNum() == 0);
    }

    
    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("TutorialQuest_Controller", _quests.Length - _currentQuests.Count);
    }

    public void Load_Data()
    {
        int completedQuestCount = ES3.Load("TutorialQuest_Controller", 0);
        
        for (int i = 0; i < _quests.Length; i++)
        {
            if (completedQuestCount > 0)
            {
                completedQuestCount--;
                continue;
            }
            
            _currentQuests.Add(_quests[i]);
        }
    }
    
    
    // Quest Box Control
    private void Update_QuestText()
    {
        _questBox.SetActive(_currentQuests.Count > 0);
        
        if (_currentQuests.Count <= 0) return;
        _questText.text = "";
        
        int currentQuestNum = _currentQuests[0].questGroupNum;

        foreach (TutorialQuest quest in _currentQuests)
        {
            if (currentQuestNum != quest.questGroupNum) return;
            _questText.text += $"{quest.Description()}\n";
        }
    }
    
    
    // Data Control
    private TutorialQuest CurrentQuest(string questName)
    {
        for (int i = 0; i < _currentQuests.Count; i++)
        {
            if (questName != _currentQuests[i].questName) continue;
            return _currentQuests[i];
        }

        return null;
    }
    
    private bool Quest_Completed(string questName)
    {
        return CurrentQuest(questName) == null;
    }
    
    
    public void Complete_Quest(string questName)
    {
        if (Quest_Completed(questName)) return;

        for (int i = 0; i < _currentQuests.Count; i++)
        {   
            if (_currentQuests[i].questName != questName) continue;
            _currentQuests.RemoveAt(i);
        }
        
        Update_QuestText();
    }
}