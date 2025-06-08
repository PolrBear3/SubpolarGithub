using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Ability!")]
public class Ability_ScrObj : ScriptableObject
{
    [Space(20)]
    [SerializeField] private string _abilityName;
    public string abilityName => _abilityName;

    [Space(20)]
    [SerializeField][TextArea(2, 2)] private string _description;
    public string description => _description;
    
    [SerializeField] private LocalizedString _localizedDescription;
    public LocalizedString localizedDescription => _localizedDescription;

    [Space(20)]
    [SerializeField] private Abiliy_ActivationData[] _activationDatas;
    

    // Gets
    public string Description()
    {
        if (_localizedDescription == null) return _description;
        if (string.IsNullOrEmpty(_localizedDescription.TableReference) && string.IsNullOrEmpty(_localizedDescription.TableEntryReference)) return _description;

        return _localizedDescription.GetLocalizedString();
    }

    public int Max_ActivationCount()
    {
        return _activationDatas.Length - 1;
    }

    public Abiliy_ActivationData Abiliy_ActivationData(int progressLevel)
    {
        progressLevel = Mathf.Clamp(progressLevel, 0, Max_ActivationCount());
        int indexNum = Mathf.FloorToInt((float)progressLevel / Max_ActivationCount() * (_activationDatas.Length - 1));

        return _activationDatas[indexNum];
    }
}