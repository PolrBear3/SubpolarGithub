using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    public Item_Slot slot;
    public Bag bag;
    public Inventory inventory;
    
    public Transform currentSlot, backSlot, throwable1Slot, throwable2Slot;
    public Transform bagSlot1, bagSlot2, bagSlot3, bagslot4;

    public void Move_to_Bag(GameObject icon)
    {
        // example reference from Icon script
    }

    public void Move_to_Inventory(GameObject icon)
    {
        // example reference from Icon script
    }
}
