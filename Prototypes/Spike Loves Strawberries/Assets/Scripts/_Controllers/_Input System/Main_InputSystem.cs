using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Main_InputSystem : MonoBehaviour
{
    public static Main_InputSystem instance;

    
    [Space(20)]
    [SerializeField] private PlayerInput _playerInput;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _holdTime;
    
    
    private Vector2 _movementDirection;
    public Vector2 movementDirection => _movementDirection;
    
    public Action<Vector2> OnMovementInput;
    
    public Action OnInteractInput;
    public Action OnInteractRelease;

    public Action OnCancelInput;
    public Action OnHoldCancelInput;
    
    private Coroutine _cancelCoroutine;


    private void Awake()
    {
        instance = this;
    }
    
    
    public void Movement(InputAction.CallbackContext context)
    {
        _movementDirection = Vector2.zero;

        Vector2 directionInput = context.ReadValue<Vector2>();

        OnMovementInput?.Invoke(directionInput);
        _movementDirection = directionInput;
    }
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInteractInput?.Invoke();
        }
        else if (context.canceled)
        {
            OnInteractRelease?.Invoke();
        }
    }

    
    public void Cancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_cancelCoroutine != null)
            {
                StopCoroutine(_cancelCoroutine);
            }
            _cancelCoroutine = StartCoroutine(HoldCancel_Coroutine());
        }
        else if (context.canceled)
        {
            if (_cancelCoroutine != null)
            {
                StopCoroutine(_cancelCoroutine);
                
                _cancelCoroutine = null;
                OnCancelInput?.Invoke();
            }
        }
    }

    private IEnumerator HoldCancel_Coroutine()
    {
        float elapsed = 0f;

        while (elapsed < _holdTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        OnHoldCancelInput?.Invoke();
        _cancelCoroutine = null;
    }
}