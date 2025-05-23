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
        if (_currentQuests.Count <= 0) return;
        
        _questText.text = "";
        
        int currentQuestNum = _currentQuests[0].questGroupNum;
        List<string> questLines = new();

        for (int i = 0; i < _currentQuests.Count; i++)
        {
            if (currentQuestNum != _currentQuests[i].questGroupNum) break;
            
            string goldAmountString = _currentQuests[i].goldAmount + " <sprite=56> - ";
            questLines.Add(goldAmountString + _currentQuests[i].Description());
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
    
    public void Complete_Quest(string questName)
    {
        TutorialQuest completeQuest = CurrentQuest(questName);
        if (completeQuest == null) return;
        
        GoldSystem.instance.Update_CurrentAmount(completeQuest.goldAmount);
        _currentQuests.Remove(CurrentQuest(questName));
        
        Update_QuestText();
        _questBox.SetActive(_currentQuests.Count > 0);
    }
}