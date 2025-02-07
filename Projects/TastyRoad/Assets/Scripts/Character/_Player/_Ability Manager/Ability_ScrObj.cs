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

    [Header("")]
    [SerializeField][Range(0, 100)] private int _maxActivationCount;
    public int maxActivationCount => _maxActivationCount;


    public Sprite ProgressIcon(int levelValue)
    {
        levelValue = Mathf.Clamp(levelValue, 0, maxActivationCount);
        int spriteIndex = Mathf.FloorToInt((float)levelValue / maxActivationCount * (progressIcons.Length - 1));

        return _progressIcons[spriteIndex];
    }
}
