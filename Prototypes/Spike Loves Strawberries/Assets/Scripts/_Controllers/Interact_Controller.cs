using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Controller : MonoBehaviour, IInteractable
{
    public Action OnInteract;
    
    
    // IInteractable
    public void Interact()
    {
        OnInteract?.Invoke();
    }
}
