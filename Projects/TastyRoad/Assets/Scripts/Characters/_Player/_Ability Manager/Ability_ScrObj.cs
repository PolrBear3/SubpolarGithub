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
    [SerializeField] private Sprite[] _progressIcons;
    public Sprite[] progressIcons => _progressIcons;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private int _maxAbilityPoint;
    public int maxAbilityPoint => _maxAbilityPoint;
    

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
