using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green_Tree : MonoBehaviour, IInteractable
{
    private Prefab_Controller _controller;

    [SerializeField] private Item_ScrObj item;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { this._controller = controller; }
    }
    public void Interact()
    {
        Get_Damaged();
        Health_Check();
    }

    private void Get_Damaged()
    {
        _controller.healthController.Subtract_Current_LifeCount(1);
    }
    private void Give_Item()
    {
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        int itemAmount = Random.Range(3, 5);

        inventory.Add_Item(item, itemAmount);
    }
    
    private void Health_Check()
    {
        if (_controller.healthController.currentLifeCount > 0) return;
        Give_Item();
        Destroy(gameObject);
    }

    private void Cut_Animation()
    {
        
    }
}