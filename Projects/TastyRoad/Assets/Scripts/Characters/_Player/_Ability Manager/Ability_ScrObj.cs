using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Ability!")]
public class Ability_ScrObj : ScriptableObject
{
    [Header("")]
    [SerializeField] private string _abilityName;
    public string abilityName => _abilityName;

    [Header("")]
    [SerializeField][TextArea(2, 2)] private string _description;
    public string description => _description;
    
    [SerializeField] private LocalizedString _localizedDescription;
    public LocalizedString localizedDescription => _localizedDescription;

    [Header("")]
    [SerializeField] private Sprite[] _progressIcons;
    public Sprite[] progressIcons => _progressIcons;
    

    public string Description()
    {
        if (_localizedDescription == null) return _description;
        if (string.IsNullOrEmpty(_localizedDescription.TableReference) && string.IsNullOrEmpty(_localizedDescription.TableEntryReference)) return _description;

        return _localizedDescription.GetLocalizedString();
    }

    public int Max_ActivationCount()
    {
        return _progressIcons.Length - 1;
    }
    
    
    public Sprite ProgressIcon(int levelValue)
    {
        levelValue = Mathf.Clamp(levelValue, 0, Max_ActivationCount());
        int spriteIndex = Mathf.FloorToInt((float)levelValue / Max_ActivationCount() * (progressIcons.Length - 1));

        return _progressIcons[spriteIndex];
    }
}
