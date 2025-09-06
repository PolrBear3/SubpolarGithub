using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "New ScriptableObject/ Switch")]
public class Switch_SrcObj : ScriptableObject
{
    [Space(20)]
    [SerializeField] private string _switchName;
    public string switchName => _switchName;

    [SerializeField] private LocalizedString _localizedString;
    
    
    [Space(20)]
    [SerializeField] private Sprite _icon;
    public Sprite icon => _icon;
    
    public Color switchColor;


    [Space(20)] 
    [SerializeField] [Range(0, 1000)] private int _toggleCount;
    public int toggleCount => _toggleCount;
    
    [SerializeField] [Range(0, 1000)] private int _price;
    public int price => _price;

    [Space(10)] 
    [SerializeField] [Range(0, 100)] private float _toggleCooltime;
    public float toggleCooltime => _toggleCooltime;
}
