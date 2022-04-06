using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private void Update()
    {
        InventorySlot_Empty_Check();
    }

    public Item_Slot currentSlot, backSlot, throwable1Slot, throwable2Slot;
    public static bool currentSlotEmpty, backSlotEmpty, throwable1Empty, throwable2Empty;

    void InventorySlot_Empty_Check()
    {
        // current slot
        if (currentSlot.slotEmpty == true)
        {
            currentSlotEmpty = true;
        }
        else if (currentSlot.slotEmpty == false)
        {
            currentSlotEmpty = false;
        }
        // back slot
        if (backSlot.slotEmpty == true)
        {
            backSlotEmpty = true;
        }
        else if (backSlot.slotEmpty == false)
        {
            backSlotEmpty = false;
        }
        // throwable 1 slot
        if (throwable1Slot.slotEmpty == true)
        {
            throwable1Empty = true;
        }
        else if (throwable1Slot.slotEmpty == false)
        {
            throwable1Empty = false;
        }
        // throwable 2 slot
        if (throwable2Slot.slotEmpty == true)
        {
            throwable2Empty = true;
        }
        else if (throwable2Slot.slotEmpty == false)
        {
            throwable2Empty = false;
        }
    }
}
