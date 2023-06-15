using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable
{
    private Prefab_Controller _controller;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }
    public void Interact()
    {
        Rest();
    }

    // Functions
    private void Rest()
    {
        
    }
}
