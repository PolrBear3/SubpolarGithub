using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Ability!")]
public class Ability_ScrObj : ScriptableObject
{
    [Header("")]
    [SerializeField] private string _abilityName;
    public string abilityName => _abilityName;

    [SerializeField][TextArea(2, 2)] private string _description;
    public string description => _description;

    [Header("")]
    [SerializeField] private Sprite[] _progressIcons;
    public Sprite[] progressIcons => _progressIcons;


    public Sprite ProgressIcon(int levelValue)
    {
        return progressIcons[Mathf.Clamp(levelValue, 0, _progressIcons.Length - 1)];
    }
}
