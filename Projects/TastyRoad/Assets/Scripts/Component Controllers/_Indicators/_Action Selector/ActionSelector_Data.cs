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

    [Header("")]
    [SerializeField] private UnityEvent _actionEvent = new();
    public UnityEvent actionEvent => _actionEvent;

    public ActionSelector_Data(Sprite actionSprite, Action action)
    {
        _actionSprite = actionSprite;
        _actionEvent.AddListener(() => action());
    }
}
