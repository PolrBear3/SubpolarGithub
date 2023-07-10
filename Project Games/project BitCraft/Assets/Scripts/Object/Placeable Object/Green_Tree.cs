  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green_Tree : MonoBehaviour, IInteractable, IDamageable
{
    private Prefab_Controller _controller;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }

    public void Interact()
    {
        Drop_Leaf();
    }
    public void Damage(int damageAmount)
    {
        Get_Damaged(damageAmount);
        Health_Check();
    }

    // Function
    private void Get_Damaged(int damageAmount)
    {
        _controller.healthController.Subtract_Current_LifeCount(damageAmount);
    }
    private void Health_Check()
    {
        if (_controller.healthController.currentLifeCount > 0) return;
        Drop_Item();
        Destroy(gameObject);
    }

    private void Drop_Item()
    {
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        int itemAmount = Random.Range(3, 5);

        inventory.Add_Item(2100, itemAmount);                 
    }
    private void Drop_Leaf()
    {
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        int itemAmount = Random.Range(0, 2);
        
        inventory.Add_Item(1101, itemAmount);
    }

    // Visual
    private void Cut_Animation()
    {
        
    }
}