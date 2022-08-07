using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip_System : MonoBehaviour
{
    // system type num > 3

    public Guest_System guestSystem;
    public Host_System hostSystem;

    public Equip_Slot[] equipSlots;

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

    // empty all the slots that are null at start
    private void Search_Start_EmptySlots()
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].currentItem == null)
            {
                equipSlots[i].Empty_Slot();
            }
        }
    }

    // function for slots
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            equipSlots[i].DeSelect_Slot();
        }
    }

    // check functions
    public bool Slot_Available(ItemType itemType)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (itemType == equipSlots[i].itemType && !equipSlots[i].hasItem)
            {
                return true;
            }
        }
        return false;
    }
    public bool Stack_Available(Item_Info itemInfo)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (itemInfo == equipSlots[i].currentItem && equipSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                return true;
            }
        }
        return false;
    }

    // input to system
    private void AddItem_to_NewSlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            // item type check
            if (equipSlots[i].itemType == itemInfo.itemType)
            {
                if (equipSlots[i].currentItem == null)
                {
                    equipSlots[i].Assign_Slot(itemInfo, amount);
                    break;
                }
            }
        }
    }
    public void Craft_Item(int systemTypeNum, Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            DeSelect_All_Slots();
            hostSystem.DeSelect_All_Slots();
            if (guestSystem_Connected()) { guestSystem.DeSelect_All_Slots(); }

            // item type check
            if (equipSlots[i].itemType == itemInfo.itemType && equipSlots[i].currentAmount != itemInfo.itemMaxAmount)
            {
                // if the slot is empty
                if (!equipSlots[i].hasItem)
                {
                    AddItem_to_NewSlot(itemInfo, amount);
                    break;
                }
                // if the slot has the same item and the current amount is not the max amount
                else if (equipSlots[i].currentItem == itemInfo && equipSlots[i].currentAmount < itemInfo.itemMaxAmount)
                {
                    equipSlots[i].Stack_Slot(amount);

                    if (equipSlots[i].currentItem == itemInfo && equipSlots[i].currentAmount > itemInfo.itemMaxAmount)
                    {
                        int leftOver = equipSlots[i].currentAmount - itemInfo.itemMaxAmount;

                        // host check
                        if (leftOver > 0 && systemTypeNum == 1)
                        {
                            hostSystem.Craft_Item(1, itemInfo, leftOver);
                        }
                        // guest check
                        else if (leftOver > 0 && systemTypeNum == 2)
                        {
                            guestSystem.Craft_Item(2, itemInfo, leftOver);
                        }

                        equipSlots[i].currentAmount = equipSlots[i].currentItem.itemMaxAmount;
                        equipSlots[i].amountText.text = equipSlots[i].currentAmount.ToString();
                    }
                    break;
                }
            }
        }
    }
}
