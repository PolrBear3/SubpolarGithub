using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green_Tree : MonoBehaviour, IInteractable
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
        Get_Damage();
        Cut_Animation();
        if (controller.healthController.currentLifeCount > 0) return;
        Destroy(gameObject);
    }

    private void Get_Damage()
    {
        controller.healthController.Subtract_Current_LifeCount(1);
    }

    private void Cut_Animation()
    {
        Debug.Log(controller.healthController.currentLifeCount);
    }
}