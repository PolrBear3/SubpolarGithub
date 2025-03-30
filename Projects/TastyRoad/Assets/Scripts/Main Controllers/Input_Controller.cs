using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Controller : MonoBehaviour
{
    public static Input_Controller instance;


    [Header("")]
    [SerializeField] private PlayerInput _playerInput;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _holdTime;

    private float _currentHoldTime;
    public float currentHoldTime => _currentHoldTime;


    private Vector2 _inputDirection;
    public Vector2 inputDirection => _inputDirection;

    private bool _isHolding;
    public bool isHolding => _isHolding;

    private bool _action1Pressed;
    public bool action1Pressed => _action1Pressed;

    private bool _action2Pressed;
    public bool action2Pressed => _action2Pressed;


    public Action<Vector2> OnMovement;

    public Action OnInteractStart;
    public Action OnInteract;
    public Action OnHoldInteract;

    public Action OnAction1;
    public Action OnAction2;


    // MonoBehaviour
    private void Awake()
    {
        Set_Instance();
    }


    // Set Data
    private void Set_Instance()
    {
        if (instance != null) return;

        instance = this;
    }


    // Contexts
    public void Movement(InputAction.CallbackContext context)
    {
        Vector2 directionInput = context.ReadValue<Vector2>();

        OnMovement?.Invoke(directionInput);
        _inputDirection = directionInput;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHolding = true;
            _currentHoldTime = Time.time;

            OnInteractStart?.Invoke();
            return;
        }

        if (context.canceled == false) return;

        _isHolding = false;

        if (Time.time - _currentHoldTime >= _holdTime)
        {
            OnHoldInteract?.Invoke();
            return;
        }

        OnInteract?.Invoke();
    }

    public void Action1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _action1Pressed = true;
        }
        else if (context.canceled)
        {
            _action1Pressed = false;
        }

        if (context.performed == false) return;
        OnAction1?.Invoke();
    }

    public void Action2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _action2Pressed = true;
        }
        else if (context.canceled)
        {
            _action2Pressed = false;
        }

        if (context.performed == false) return;
        OnAction2?.Invoke();
    }
}
