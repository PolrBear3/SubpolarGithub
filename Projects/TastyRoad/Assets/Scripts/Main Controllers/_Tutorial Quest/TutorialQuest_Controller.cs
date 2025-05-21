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

    [Space(20)] 
    [SerializeField][Range(0, 5)] private int _updateQuestCount;
    
    
    private List<TutorialQuest> _currentQuests = new();
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        Update_QuestText();

        // Input_Controller.instance.OnActionMapUpdate += () => _questBox.gameObject.SetActive();
    }

    
    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("TutorialQuest_Controller/_currentQuests", _currentQuests);
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("TutorialQuest_Controller/_currentQuests") == false)
        {
            for (int i = 0; i < _quests.Length; i++)
            {
                _currentQuests.Add(_quests[i]);
            }
            return;
        }

        List<TutorialQuest> loadQuests = new();
        _currentQuests = ES3.Load("TutorialQuest_Controller/_currentQuests", loadQuests);
    }
    
    
    // Quest Box Control
    private void Update_QuestText()
    {
        _questText.text = "";
        int questTextCount = 0;
        
        foreach (TutorialQuest quest in _currentQuests)
        {
            _questText.text += $"{quest.Description()}\n";
            questTextCount++;
            
            if (questTextCount >= _updateQuestCount) return;
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