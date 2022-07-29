using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Slots_System : MonoBehaviour
{
    public Slots_System guestSystem;
    public Static_Slot[] staticSlots;

    private void Start()
    {
        Search_Start_EmptySlots();
    }

    // connection
    public bool guestSystem_Connected()
    {
        if (guestSystem != null) { return true; }
        else return false;
    }

    public void Connect(Slots_System guestSystem)
    {
        this.guestSystem = guestSystem;
        DeSelect_All_Slots();
        guestSystem.DeSelect_All_Slots();
    }
    public void Reset_HostSystem()
    {
        guestSystem = null;
        DeSelect_All_Slots();
    }
    
    // empty all the slots that are null at start
    private void Search_Start_EmptySlots()
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            if (staticSlots[i].currentItem == null)
            {
                staticSlots[i].Empty_Slot();
            }
        }
    }

    // function for slots
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            staticSlots[i].DeSelect_Slot();
        }
    }

    // check functions
    public bool Slot_Available()
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            if (!staticSlots[i].hasItem)
            {
                return true;
            }
        }
        return false;
    }
    public bool Stack_Available(Item_Info itemInfo)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            if (itemInfo == staticSlots[i].currentItem && staticSlots[i].itemAmount < itemInfo.itemMaxAmount)
            {
                return true;
            }
        }
        return false;
    }

    // in
    private void MaxSplit_Refund(Static_Slot staticSlot, Item_Info itemInfo)
    {
        if (staticSlot.itemAmount > staticSlot.currentItem.itemMaxAmount)
        {
            int leftOver = staticSlot.itemAmount - staticSlot.currentItem.itemMaxAmount;

            if (Slot_Available())
            {
                AddItem_to_NewSlot(staticSlot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                guestSystem.Craft_Item(itemInfo, leftOver);
            }

            staticSlot.itemAmount = staticSlot.currentItem.itemMaxAmount;
            staticSlot.amountText.text = staticSlot.itemAmount.ToString();
        }
    }
    private void AddItem_to_NewSlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            if (staticSlots[i].currentItem == null)
            {
                staticSlots[i].Assign_Slot(itemInfo, amount);
                break;
            }
        }
    }
    
    public void Craft_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            DeSelect_All_Slots();
            guestSystem.DeSelect_All_Slots();

            // if the slot is empty
            if (!staticSlots[i].hasItem)
            {
                AddItem_to_NewSlot(itemInfo, amount);
                break;
            }
            // if the slot has the same item and the current amount is not the max amount
            else if (staticSlots[i].currentItem == itemInfo && staticSlots[i].itemAmount < itemInfo.itemMaxAmount)
            {
                staticSlots[i].Stack_Slot(amount);
                MaxSplit_Refund(staticSlots[i], itemInfo);
                break;
            }
        }
    }
}
