using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots_System : MonoBehaviour
{
    public Static_Slots_System hostSystem;
    public Slot[] slots;

    private void Awake()
    {
        hostSystem = GameObject.FindGameObjectWithTag("GameController").GetComponent<Static_Slots_System>();
        Connect_to_HostSystem();
    }
    private void Start()
    {
        Search_Start_EmptySlots();
    }

    // connection
    public void Connect_to_HostSystem()
    {
        hostSystem.Connect(this);
    }
        // if this system is enabled or destroyed > hostSystem.Reset_HostSystem(), DeSelect_All_Slots()

    // empty all the slots that are null at start
    private void Search_Start_EmptySlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == null)
            {
                slots[i].Empty_Slot();
            }
        }
    }

    // functions for slots
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].DeSelect_Slot();
        }
    }

    // check functions
    public bool Slot_Available()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].hasItem)
            {
                return true;
            }
        }
        return false;
    }
    public bool Stack_Available(Item_Info itemInfo)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (itemInfo == slots[i].currentItem && slots[i].itemAmount < itemInfo.itemMaxAmount)
            {
                return true;
            }
        }
        return false;
    }

    // in
    private void MaxSplit_Refund(Slot slot, Item_Info itemInfo)
    {
        if (slot.itemAmount > slot.currentItem.itemMaxAmount)
        {
            int leftOver = slot.itemAmount - slot.currentItem.itemMaxAmount;

            if (Slot_Available())
            {
                AddItem_to_NewSlot(slot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                hostSystem.Craft_Item(itemInfo, leftOver);
            }

            slot.itemAmount = slot.currentItem.itemMaxAmount;
            slot.amountText.text = slot.itemAmount.ToString();
        }
    }
    private void AddItem_to_NewSlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == null)
            {
                slots[i].Assign_Slot(itemInfo, amount);
                break;
            }
        }
    }

    public void Craft_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            DeSelect_All_Slots();
            hostSystem.DeSelect_All_Slots();

            // if the slot is empty
            if (!slots[i].hasItem)
            {
                AddItem_to_NewSlot(itemInfo, amount);
                break;
            }
            // if the slot has the same item and the current amount is not the max amount
            else if (slots[i].currentItem == itemInfo && slots[i].itemAmount < itemInfo.itemMaxAmount)
            {
                slots[i].Stack_Slot(amount);
                MaxSplit_Refund(slots[i], itemInfo);
                break;
            }
        }
    }

    // craft test
    public void Test_Craft_Item(Item_Info itemInfo)
    {
        Craft_Item(itemInfo, 1);
    }
}
