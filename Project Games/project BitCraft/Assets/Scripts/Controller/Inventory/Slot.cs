using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Text _amountText;
    
    private Inventory_Controller _inventory;
    public Inventory_Controller inventory { get => _inventory; set => _inventory = value; }

    private Item_ScrObj _currentItem;

    private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    // check system
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

    // script component connection
    public void Set_Inventory_Controller(Inventory_Controller inventory)
    {
        _inventory = inventory;
    }

    // functions
    private void Calculate_LeftOver()
    {
        if (_currentAmount > _currentItem.maxAmount)
        {
            int leftOver = _currentAmount - _currentItem.maxAmount;
            _currentAmount = _currentItem.maxAmount;
            _inventory.Add_Item(_currentItem, leftOver);
        }
    }

    public void Assign(Item_ScrObj item, int amount)
    {
        _currentItem = item;
        _currentAmount = amount;

        Calculate_LeftOver();

        _itemImage.sprite = item.sprite;
        _itemImage.color = Color.white;

        _amountText.text = _currentAmount.ToString();
        Color textColor = _amountText.color;
        textColor.a = 1f;
        _amountText.color = textColor;
    }
    public void Clear()
    {
        _currentItem = null;
        _currentAmount = 0;

        _itemImage.sprite = null;
        _itemImage.color = Color.clear;

        Color textColor = _amountText.color;
        textColor.a = 0f;
        _amountText.color = textColor;
    }

    public void Increase_Amount(int amount)
    {
        _currentAmount += amount;

        Calculate_LeftOver();

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