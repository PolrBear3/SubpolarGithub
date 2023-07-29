using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green_Tree : MonoBehaviour, IInteractable, IDamageable
{
    private Prefab_Controller _controller;

    [SerializeField] private Item_ScrObj _additionalItem;

    [SerializeField] private int _leafAmount;
    [SerializeField] private int _additionalAmount;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }

    public void Interact()
    {
        Drop_Leaf(0, 2);
        Drop_Additional(0, 2);
    }
    public void Damage(int damageAmount)
    {
        Get_Damaged(damageAmount);
        Health_Check();
    }

    // Function
    private void Get_Damaged(int damageAmount)
    {
        _controller.statController.Update_Current_LifeCount(-damageAmount);
    }
    private void Health_Check()
    {
        if (_controller.statController.currentLifeCount > 0) return;

        Drop_Log(3, 5);
        Drop_Leaf(2, 5);
        Drop_Additional(2, 5);

        Destroy(gameObject);
    }

    private void Drop_Log(int minAmount, int maxAmount)
    {
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        int itemAmount = Random.Range(minAmount, maxAmount);

        inventory.Add_Item(364979, itemAmount);                 
    }
    private void Drop_Leaf(int minAmount, int maxAmount)
    {
        if (_leafAmount <= 0) return;

        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        int itemAmount = Random.Range(minAmount, maxAmount);

        _leafAmount -= itemAmount;
        inventory.Add_Item(677972, itemAmount);
    }
    private void Drop_Additional(int minAmount, int maxAmount)
    {
        if (_additionalItem == null) return;
        if (_additionalAmount <= 0) return;

        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        int itemAmount = Random.Range(minAmount, maxAmount);

        _additionalAmount -= itemAmount;
        inventory.Add_Item(_additionalItem, itemAmount);
    }

    // Visual
    private void Cut_Animation()
    {
        
    }
}