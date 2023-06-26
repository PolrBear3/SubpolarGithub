using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_CraftTable : MonoBehaviour, IInteractable, IInteractableUpdate
{
    private Prefab_Controller _controller;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }
    public void Interact()
    {
        Craft();
    }
    public void Interact_Update()
    {
        Insert_Ingredient();
    }

    // Functions
    private void Insert_Ingredient()
    {
        
    }

    private void Craft()
    {

    }
}
