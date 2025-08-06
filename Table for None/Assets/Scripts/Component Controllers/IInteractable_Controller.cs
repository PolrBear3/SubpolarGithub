using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IInteractable_Controller : MonoBehaviour, IInteractable
{
    [Space(20)]
    [SerializeField] private UnityEvent OnInteract_Event;

    public Action OnTriggerInteract;
    
    public Action OnInteract;
    public Action OnHoldInteract;

    public Action OnAction1;
    public Action OnAction2;
    
    public Action OnUnInteract;
    
    private bool _interactLocked;
    public bool interactLocked => _interactLocked;


    // IInteractable
    public void Trigger_Interact()
    {
        OnTriggerInteract.Invoke();
    }
    
    public void Interact()
    {
        if (_interactLocked) return;

        OnInteract?.Invoke();
        OnInteract_Event?.Invoke();
    }
    
    public void Hold_Interact()
    {
        if (_interactLocked) return;

        OnHoldInteract?.Invoke();
    }

    public void Action1()
    {
        if (_interactLocked) return;
        
        OnAction1?.Invoke();
    }

    public void Action2()
    {
        if (_interactLocked) return;
        
        OnAction2?.Invoke();
    }

    public void UnInteract()
    {
        if (_interactLocked) return;

        OnUnInteract?.Invoke();
    }


    // Toggles
    public void Toggle_Lock(bool toggle)
    {
        _interactLocked = toggle;
    }
}
