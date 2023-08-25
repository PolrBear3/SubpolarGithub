using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory_Controller : MonoBehaviour
{
    [SerializeField] private Game_Controller _controller;
    public Game_Controller controller { get => _controller; set => _controller = value; }

    [SerializeField] private Drag_Slot _dragSlot;
    public Drag_Slot dragSlot { get => _dragSlot; set => _dragSlot = value; }

    [SerializeField] private List<Slot> _slots = new List<Slot>();
    public List<Slot> slots { get => _slots; set => _slots = value; }

    private Slot _equippedSlot;
    public Slot equippedSlot { get => _equippedSlot; set => _equippedSlot = value; }

    //
    private void Start()
    {
        //
        Set_Slots();

        // inventory functions
        

        //
        Set_EquipSlot(0);
    }

    public void OnNext()
    {
        Set_EquipSlot(_equippedSlot.slotNum + 1);
    }
    public void OnBack()
    {
        Set_EquipSlot(_equippedSlot.slotNum - 1);
    }
    
    // Check
    public bool Is_Inventory_Full()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_slots[i].hasItem) return false;
        }
        return true;
    }
    public bool Is_Inventory_Full(Item_ScrObj item, int amount)
    {
        int amountCount = amount;

        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_slots[i].hasItem) return false;
            if (item != _slots[i].currentItem) continue;

            if (amount + _slots[i].currentAmount > item.maxAmount)
            {
                amountCount = amountCount + _slots[i].currentAmount - item.maxAmount;
                continue;
            }

            return false;
        }

        if (amountCount > 0) return true;
        return true;
    }
    public bool Is_Inventory_Full(int itemID, int amount)
    {
        return false;
    }

    // Get
    public Slot Empty_Slot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].hasItem) continue;
            return _slots[i];
        }
        return null;
    }

    // Set
    private void Set_Slots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].slotNum = i;
            _slots[i].Set_Inventory_Controller(this);
            _slots[i].Clear();
        }
    }

    // Equipment
    private void Set_EquipSlot(int slotNum)
    {
        int setSlotNum = slotNum;

        if (slotNum > _slots.Count - 1)
        {
            setSlotNum = 0;
        }
        else if (slotNum < 0)
        {
            setSlotNum = _slots.Count - 1;
        }

        if (_equippedSlot != null)
        {
            _equippedSlot.Equip(false);
        }

        _equippedSlot = _slots[setSlotNum];
        _equippedSlot.Equip(true);

        _controller.tilemapController.actionSystem.UnHighlight_All_EquipmentUseTiles();
        _controller.interactionController.Update_Equipment_Icon();
    }

    // Function
    public void Add_Item(Item_ScrObj item, int amount)
    {
        if (item == null || amount <= 0) return;
        
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].currentAmount <= 0)
            {
                _slots[i].Assign(item, amount);
                break;
            }

            if (_slots[i].Is_Max_Amount()) continue;
            if (!_slots[i].Is_Same_Item(item)) continue;

            _slots[i].Increase_Amount(amount);
            break;
        }
    }
    public void Add_Item(int itemID, int amount)
    {
        if (amount <= 0) return;

        Item_ScrObj item = controller.prefabsData.Get_Item(itemID);

        if (item == null)
        {
            Debug.Log("Invalid Item ID");
            return;
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].currentAmount <= 0)
            {
                _slots[i].Assign(item, amount);
                break;
            }

            if (_slots[i].Is_Max_Amount()) continue;
            if (!_slots[i].Is_Same_Item(item)) continue;

            _slots[i].Increase_Amount(amount);
            break;
        }
    }

    public void Decrease_EquippedSlot_Item(int amount)
    {
        _equippedSlot.Decrease_Amount(amount);
        _controller.interactionController.Update_Equipment_Icon();
    }
}