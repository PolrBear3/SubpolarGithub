using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct VideoClip_Data
{
    [SerializeField] private VideoClip _video;
    public VideoClip video => _video;

    [TextArea]
    [SerializeField] private string _info;
    public string info => _info;
}

[CreateAssetMenu(menuName = "New ScriptableObject/ New Guide!")]
public class Guide_ScrObj : ScriptableObject
{
    [Header("")]
    [SerializeField] private string _guideName;
    public string guideName => _guideName;

    [Header("")]
    [SerializeField] private VideoClip_Data[] _clipDatas;
    public VideoClip_Data[] clipDatas => _clipDatas;
}
