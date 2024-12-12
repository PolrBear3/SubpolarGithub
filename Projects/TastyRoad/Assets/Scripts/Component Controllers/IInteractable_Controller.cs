using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IInteractable_Controller : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    public Action OnInteract;
    public Action OnHoldInteract;
    public Action OnUnInteract;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


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
