using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoldInput_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private PlayerInput _playerInput;


    [Header("")]
    [SerializeField] private string _holdKeyAction;
    [SerializeField][Range(0, 10)] private float _holdTime;


    public Action OnStart;
    public Action OnEnd;

    public Action OnComplete;
    public Action OnInComplete;


    private bool _onHold;
    public bool onHold => _onHold;

    private float _pressStartTime;


    // UnityEngine
    private void Awake()
    {
        _playerInput.actions[_holdKeyAction].started += ctx => OnHoldStart();
        _playerInput.actions[_holdKeyAction].canceled += ctx => OnHoldEnd();
    }


    // Press Control
    private void OnHoldStart()
    {
        _pressStartTime = Time.time;

        _onHold = true;
        OnStart?.Invoke();
    }

    private void OnHoldEnd()
    {
        _onHold = false;
        OnEnd?.Invoke();

        float holdDuration = Time.time - _pressStartTime;

        if (_playerInput.enabled == false) return;

        if (holdDuration < _holdTime)
        {
            OnInComplete?.Invoke();
            return;
        }

        OnComplete?.Invoke();
    }
}
