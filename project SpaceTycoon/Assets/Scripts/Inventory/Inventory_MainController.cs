using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    // test start from inventory
    public GameObject jetPack_icon_inventory;
    private void Start()
    {
        
    }

    // slots
    public Inventory inventory;
    public Bag bag;
    
    // position
    public Transform currentSlot, backSlot, throwable1Slot, throwable2Slot;
    public Transform bagSlot1, bagSlot2, bagSlot3, bagSlot4;

    // player items
    public GameObject jetPack;
}
