using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionBubble_Interactable : MonoBehaviour, IInteractable
{
    [Header("")]
    [SerializeField] private PlayerInput _input;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private Action_Bubble _bubble;
    public Action_Bubble bubble => _bubble;

    private bool _interactLocked;
    public bool interactLocked => _interactLocked;

    private bool _unInteractLocked;
    public bool unInteractLocked => _unInteractLocked;

    public delegate void Event();

    public Action OnHoldIInteract;
    public event Event OnIInteract;
    public event Event OnUnIInteract;

    public event Event OnInteractInput;
    public event Event OnAction1Input;
    public event Event OnAction2Input;


    // MonoBehaviour
    public void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    public void Start()
    {
        UnInteract();

        _detection.ExitEvent += UnInteract;
    }

    private void OnDestroy()
    {
        _detection.ExitEvent += UnInteract;
    }


    // InputSystem
    private void OnInteract()
    {
        OnInteractInput?.Invoke();
    }

    private void OnAction1()
    {
        OnAction1Input?.Invoke();

        if (_unInteractLocked) return;

        UnInteract();
    }

    private void OnAction2()
    {
        if (OnAction2Input == null) return;
        OnAction2Input?.Invoke();

        if (_unInteractLocked) return;

        UnInteract();
    }


    public void Clear_ActionSubscriptions()
    {
        OnHoldIInteract = null;
        OnIInteract = null;
        OnUnIInteract = null;

        OnInteractInput = null;
        OnAction1Input = null;
        OnAction2Input = null;
    }


    // IInteractable
    public void Interact()
    {
        if (_interactLocked) return;

        // bubble empty
        if (_bubble == null) return;

        // bubble off
        if (_bubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        // bubble on
        InputToggle(true);
        _bubble.Toggle(true);

        //
        OnIInteract?.Invoke();
    }

    public void Hold_Interact()
    {
        OnHoldIInteract?.Invoke();
    }

    public void UnInteract()
    {
        if (_unInteractLocked) return;

        if (_bubble == null) return;

        // bubble off
        InputToggle(false);
        _bubble.Toggle(false);

        //
        OnUnIInteract?.Invoke();
    }


    //
    public void InputToggle(bool toggleOn)
    {
        _input.enabled = toggleOn;
    }


    public void LockInteract(bool toggleLock)
    {
        _interactLocked = toggleLock;
    }

    public void LockUnInteract(bool toggleLock)
    {
        _unInteractLocked = toggleLock;
    }


    public void Refresh()
    {
        LockInteract(false);
        LockUnInteract(false);

        UnInteract();
    }
}
