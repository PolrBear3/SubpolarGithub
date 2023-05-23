using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag_Slot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Text _amountText;

    [SerializeField] private Inventory_Controller _inventoryController;

    private Item_ScrObj _currentItem;

    private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    public void Drag_Item(Item_ScrObj item, int amount)
    {

    }

    public void Drop_Item()
    {

    }
}
