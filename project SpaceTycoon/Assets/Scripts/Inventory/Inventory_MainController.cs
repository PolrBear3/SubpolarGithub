using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    public Inventory inventory;
    public Bag bag;
    
    public Transform currentSlot, backSlot, throwable1Slot, throwable2Slot;
    public Transform bagSlot1, bagSlot2, bagSlot3, bagslot4;

    public void Move_to_Bag(GameObject icon)
    {
        if (bag.slot1Empty)
        {
            Instantiate(icon, bagSlot1);
        }
        else if (bag.slot2Empty)
        {
            Instantiate(icon, bagSlot2);
        }
        else if (bag.slot3Empty)
        {
            Instantiate(icon, bagSlot3);
        }
        else if (bag.slot4Empty)
        {
            Instantiate(icon, bagslot4);
        }
    }

    public void Move_to_Inventory(GameObject icon, string itemType)
    {
        if (itemType == "currentType" && inventory.currentSlotEmpty)
        {
            Instantiate(icon, currentSlot);
        }

        if (itemType == "backType" && inventory.backSlotEmpty)
        {
            Instantiate(icon, backSlot);
        }

        if (itemType == "throwableType" && inventory.throwable1Empty)
        {
            Instantiate(icon, throwable1Slot);
        }
        else if (itemType == "throwableType" && inventory.throwable2Empty)
        {
            Instantiate(icon, throwable2Slot);
        }
    }
}
