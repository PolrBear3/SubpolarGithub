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
    
    
    private Vector2 _movementDirection;
    public Vector2 movementDirection => _movementDirection;
    
    public Action<Vector2> OnMovementInput;
    
    public Action OnInteractInput;
    public Action OnInteractRelease;


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
}