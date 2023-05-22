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

    // script component connection
    public void Set_Inventory_Controller(Inventory_Controller inventory)
    {
        _inventory = inventory;
    }

    // functions
    public void Assign(Item_ScrObj item, int amount)
    {
        _currentItem = item;
        _currentAmount = amount;

        _itemImage.sprite = item.sprite;
        _itemImage.color = Color.white;

        _amountText.text = amount.ToString();
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
}