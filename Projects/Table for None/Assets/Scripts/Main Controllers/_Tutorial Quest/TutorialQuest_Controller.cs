using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialQuest_Controller : MonoBehaviour, ISaveLoadable
{
    public static TutorialQuest_Controller instance;
    
    
    [Space(20)]
    [SerializeField] private UI_EffectController _effectController;
    
    [Space(20)]
    [SerializeField] private RectTransform _questBox;
    [SerializeField] private TextMeshProUGUI _questText;
    
    [Space(20)] 
    [SerializeField] private TutorialQuest_Group[] _questGroups;


    private TutorialQuest_ControllerData _data;
    public TutorialQuest_ControllerData data => _data;

    private List<TutorialQuest_ScrObj> _showingQuests = new();
    public List<TutorialQuest_ScrObj> showingQuests => _showingQuests;
    
    public Action OnQuestComplete;
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        Toggle_QuestBox(true);
        Update_QuestText();

        // subscriptions
        Cutscene_Controller.instance.OnToggle += Toggle_QuestBox;

        Input_Controller.instance.OnActionMapUpdate += Toggle_QuestBox;

        Localization_Controller.instance.OnLocalizationLoad += Update_QuestText;
        Localization_Controller.instance.OnLanguageChanged += Update_QuestText;

    }

    private void OnDestroy()
    {
        // subscriptions
        Cutscene_Controller.instance.OnToggle -= Toggle_QuestBox;
        
        Localization_Controller.instance.OnLocalizationLoad -= Update_QuestText;
        Localization_Controller.instance.OnLanguageChanged -= Update_QuestText;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("TutorialQuest_Controller/TutorialQuest_ControllerData", _data);
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("TutorialQuest_Controller/TutorialQuest_ControllerData") == false)
        {
            _data = new();
            List<TutorialQuest> currentQuests = _data.currentQuests;
            
            for (int i = 0; i < _questGroups.Length; i++)
            {
                for (int j = 0; j < _questGroups[i].quests.Length; j++)
                {
                    currentQuests.Add(new(_questGroups[i].quests[j], i));
                }
            }
            return;
        }

        _data = ES3.Load("TutorialQuest_Controller/TutorialQuest_ControllerData", new TutorialQuest_ControllerData());
        List<TutorialQuest> loadedQuests = _data.currentQuests;
        
        for (int i = 0; i < loadedQuests.Count; i++)
        {
            for (int j = 0; j < _questGroups.Length; j++)
            {
                TutorialQuest_ScrObj[] questsInGroup = _questGroups[j].quests;
                
                for (int k = 0; k < questsInGroup.Length; k++)
                {
                    if (loadedQuests[i].questScrObj != questsInGroup[k]) continue;
                    loadedQuests[i].Set_GroupNum(j);
                    
                    break;
                }
            }
        }
    }
    
    
    // Main
    private bool QuestBox_ToggleAvailable()
    {
        List<TutorialQuest> currentQuests = _data.currentQuests;
        
        bool inGame = Input_Controller.instance.Current_ActionMapNum() == 0 && currentQuests.Count > 0;
        bool cutscenePlaying = Cutscene_Controller.instance.coroutine != null;

        return inGame && cutscenePlaying == false;
    }
    
    private void Toggle_QuestBox(bool toggle)
    {
        bool toggleAvailable = QuestBox_ToggleAvailable();
        _questBox.gameObject.SetActive(toggle && toggleAvailable);

        if (toggle == false || toggleAvailable == false) return;
        _effectController.Update_Scale(_questBox.gameObject);
    }
    private void Toggle_QuestBox()
    {
        Toggle_QuestBox(QuestBox_ToggleAvailable());
    }
    
    
    // Quest Box Control
    private void Update_QuestText()
    {
        List<TutorialQuest> currentQuests = _data.currentQuests;
        if (currentQuests.Count <= 0) return;
        
        _showingQuests.Clear();
        _questText.text = "";
        
        int currentGroupNum = currentQuests[0].groupNum;
        List<string> questLines = new();

        for (int i = 0; i < currentQuests.Count; i++)
        {
            if (currentGroupNum != currentQuests[i].groupNum) break;

            TutorialQuest_ScrObj questScrObj = currentQuests[i].questScrObj;
            _showingQuests.Add(questScrObj); 
            
            string completeCountString = "<sprite=69> [ " + currentQuests[i].currentCompleteCount + "/" + questScrObj.completeCount + " ]  ";
            questLines.Add(completeCountString + questScrObj.Description());
        }
        
        _questText.text = string.Join("\n\n", questLines);
    }
    
    
    // Data Control
    public TutorialQuest CurrentQuest(TutorialQuest_ScrObj questScrObj)
    {
        List<TutorialQuest> currentQuests = _data.currentQuests;
        
        for (int i = 0; i < currentQuests.Count; i++)
        {
            if (questScrObj != currentQuests[i].questScrObj) continue;
            return currentQuests[i];
        }

        return null;
    }
    public TutorialQuest_ScrObj CurrentQuest(string questName)
    {
        List<TutorialQuest> currentQuests = _data.currentQuests;
        
        for (int i = 0; i < currentQuests.Count; i++)
        {
            if (questName != currentQuests[i].questScrObj.questName) continue;
            return currentQuests[i].questScrObj;
        }

        return null;
    }

    public void Complete_Quest(TutorialQuest_ScrObj questScrObj, int completeUpdateValue)
    {
        TutorialQuest currentQuest = CurrentQuest(questScrObj);
        if (currentQuest == null || completeUpdateValue == 0) return;

        if (currentQuest.Update_CompleteCount(completeUpdateValue) < questScrObj.completeCount)
        {
            Update_QuestText();
            return;
        }
        
        List<TutorialQuest> currentQuests = _data.currentQuests;
        
        GoldSystem.instance.Update_CurrentAmount(questScrObj.goldAmount);
        currentQuests.Remove(currentQuest);
        
        Update_QuestText();
        
        bool isIngame = Input_Controller.instance.Current_ActionMapNum() == 0;
        Toggle_QuestBox(isIngame && currentQuests.Count > 0);
        
        OnQuestComplete?.Invoke();
    }
}