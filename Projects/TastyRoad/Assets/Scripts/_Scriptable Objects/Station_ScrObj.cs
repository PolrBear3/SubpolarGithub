using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Station!")]
public class Station_ScrObj : ScriptableObject
{
    [Space(20)]
    public string stationName;
    [SerializeField] private LocalizedString _localizedString;
    
    [Space(10)]
    public int id;
    
    [Space(20)]
    public GameObject prefab;
    public Guide_ScrObj usageGuide;

    [Space(20)]
    public Sprite sprite;
    public Sprite miniSprite;
    public Sprite dialogIcon;

    [Space(20)]
    [Range(0, 1000)] public int price;
    [Range(0, 100)] public int buildToArchiveCount;
    [Range(0, 100)] public int durability;

    [Space(10)] 
    [SerializeField] private StationData[] _linkedStationDatas;
    public StationData[] linkedStationDatas => _linkedStationDatas;
    
    
    // Main
    public string LocalizedName()
    {
        if (_localizedString == null) return stationName;
        if (string.IsNullOrEmpty(_localizedString.TableReference) && string.IsNullOrEmpty(_localizedString.TableEntryReference)) return stationName;
        
        return _localizedString.GetLocalizedString();
    }
}