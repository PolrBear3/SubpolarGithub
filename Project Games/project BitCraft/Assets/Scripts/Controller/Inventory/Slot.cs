using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    
    private Inventory_Controller _inventory;
    public Inventory_Controller inventory { get => _inventory; set => _inventory = value; }

    private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    public void Set_Inventory_Controller(Inventory_Controller inventory)
    {
        _inventory = inventory;
    }

    public void Assign(Item_ScrObj item)
    {
        itemImage.sprite = item.sprite;
        itemImage.color = Color.white;
    }
    public void Clear()
    {
        itemImage.color = Color.clear;
    }
}