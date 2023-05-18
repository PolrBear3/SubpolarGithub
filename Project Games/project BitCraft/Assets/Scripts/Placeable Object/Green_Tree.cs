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
        Get_Damaged();
        Health_Check();
    }

    private void Health_Check()
    {
        if (controller.healthController.currentLifeCount > 0) return;
        Destroy(gameObject);
    }

    private void Get_Damaged()
    {
        controller.healthController.Subtract_Current_LifeCount(1);
    }

    private void Cut_Animation()
    {
        
    }
}