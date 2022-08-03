using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest_System : MonoBehaviour
{
    [HideInInspector]
    public Host_System hostSystem;
    public Guest_Slot[] guestSlots;

    [HideInInspector]
    public bool slotSelected;

    private void Awake()
    {
        hostSystem = GameObject.FindGameObjectWithTag("player_inventory").GetComponent<Host_System>();
    }
    private void Start()
    {
        Search_Start_EmptySlots();
    }

    // connection
    public bool hostSystem_Connected()
    {
        if (hostSystem != null) { return true; }
        else return false;
    }
    public void Connect_to_HostSystem()
    {
        hostSystem.Connect(this);
    }
    public void Disconnect_HostSystem()
    {
        hostSystem.Reset_GuestSystem();
        DeSelect_All_Slots();
    }

    // empty all the slots that are null at start
    private void Search_Start_EmptySlots()
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            if (guestSlots[i].currentItem == null)
            {
                guestSlots[i].Empty_Slot();
            }
        }
    }

    // functions for slots
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            guestSlots[i].DeSelect_Slot();
        }
    }

    // check functions
    public bool Slot_Available()
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            if (!guestSlots[i].hasItem)
            {
                return true;
            }
        }
        return false;
    }
    public bool Stack_Available(Item_Info itemInfo)
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            if (itemInfo == guestSlots[i].currentItem && guestSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                return true;
            }
        }
        return false;
    }

    // input to system
    private void MaxSplit_Refund(Guest_Slot guestSlot, Item_Info itemInfo)
    {
        if (guestSlot.currentAmount > guestSlot.currentItem.itemMaxAmount)
        {
            int leftOver = guestSlot.currentAmount - guestSlot.currentItem.itemMaxAmount;

            if (Slot_Available())
            {
                AddItem_to_NewSlot(guestSlot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                hostSystem.Craft_Item(itemInfo, leftOver);
            }

            guestSlot.currentAmount = guestSlot.currentItem.itemMaxAmount;
            guestSlot.amountText.text = guestSlot.currentAmount.ToString();
        }
    }
    private void AddItem_to_NewSlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            if (guestSlots[i].currentItem == null)
            {
                guestSlots[i].Assign_Slot(itemInfo, amount);
                break;
            }
        }
    }
    public void Craft_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            DeSelect_All_Slots();
            if (hostSystem_Connected()) { hostSystem.DeSelect_All_Slots(); }

            // if the slot is empty
            if (!guestSlots[i].hasItem)
            {
                AddItem_to_NewSlot(itemInfo, amount);
                break;
            }
            // if the slot has the same item and the current amount is not the max amount
            else if (guestSlots[i].currentItem == itemInfo && guestSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                guestSlots[i].Stack_Slot(amount);
                MaxSplit_Refund(guestSlots[i], itemInfo);
                break;
            }
        }
    }
}
