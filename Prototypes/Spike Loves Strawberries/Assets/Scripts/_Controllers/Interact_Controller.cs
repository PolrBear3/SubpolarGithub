using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Controller : MonoBehaviour, IInteractable
{
    [Space(20)]
    [SerializeField] private SpriteRenderer _indicationSR;
    public SpriteRenderer indicationSR => _indicationSR;

    
    public Action OnInteract;
    public Action OnInteractRelease;
    
    
    // IInteractable
    public void Interact()
    {
        OnInteract?.Invoke();
    }
    
    
    // Indication
    public void Toggle_Indication(bool toggle)
    {
        float alphaValue = toggle ? 1f : 0f;
        _indicationSR.material.SetFloat("_OutlineAlpha", alphaValue);
    }
}