using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spike_Data
{
    private bool _hasTail;
    public bool hasTail => _hasTail;

    private bool _hasItem;
    public bool hasItem => _hasItem;
    
    private int _damageCount;
    public int damageCount => _damageCount;
    
    private List<IInteractable> _detectedInteractables = new();
    public List<IInteractable> detectedInteractables => _detectedInteractables;
    
    private GameObject _currentInteractable;
    public GameObject currentInteractable => _currentInteractable;
    
    
    // New
    public Spike_Data(int maxDamageCount)
    {
        _hasTail = true;
        _damageCount = maxDamageCount;
    }
    
    
    // Data Control
    public void Toggle_HasTail(bool toggle)
    {
        _hasTail = toggle;
    }

    public void Set_DamageCount(int setValue)
    {
        _damageCount = setValue;
    }

    public void Set_CurrentInteractable(GameObject interactable)
    {
        _hasItem = interactable != null;
        _currentInteractable = interactable;
    }
}
