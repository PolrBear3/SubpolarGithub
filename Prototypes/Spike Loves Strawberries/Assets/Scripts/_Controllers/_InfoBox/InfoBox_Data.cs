using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class InfoBox_Data
{
    [Space(20)]
    [SerializeField] private string _defaultInfo;
    [SerializeField] private LocalizedString _localizedInfo;

    [Space(20)] 
    [SerializeField] private Vector2 _setPosition;
    public Vector2 setPosition => _setPosition;
    
    
    // Main
    public string InfoString()
    {
        bool notLocalized = string.IsNullOrEmpty(_localizedInfo.TableReference) || string.IsNullOrEmpty(_localizedInfo.TableEntryReference);
        if (_localizedInfo == null || notLocalized) return _defaultInfo;

        return _localizedInfo.GetLocalizedString();
    }
}