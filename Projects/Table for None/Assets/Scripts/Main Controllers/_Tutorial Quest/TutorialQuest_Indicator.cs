using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest_Indicator : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _indicatePointer;
    [SerializeField] private TutorialQuest_ScrObj[] _indicateQuests;
    
    
    // MonoBehaviour
    private void Start()
    {
        if (_indicateQuests.Length <= 0 && ES3.KeyExists("TutorialQuest_Controller/TutorialQuest_ControllerData") == false)
        {
            _indicatePointer.SetActive(true);
            return;
        }
        Toggle();
        
        // subscriptions
        TutorialQuest_Controller.instance.OnQuestComplete += Toggle;
    }

    private void OnDestroy()
    {
        // subscriptions
        TutorialQuest_Controller.instance.OnQuestComplete -= Toggle;
    }
    
    
    // Main
    private void Toggle()
    {
        if (_indicateQuests.Length <= 0)
        {
            _indicatePointer.SetActive(false);
            return;
        }
        
        List<TutorialQuest_ScrObj> showingQuests = TutorialQuest_Controller.instance.showingQuests;

        for (int i = 0; i < _indicateQuests.Length; i++)
        {
            if (showingQuests.Contains(_indicateQuests[i]) == false) continue;
            
            _indicatePointer.SetActive(true);
            return;
        }
        _indicatePointer.SetActive(false);
    }

    public void Interact_Toggle()
    {
        if (_indicateQuests.Length > 0) return;
        
        _indicatePointer.SetActive(false);
    }
}