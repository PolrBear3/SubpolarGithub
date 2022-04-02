using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int stackSize;

    public ItemData ItemData => itemData;
    public int StackSize => stackSize;

    // containing data
    public InventorySlot(ItemData source, int amount)
    {
        itemData = source;
        stackSize = amount;
    }

    // clear slot
    public InventorySlot()
    {
        ClearSlot();
    }
    public void ClearSlot()
    {
        itemData = null;
        stackSize = -1;
    }

    // check if there is room in stack
    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.maxStackSize - stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    public bool RoomLeftInStack(int amountToAdd)
    {
        if (stackSize + amountToAdd <= itemData.maxStackSize) return true;
        else return false;
    }

    // add an remove from stack 
    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
    }
}
