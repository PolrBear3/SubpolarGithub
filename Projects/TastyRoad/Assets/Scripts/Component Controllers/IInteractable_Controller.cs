using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IInteractable_Controller : MonoBehaviour, IInteractable
{
    public Action OnInteract;
    public Action OnHoldInteract;
    public Action OnUnInteract;


    private bool _interactLocked;


    // IInteractable
    public void Interact()
    {
        if (_interactLocked) return;

        OnInteract?.Invoke();
    }

    public void Hold_Interact()
    {
        if (_interactLocked) return;

        OnHoldInteract?.Invoke();
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
