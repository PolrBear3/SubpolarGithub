using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Localization;
using UnityEngine.Serialization;

[System.Serializable]
public struct VideoClip_Data
{
    [SerializeField] private VideoClip _video;
    public VideoClip video => _video;

    [TextArea]
    [SerializeField] private string _info;
    public string info => _info;
    
    [FormerlySerializedAs("_localizedDescription")] [SerializeField] private LocalizedString _localizedInfo;
    public LocalizedString localizedInfo => _localizedInfo;

    public string Info()
    {
        if (_localizedInfo == null) return _info;
        if (string.IsNullOrEmpty(_localizedInfo.TableReference) && string.IsNullOrEmpty(_localizedInfo.TableEntryReference)) return _info;
        
        return _localizedInfo.GetLocalizedString();
    }
}

[CreateAssetMenu(menuName = "New ScriptableObject/ New Guide!")]
public class Guide_ScrObj : ScriptableObject
{
    [Header("")]
    [SerializeField] private string _guideName;
    public string guideName => _guideName;

    [SerializeField] private int _guideID;
    public int guideID => _guideID;

    [Header("")]
    [SerializeField] private VideoClip_Data[] _clipDatas;
    public VideoClip_Data[] clipDatas => _clipDatas;
}
