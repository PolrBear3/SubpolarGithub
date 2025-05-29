using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ActionSelector_Data
{
    [SerializeField] private Sprite _actionSprite;
    public Sprite actionSprite => _actionSprite;

    [Space(20)] 
    [SerializeField][Range(0, 100)] private float _activateValue;
    public float activateValue => _activateValue;

    [SerializeField] private UnityEvent _actionEvent = new();
    public UnityEvent actionEvent => _actionEvent;

    
    // New
    public ActionSelector_Data(Sprite actionSprite, Action action)
    {
        _actionSprite = actionSprite;
        _actionEvent.AddListener(() => action());
    }

    public ActionSelector_Data(Action action, float activateValue)
    {
        _activateValue = activateValue;
        _actionEvent.AddListener(() => action());
    }
}
