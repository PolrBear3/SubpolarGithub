using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

[System.Serializable]
public class DialogData
{
    public Sprite icon;

    [Header("")]
    [TextArea(2, 2)]
    public string info;

    
    [Header("")]
    [SerializeField] private LocalizedString _localizedInfo;
    public LocalizedString localizedInfo => _localizedInfo;


    // new
    public DialogData (Sprite icon, string info)
    {
        this.icon = icon;
        this.info = info;
    }
    
    
    // Information
    public void Set_SmartInfo(Dictionary<string, string> strings)
    {
        if (_localizedInfo == null) return;
        if (string.IsNullOrEmpty(localizedInfo.TableReference) && string.IsNullOrEmpty(localizedInfo.TableEntryReference)) return;

        foreach (var smartString in strings)
        {
            (_localizedInfo[smartString.Key] as StringVariable).Value = smartString.Value;
        }
    }
    
    public string DialogInfo()
    {
        if (_localizedInfo == null) return info;
        if (string.IsNullOrEmpty(localizedInfo.TableReference) && string.IsNullOrEmpty(localizedInfo.TableEntryReference)) return info;

        return localizedInfo.GetLocalizedString();
    }
}