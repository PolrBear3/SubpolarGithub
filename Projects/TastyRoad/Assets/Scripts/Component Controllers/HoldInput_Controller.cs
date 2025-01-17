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

    public Action OnHoldComplete;
    public Action OnHoldInComplete;


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
    }

    private void OnHoldEnd()
    {
        _onHold = false;

        float holdDuration = Time.time - _pressStartTime;

        if (_playerInput.enabled == false) return;

        if (holdDuration < _holdTime)
        {
            OnHoldInComplete?.Invoke();
            return;
        }

        OnHoldComplete?.Invoke();
    }
}
