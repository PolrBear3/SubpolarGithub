using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Box_Slot
{
    [SerializeField] private Item_Info _itemInfo;
    [SerializeField] private int _currentAmount;
    
    public Item_Info itemInfo => _itemInfo;
    public int currentAmount => _currentAmount;

    // passing in item information and amount
    public Box_Slot(Item_Info itemInfo, int currentAmount)
    {
        _itemInfo = itemInfo;
        _currentAmount = currentAmount;
    }

    // item state of nothing
    public Box_Slot()
    {
        Clear_Slot();
    }

    // amount control
    public void Clear_Slot()
    {
        _itemInfo = null;
        _currentAmount = -1;
    }

    public void Add_to_Stack(int addAmount)
    {
        _currentAmount += addAmount;
    }

    public void Remove_from_Stack(int removeAmount)
    {
        _currentAmount -= removeAmount;
    }

    public void Update_Box_Slot(Item_Info info, int amount)
    {
        _itemInfo = info;
        _currentAmount = amount;
    }

    // amount check systems
    public bool Room_left_Stack(int amountToAdd, out int amountRemanining)
    {
        amountRemanining = _itemInfo.itemMaxAmount - _currentAmount;
        return Room_left_Stack(amountToAdd);
    }

    public bool Room_left_Stack(int amountToAdd)
    {
        if (_currentAmount + amountToAdd <= _itemInfo.itemMaxAmount) return true;
        else return false;
    }
}
