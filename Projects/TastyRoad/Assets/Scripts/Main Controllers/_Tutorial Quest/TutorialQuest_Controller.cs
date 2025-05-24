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
        _questBox.SetActive(_currentQuests.Count > 0);

        Input_Controller input = Input_Controller.instance;
        input.OnActionMapUpdate += () => _questBox.gameObject.SetActive(input.Current_ActionMapNum() == 0);

        Localization_Controller.instance.OnLanguageChanged += Update_QuestText;
    }

    private void OnDestroy()
    {
        Localization_Controller.instance.OnLanguageChanged -= Update_QuestText;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        Dictionary<string, int> currentQuestData = new();

        for (int i = 0; i < _currentQuests.Count; i++)
        {
            currentQuestData.Add(_currentQuests[i].questName, _currentQuests[i].currentCompleteCount);
        }
        
        ES3.Save("TutorialQuest_Controller", currentQuestData);
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("TutorialQuest_Controller") == false)
        {
            foreach (TutorialQuest quest in _quests)
            {
                _currentQuests.Add(quest);
            }
            return;
        }
        
        Dictionary<string, int> currentQuestData = new();
        currentQuestData = ES3.Load("TutorialQuest_Controller", currentQuestData);
        
        for (int i = 0; i < _quests.Length; i++)
        {
            TutorialQuest quest = _quests[i];
            
            if (currentQuestData.ContainsKey(quest.questName) == false) continue;
            if (currentQuestData.TryGetValue(quest.questName, out int completeCount) == false) continue;
            if (completeCount >= quest.completeCount) continue;

            quest.Load_Current(completeCount);
            _currentQuests.Add(quest);
        }
    }
    
    
    // Quest Box Control
    private void Update_QuestText()
    {
        if (_currentQuests.Count <= 0) return;
        
        _questText.text = "";
        
        int currentQuestNum = _currentQuests[0].questGroupNum;
        List<string> questLines = new();

        for (int i = 0; i < _currentQuests.Count; i++)
        {
            if (currentQuestNum != _currentQuests[i].questGroupNum) break;

            string completeCountString = "[ " + _currentQuests[i].currentCompleteCount + "/" + _currentQuests[i].completeCount + " ]  ";
            questLines.Add(completeCountString + _currentQuests[i].Description());
        }
        
        _questText.text = string.Join("\n\n", questLines);
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
    
    public void Complete_Quest(string questName, int completeUpdateValue)
    {
        TutorialQuest completeQuest = CurrentQuest(questName);
        if (completeQuest == null) return;
        
        if (completeQuest.Update_CompleteCount(completeUpdateValue) < completeQuest.completeCount)
        {
            Update_QuestText();
            return;
        }
        
        GoldSystem.instance.Update_CurrentAmount(completeQuest.goldAmount);
        _currentQuests.Remove(completeQuest);
        
        Update_QuestText();
        _questBox.SetActive(_currentQuests.Count > 0);
    }
}