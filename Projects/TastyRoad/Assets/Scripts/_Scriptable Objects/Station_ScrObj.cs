using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Station!")]
public class Station_ScrObj : ScriptableObject
{
    [Header("")]
    public GameObject prefab;
    public Guide_ScrObj usageGuide;

    [Header("")]
    public Sprite sprite;
    public Sprite miniSprite;
    public Sprite dialogIcon;

    [Header("")]
    public string stationName;
    [SerializeField] private LocalizedString _localizedString;
    
    [Space(20)]
    public int id;

    [Header("")]
    [Range(0, 1000)] public int price;
    [Range(0, 100)] public int buildToArchiveCount;

    [Header("")]
    [Range(0, 100)] public int durability;
    
    
    public string LocalizedName()
    {
        if (_localizedString == null) return stationName;
        if (string.IsNullOrEmpty(_localizedString.TableReference) && string.IsNullOrEmpty(_localizedString.TableEntryReference)) return stationName;
        
        return _localizedString.GetLocalizedString();
    }
}