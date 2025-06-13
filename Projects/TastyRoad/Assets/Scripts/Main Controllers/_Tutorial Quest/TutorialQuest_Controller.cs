using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class TutorialQuest_Controller : MonoBehaviour, ISaveLoadable
{
    public static TutorialQuest_Controller instance;
    
    
    [Space(20)]
    [SerializeField] private GameObject _questBox;
    [SerializeField] private TextMeshProUGUI _questText;
    
    [Space(20)] 
    [SerializeField] private TutorialQuest_Group[] _questGroups;

    
    [SerializeField] private List<TutorialQuest> _currentQuests = new();
    
    
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
        input.OnActionMapUpdate += () => _questBox.gameObject.SetActive(input.Current_ActionMapNum() == 0 && _currentQuests.Count > 0);

        Localization_Controller.instance.OnLocalizationLoad += Update_QuestText;
        Localization_Controller.instance.OnLanguageChanged += Update_QuestText;
    }

    private void OnDestroy()
    {
        Localization_Controller.instance.OnLocalizationLoad -= Update_QuestText;
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
            for (int i = 0; i < _questGroups.Length; i++)
            {
                for (int j = 0; j < _questGroups[i].quests.Length; j++)
                {
                    _currentQuests.Add(_questGroups[i].quests[j]);
                    _questGroups[i].quests[j].Set_GroupNum(i);
                }
            }
            return;
        }
        
        Dictionary<string, int> currentQuestData = new();
        currentQuestData = ES3.Load("TutorialQuest_Controller", currentQuestData);
        
        for (int i = 0; i < _questGroups.Length; i++)
        {
            for (int j = 0; j < _questGroups[i].quests.Length; j++)
            {
                TutorialQuest quest = _questGroups[i].quests[j];
            
                if (currentQuestData.ContainsKey(quest.questName) == false) continue;
                if (currentQuestData.TryGetValue(quest.questName, out int completeCount) == false) continue;
                if (completeCount >= quest.completeCount) continue;

                quest.Load_CompleteCount(completeCount);
                
                _currentQuests.Add(quest);
                _questGroups[i].quests[j].Set_GroupNum(i);
            }
        }
    }
    
    
    // Quest Box Control
    private void Update_QuestText()
    {
        if (_currentQuests.Count <= 0) return;
        
        _questText.text = "";
        
        int currentGroupNum = _currentQuests[0].groupNum;
        List<string> questLines = new();

        for (int i = 0; i < _currentQuests.Count; i++)
        {
            if (currentGroupNum != _currentQuests[i].groupNum) break;

            string completeCountString = "[ " + _currentQuests[i].currentCompleteCount + "/" + _currentQuests[i].completeCount + " ]  ";
            questLines.Add(completeCountString + _currentQuests[i].Description());
        }
        
        _questText.text = string.Join("\n\n", questLines);
    }
    
    
    // Data Control
    public TutorialQuest CurrentQuest(string questName)
    {
        for (int i = 0; i < _currentQuests.Count; i++)
        {
            if (questName != _currentQuests[i].questName) continue;
            return _currentQuests[i];
        }

        return null;
    }

    public void Complete_Quest(TutorialQuest questData, int completeUpdateValue)
    {
        if (questData == null || completeUpdateValue == 0) return;
        
        if (questData.Update_CompleteCount(completeUpdateValue) < questData.completeCount)
        {
            Update_QuestText();
            return;
        }
        
        GoldSystem.instance.Update_CurrentAmount(questData.goldAmount);
        _currentQuests.Remove(questData);
        
        Update_QuestText();
        
        bool isIngame = Input_Controller.instance.Current_ActionMapNum() == 0;
        _questBox.SetActive(isIngame && _currentQuests.Count > 0);
    }
    public void Complete_Quest(string questName, int completeUpdateValue)
    {
        Complete_Quest(CurrentQuest(questName), completeUpdateValue);
    }
}