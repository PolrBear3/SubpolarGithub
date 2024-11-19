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

    public event Event InteractEvent;
    public event Event UnInteractEvent;

    public Action OnHoldInteract;

    public event Event OnInteractEvent;
    public event Event OnAction1Event;
    public event Event OnAction2Event;


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
        OnInteractEvent?.Invoke();
    }

    private void OnAction1()
    {
        OnAction1Event?.Invoke();

        if (_unInteractLocked) return;

        UnInteract();
    }

    private void OnAction2()
    {
        if (OnAction2Event == null) return;
        OnAction2Event?.Invoke();

        if (_unInteractLocked) return;

        UnInteract();
    }


    public void Clear_ActionSubscriptions()
    {
        OnAction1Event = null;
        OnAction2Event = null;
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
        InteractEvent?.Invoke();
    }

    public void Hold_Interact()
    {
        OnHoldInteract?.Invoke();
    }

    public void UnInteract()
    {
        if (_unInteractLocked) return;

        if (_bubble == null) return;

        // bubble off
        InputToggle(false);
        _bubble.Toggle(false);

        //
        UnInteractEvent?.Invoke();
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
