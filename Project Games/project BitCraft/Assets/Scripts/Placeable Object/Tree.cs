using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IInteractable
{
    private Prefab_Controller controller;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { this.controller = controller; }
    }
    public void Interact()
    {
        Cut();
    }

    private void Cut()
    {

    }

    private void Get_Damage()
    {

    }

    private void Cut_Animation()
    {

    }
}