using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IInteractable_Controller : MonoBehaviour, IInteractable
{
    public Action OnInteract;
    public Action OnHoldInteract;
    public Action OnUnInteract;


    // IInteractable
    public void Interact()
    {
        OnInteract?.Invoke();
    }

    public void Hold_Interact()
    {
        OnHoldInteract?.Invoke();
    }

    public void UnInteract()
    {
        OnUnInteract?.Invoke();
    }
}
