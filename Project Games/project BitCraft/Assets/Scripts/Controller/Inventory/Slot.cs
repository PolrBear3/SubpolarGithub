using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Text _amountText;
    [SerializeField] private GameObject _equipCursor;
    
    private Inventory_Controller _inventory;
    public Inventory_Controller inventory { get => _inventory; set => _inventory = value; }

    private Item_ScrObj _currentItem;
    public Item_ScrObj currentItem { get => _currentItem; set => _currentItem = value; }

    private int _slotNum;
    public int slotNum { get => _slotNum; set => _slotNum = value; }

    [SerializeField] private bool _hasItem;
    public bool hasItem { get => _hasItem; set => _hasItem = value; }

    private bool _equipped;
    public bool equipped { get => _equipped; set => _equipped = value; }

    private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    //
    public void OnPointerEnter(PointerEventData eventData)
    {
        _inventory.dragSlot.slotDetected = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _inventory.dragSlot.slotDetected = false;
    }

    // Check System
    public bool Is_Same_Item(Item_ScrObj item)
    {
        if (_currentItem == null) return false;
        if (item != _currentItem) return false;
        return true;
    }
    public bool Is_Max_Amount()
    {
        if (_currentItem == null) return false;
        if (_currentAmount < _currentItem.maxAmount) return false;
        return true;
    }

    // Setup
    public void Set_Inventory_Controller(Inventory_Controller inventory)
    {
        _inventory = inventory;
    }

    // Functions
    public void DragDrop_Item()
    {
        Drag_Slot dragSlot = _inventory.dragSlot;

        // Prevent Empty Slot Click
        if (!dragSlot.itemDragging && !_hasItem) return;

        // Drag
        if (!dragSlot.itemDragging)
        {
            dragSlot.Assign(_currentItem, _currentAmount);
            Clear();

            _inventory.controller.interactionController.Update_Equipment_Icon();
        }
        // Drop
        else
        {
            // Drop - Empty Slot
            if (_currentItem == null)
            {
                Assign(dragSlot.currentItem, dragSlot.currentAmount);
                dragSlot.Clear();

                _inventory.controller.interactionController.Update_Equipment_Icon();
            }
            // Drop - Non-Empty Slot
            else
            {
                // Increase Item Amount
                if (dragSlot.Is_Same_Item(_currentItem) && !Is_Max_Amount())
                {
                    int increaseAmount = _currentAmount + dragSlot.currentAmount;

                    // Over Max Amount
                    if (increaseAmount > _currentItem.maxAmount)
                    {
                        int leftOver = increaseAmount - _currentItem.maxAmount;

                        dragSlot.Assign(_currentItem, leftOver);
                        Assign(_currentItem, _currentItem.maxAmount);
                    }
                    // Not Max Amount
                    else
                    {
                        Increase_Amount(dragSlot.currentAmount);
                        dragSlot.Clear();
                    }
                }
                // Swap Items
                else
                {
                    Item_ScrObj savedItem = _currentItem;
                    int savedAmount = _currentAmount;

                    Assign(dragSlot.currentItem, dragSlot.currentAmount);
                    dragSlot.Assign(savedItem, savedAmount);

                    _inventory.controller.interactionController.Update_Equipment_Icon();
                }
            }
        }
    }

    public void Assign(Item_ScrObj item, int amount)
    {
        _hasItem = true;

        _currentItem = item;
        _currentAmount = amount;

        if (_currentAmount > _currentItem.maxAmount)
        {
            int leftOver = _currentAmount - _currentItem.maxAmount;
            _currentAmount = _currentItem.maxAmount;
            _inventory.Add_Item(_currentItem, leftOver);
        }

        _itemImage.sprite = item.sprite;
        _itemImage.color = Color.white;

        _amountText.text = _currentAmount.ToString();
        Color textColor = _amountText.color;
        textColor.a = 1f;
        _amountText.color = textColor;
    }
    public void Clear()
    {
        _hasItem = false;

        _currentItem = null;
        _currentAmount = 0;

        _itemImage.sprite = null;
        _itemImage.color = Color.clear;

        Color textColor = _amountText.color;
        textColor.a = 0f;
        _amountText.color = textColor;
    }

    public void Equip(bool equipped)
    {
        _equipped = equipped;
        _equipCursor.SetActive(equipped);
    }

    public void Increase_Amount(int amount)
    {
        _currentAmount += amount;

        if (_currentAmount > _currentItem.maxAmount)
        {
            int leftOver = _currentAmount - _currentItem.maxAmount;
            _currentAmount = _currentItem.maxAmount;
            _inventory.Add_Item(_currentItem, leftOver);
        }

        _amountText.text = _currentAmount.ToString();
    }
    public void Decrease_Amount(int amount)
    {
        _currentAmount -= amount;
        _amountText.text = _currentAmount.ToString();

        if (_currentAmount > 0) return;
        Clear();
    }
}