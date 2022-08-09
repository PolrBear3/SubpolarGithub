using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host_System : MonoBehaviour
{
    // system type num > 1

    public Guest_System guestSystem;
    public Equip_System equipSystem;
    public GameObject inventoryMenu;
    
    public Host_Slot[] hostSlots;

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
        equipSystem.guestSystem = guestSystem;
        DeSelect_All_Slots();
        guestSystem.DeSelect_All_Slots();
    }
    public void Reset_GuestSystem()
    {
        guestSystem = null;
        equipSystem.guestSystem = null;
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
    private void Transfer_Item_Durability(bool isNew, Host_Slot slot, Item_Info itemInfo, float durability)
    {
        if (!isNew)
        {
            slot.currentDurability = durability;
        }
        else
        {
            slot.currentDurability = itemInfo.itemMaxDurability;
        }
    }
    private void AddItem_to_NewSlot(bool isNew, Item_Info itemInfo, int amount, float durability)
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            if (hostSlots[i].currentItem == null)
            {
                Transfer_Item_Durability(isNew, hostSlots[i], itemInfo, durability);
                hostSlots[i].Assign_Slot(itemInfo, amount);
                break;
            }
        }
    }
    
    public void Craft_Item(bool isNew, int systemTypeNum, Item_Info itemInfo, int amount, float durability)
    {
        for (int i = 0; i < hostSlots.Length; i++)
        {
            DeSelect_All_Slots();
            equipSystem.DeSelect_All_Slots();
            if (guestSystem_Connected()) { guestSystem.DeSelect_All_Slots(); }

            // if the slot is empty
            if (!hostSlots[i].hasItem)
            {
                AddItem_to_NewSlot(isNew, itemInfo, amount, durability);
                break;
            }
            // if the slot has the same item and the current amount is not the max amount
            else if (hostSlots[i].currentItem == itemInfo && hostSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                // stack the item
                hostSlots[i].Stack_Slot(amount);

                // if the slot current amount is over max amount
                if (hostSlots[i].currentAmount > itemInfo.itemMaxAmount)
                {
                    int leftOver = hostSlots[i].currentAmount - itemInfo.itemMaxAmount;

                    // this host check
                    if (leftOver > 0 && systemTypeNum == 1)
                    {
                        Craft_Item(true, 1, itemInfo, leftOver, 0);
                    }
                    // guest check
                    else if (leftOver > 0 && systemTypeNum == 2)
                    {
                        if (Slot_Available() || Stack_Available(itemInfo))
                        {
                            Craft_Item(true, 1, itemInfo, leftOver, 0);
                        }
                        else
                        {
                            guestSystem.Craft_Item(true, 1, itemInfo, leftOver, 0);
                        }
                    }
                    // equip check
                    else if (leftOver > 0 && systemTypeNum == 3)
                    {
                        equipSystem.Craft_Item(true, 1, itemInfo, leftOver, 0);
                    }

                    hostSlots[i].currentAmount = hostSlots[i].currentItem.itemMaxAmount;
                    hostSlots[i].amountText.text = hostSlots[i].currentAmount.ToString();
                }
                break;
            }
        }
    }

    // test
    public void Test_Spawn_Item(Item_Info item)
    {
        Craft_Item(true, 1, item, 1, item.itemMaxDurability);
    }
}
