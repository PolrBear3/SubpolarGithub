using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop_ItemData
{
    public Item_ScrObj item;
    public int minDropAmount;
    public int maxDropAmount;
}

[System.Serializable]
public class Current_ItemData
{
    public Item_ScrObj item;
    public int maxAmount;
    public int currentAmount;

    [Range(0, 100)]
    public int regenerateAmount;

    [Range(0, 100)]
    public float regeneratePercentage;
}

public class Basic_Object : MonoBehaviour, IInteractable, IDamageable, IInteractableUpdate
{
    private Prefab_Controller _controller;

    [SerializeField] private List<Drop_ItemData> _interactDrops = new List<Drop_ItemData>();
    [SerializeField] private List<Drop_ItemData> _removedDrops = new List<Drop_ItemData>();
    [SerializeField] private List<Current_ItemData> _currentItemData = new List<Current_ItemData>();

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }
    private void Start()
    {
        Set_Current_ItemData_Amount();
    }

    public void Interact()
    {
        Interact_ItemDrop();
    }
    public void Damage(int damageAmount)
    {
        _controller.statController.Update_Current_LifeCount(-damageAmount);
        Health_Check();
    }
    public void Interact_Update()
    {
        Regenerate_ItemDrop();
    }

    // Set
    private void Set_Current_ItemData_Amount()
    {
        for (int i = 0; i < _currentItemData.Count; i++)
        {
            _currentItemData[i].currentAmount = _currentItemData[i].maxAmount;
        }
    }

    // Get
    private Current_ItemData Current_Item(Item_ScrObj targetItem)
    {
        for (int i = 0; i < _currentItemData.Count; i++)
        {
            if (_currentItemData[i].item != targetItem) continue;
            return _currentItemData[i];
        }
        return null;
    }
    private Drop_ItemData RemovedDrop_Item(Item_ScrObj targetItem)
    {
        for (int i = 0; i < _removedDrops.Count; i++)
        {
            if (_removedDrops[i].item != targetItem) continue;
            return _removedDrops[i];
        }
        return null;
    }
    public bool Percentage_Successful(float percentage)
    {
        var value = (100 - percentage) * 0.01f;
        if (Random.value > value)
        {
            return true;
        }
        else return false;
    }

    // Functions
    private void Health_Check()
    {
        if (_controller.statController.currentLifeCount > 0) return;

        Removed_ItemDrop();
        Destroy(gameObject);
    }

    private void Interact_ItemDrop()
    {
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;

        for (int i = 0; i < _interactDrops.Count; i++)
        {
            Current_ItemData targetItem = Current_Item(_interactDrops[i].item);

            if (targetItem.currentAmount <= 0) return;

            int minAmount = _interactDrops[i].minDropAmount;
            int maxAmount = _interactDrops[i].maxDropAmount;

            if (targetItem.currentAmount < minAmount)
            {
                minAmount = targetItem.currentAmount;
                maxAmount = targetItem.currentAmount;
            }

            int dropAmount = Random.Range(minAmount, maxAmount);

            targetItem.currentAmount -= dropAmount;
            inventory.Add_Item(_interactDrops[i].item, dropAmount);
        }
    }
    private void Removed_ItemDrop()
    {
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;

        for (int i = 0; i < _removedDrops.Count; i++)
        {
            Current_ItemData targetItem = Current_Item(_removedDrops[i].item);

            if (targetItem.currentAmount <= 0) return;

            int minAmount = _removedDrops[i].minDropAmount;
            int maxAmount = _removedDrops[i].maxDropAmount;

            if (targetItem.currentAmount < minAmount)
            {
                minAmount = targetItem.currentAmount;
                maxAmount = targetItem.currentAmount;
            }

            int dropAmount = Random.Range(minAmount, maxAmount);

            targetItem.currentAmount -= dropAmount;
            inventory.Add_Item(_removedDrops[i].item, dropAmount);
        }
    }

    private void Regenerate_ItemDrop()
    {
        for (int i = 0; i < _currentItemData.Count; i++)
        {
            if (!Percentage_Successful(_currentItemData[i].regeneratePercentage)) continue;

            int amount = Random.Range(0, _currentItemData[i].regenerateAmount);
            _currentItemData[i].currentAmount += amount;

            if (_currentItemData[i].currentAmount <= _currentItemData[i].maxAmount) continue;
            _currentItemData[i].currentAmount = _currentItemData[i].maxAmount;
        }
    }
}
