using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using TMPro;

public class Input_Controller : MonoBehaviour
{
    public static Input_Controller instance;

    
    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput playerInput => _playerInput;

    [Space(20)]
    [SerializeField][Range(0, 100)] private float _holdTime;

    
    private bool _isHolding;
    public bool isHolding => _isHolding;

    private float _currentHoldTime;
    public float currentHoldTime => _currentHoldTime;


    public Action OnClick;
    public Action OnExit;
    

    // MonoBehaviour
    private void Awake()
    {
        if (instance != null) return;
        instance = this;
    }
    

    // Invoke
    public void Click(InputAction.CallbackContext context)
    {
        if (_isHolding) return;
        if (context.performed == false) return;
        
        OnClick?.Invoke();
    }

    public void Exit(InputAction.CallbackContext context)
    {
        if (_isHolding) return;
        if (context.performed == false) return;
        
        OnExit?.Invoke();
    }
}