using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Serialization;


[System.Serializable]
public class Information_Template
{
    [SerializeField] private string _infoName;
    public string infoName => _infoName;

    [SerializeField] private LocalizedString _localizedInfo;
    public LocalizedString localizedInfo => _localizedInfo;
    
    
    // Data Control
    public void Set_SmartInfo(Dictionary<string, string> strings)
    {
        if (_localizedInfo == null) return;
        if (string.IsNullOrEmpty(localizedInfo.TableReference) && string.IsNullOrEmpty(localizedInfo.TableEntryReference)) return;

        foreach (var smartString in strings)
        {
            (_localizedInfo[smartString.Key] as StringVariable).Value = smartString.Value;
        }
    }

    public void Set_SmartInfo(string variableName, string setString)
    {
        if (_localizedInfo == null) return;
        if (string.IsNullOrEmpty(localizedInfo.TableReference) && string.IsNullOrEmpty(localizedInfo.TableEntryReference)) return;

        (_localizedInfo[variableName] as StringVariable).Value = setString;
    }
    
    
    public string InfoString()
    {
        if (_localizedInfo == null) return _infoName;
        if (string.IsNullOrEmpty(localizedInfo.TableReference) && string.IsNullOrEmpty(localizedInfo.TableEntryReference)) return _infoName;

        return localizedInfo.GetLocalizedString();
    }
}