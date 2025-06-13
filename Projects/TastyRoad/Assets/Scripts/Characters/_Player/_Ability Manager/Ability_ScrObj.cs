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
    [SerializeField] private Sprite[] _activationIconSprite;
    public Sprite[] activationIconSprite => _activationIconSprite;
    

    // Gets
    public string Description()
    {
        if (_localizedDescription == null) return _description;
        if (string.IsNullOrEmpty(_localizedDescription.TableReference) && string.IsNullOrEmpty(_localizedDescription.TableEntryReference)) return _description;

        return _localizedDescription.GetLocalizedString();
    }

    public int Max_ActivationCount()
    {
        return _activationIconSprite.Length - 1;
    }
}