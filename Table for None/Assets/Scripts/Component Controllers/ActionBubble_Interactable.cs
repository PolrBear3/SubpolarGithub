using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionBubble_Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private Action_Bubble _bubble;
    public Action_Bubble bubble => _bubble;


    [Space(20)]
    [SerializeField] private bool _interactLocked;
    public bool interactLocked => _interactLocked;

    [SerializeField] private bool _unInteractLocked;
    public bool unInteractLocked => _unInteractLocked;


    public Action OnTriggerInteract;
    
    public Action OnInteract;
    public Action OnHoldInteract;

    public Action OnUnInteract;

    public Action OnAction1;
    public Action OnAction2;

    private bool _actionsSubscribed;


    // MonoBehaviour
    public void Start()
    {
        UnInteract();

        // subscriptions
        _detection.ExitEvent += UnInteract;
    }

    private void OnDestroy()
    {
        // subscriptions
        OnInteract = null;
        OnHoldInteract = null;
        OnUnInteract = null;
        OnAction1 = null;
        OnAction2 = null;

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 -= Invoke_Action1;
        input.OnAction2 -= Invoke_Action2;
    }


    // IInteractable
    public void Trigger_Interact()
    {
        OnTriggerInteract.Invoke();
    }
    
    public void Interact()
    {
        if (_interactLocked || _bubble == null) return;

        if (_bubble.bubbleOn)
        {
            // bubble off
            UnInteract();
            return;
        }

        _bubble.Toggle(true);
        if (bubble.bubbleOn == false) return;
        
        OnInteract?.Invoke();

        if (_actionsSubscribed) return;

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 += Invoke_Action1;
        input.OnAction2 += Invoke_Action2;

        _actionsSubscribed = true;
    }

    public void Hold_Interact()
    {
        if (_interactLocked) return;

        OnHoldInteract?.Invoke();
    }

    public void Action1()
    {
        
    }
    
    public void Action2()
    {
        
    }

    public void UnInteract()
    {
        if (_unInteractLocked || _bubble == null) return;

        _bubble.Toggle(false);
        OnUnInteract?.Invoke();

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 -= Invoke_Action1;
        input.OnAction2 -= Invoke_Action2;

        _actionsSubscribed = false;
    }


    // Input_Controller
    public void Clear_ActionSubscriptions()
    {
        OnInteract = null;
        OnHoldInteract = null;

        OnUnInteract = null;

        OnAction1 = null;
        OnAction2 = null;

        UnInteract();
    }


    private void Invoke_Action1()
    {
        if (_interactLocked) return;

        OnAction1?.Invoke();
        UnInteract();
    }

    private void Invoke_Action2()
    {
        if (_interactLocked || OnAction2 == null) return;

        OnAction2?.Invoke();
        UnInteract();
    }


    // Toggle
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
