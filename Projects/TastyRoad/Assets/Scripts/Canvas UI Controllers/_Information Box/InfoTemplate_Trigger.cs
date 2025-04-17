using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTemplate_Trigger : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Information_Template[] _templates;
    public Information_Template[] templates => _templates;
    
    
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
        string action2Key = "<sprite=2> " + action2 + "\n";
        
        _templates[1].Set_SmartInfo("hold", holdAction);
        string holdKey = _templates[1].InfoString();

        return action1Key + action2Key + holdKey;
    }
}
