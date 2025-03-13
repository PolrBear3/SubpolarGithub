using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoldSystem_TriggerData
{
    [SerializeField] private Sprite _triggerIcon;
    public Sprite triggerIcon => _triggerIcon;

    [SerializeField] private int _triggerValue;
    public int triggerValue => _triggerValue;


    public GoldSystem_TriggerData(Sprite icon, int value)
    {
        _triggerIcon = icon;
        _triggerValue = value;
    }
}
