using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host_System : MonoBehaviour
{
    public Guest_System guestSystem;
    public Host_Slot[] hostSlots;

    public GameObject inventoryMenu;

    [HideInInspector]
    public bool slotSelected;

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
    public void Connect(Guest_System guestSystem)
    {
        this.guestSystem = guestSystem;
        DeSelect_All_Slots();
        guestSystem.DeSelect_All_Slots();
    }
    public void Reset_GuestSystem()
    {
        guestSystem = null;
        DeSelect_All_Slots();
    }

    // empty all the slots that are null at start
    private void Search_Start_EmptySlots()
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            if (hostSlots[i].currentItem == null)
            {
                hostSlots[i].Empty_Slot();
            }
        }
    }

    // function for slots
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            hostSlots[i].DeSelect_Slot();
        }
    }

    // check functions
    public bool Slot_Available()
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            if (!hostSlots[i].hasItem)
            {
                return true;
            }
        }
        return false;
    }
    public bool Stack_Available(Item_Info itemInfo)
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            if (itemInfo == hostSlots[i].currentItem && hostSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                return true;
            }
        }
        return false;
    }

    // input to system
    private void MaxSplit_Refund(Host_Slot hostSlot, Item_Info itemInfo)
    {
        if (hostSlot.currentAmount > hostSlot.currentItem.itemMaxAmount)
        {
            int leftOver = hostSlot.currentAmount - hostSlot.currentItem.itemMaxAmount;

            if (Slot_Available())
            {
                AddItem_to_NewSlot(hostSlot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                guestSystem.Craft_Item(itemInfo, leftOver);
            }

            hostSlot.currentAmount = hostSlot.currentItem.itemMaxAmount;
            hostSlot.amountText.text = hostSlot.currentAmount.ToString();
        }
    }
    private void AddItem_to_NewSlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            if (hostSlots[i].currentItem == null)
            {
                hostSlots[i].Assign_Slot(itemInfo, amount);
                break;
            }
        }
    }
    public void Craft_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            DeSelect_All_Slots();
            if (guestSystem_Connected()) { guestSystem.DeSelect_All_Slots(); }

            // if the slot is empty
            if (!hostSlots[i].hasItem)
            {
                AddItem_to_NewSlot(itemInfo, amount);
                break;
            }
            // if the slot has the same item and the current amount is not the max amount
            else if (hostSlots[i].currentItem == itemInfo && hostSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                hostSlots[i].Stack_Slot(amount);
                MaxSplit_Refund(hostSlots[i], itemInfo);
                break;
            }
        }
    }

    // test
    public void Test_Spawn_Item(Item_Info item)
    {
        Craft_Item(item, 1);
    }
}
