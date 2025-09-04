using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoTemplate_Trigger : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private TextMeshProUGUI _setText;
    public TextMeshProUGUI setText => _setText;
    
    [Space(20)]
    [SerializeField] private Information_Template[] _templates;
    public Information_Template[] templates => _templates;
    
    
    // UnityEngine
    private void Start()
    {
        Update_SetText();
        Input_Controller.instance.OnSchemeUpdate += Update_SetText;
    }

    private void OnDestroy()
    {
        Input_Controller.instance.OnSchemeUpdate -= Update_SetText;
    }


    // Set Text
    private void Update_SetText()
    {
        if (_setText == null) return;
        Input_Controller.instance.Update_EmojiAsset(_setText);
    }
    
    
    // Template Data
    public string TemplateString(int templateIndex)
    {
        templateIndex = Mathf.Clamp(templateIndex, 0, _templates.Length - 1);
        return _templates[templateIndex].InfoString();
    }
    
    
    // Package Templates
    public string KeyControl_Template(string action1, string action2, string holdAction)
    {
        string action1Key = "<sprite=0> " + action1 + "\n";
        string action2Key = "<sprite=2> " + action2;
        
        _templates[1].Set_SmartInfo("hold", holdAction);
        string holdKey = !string.IsNullOrEmpty(holdAction) ? "\n" + _templates[1].InfoString() : String.Empty;

        return action1Key + action2Key + holdKey;
    }
}
