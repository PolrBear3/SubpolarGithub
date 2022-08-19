using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest_System : MonoBehaviour
{
    // system type num > 2

    [HideInInspector]
    public Host_System hostSystem;
    public SlotItems_CheckSystem checkSystem;
    
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
    private void Transfer_Item_Durability(bool isNew, Guest_Slot slot, Item_Info itemInfo, float durability)
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
        for (int i = 0; i < guestSlots.Length; i++)
        {
            if (guestSlots[i].currentItem == null)
            {
                Transfer_Item_Durability(isNew, guestSlots[i], itemInfo, durability);
                guestSlots[i].Assign_Slot(itemInfo, amount);
                break;
            }
        }
    }

    public void Craft_Item(bool isNew, int systemTypeNum, Item_Info itemInfo, int amount, float durability)
    {
        for (int i = 0; i < guestSlots.Length; i++)
        {
            DeSelect_All_Slots();
            if (hostSystem_Connected()) { hostSystem.DeSelect_All_Slots(); }

            // if the slot is empty
            if (!guestSlots[i].hasItem)
            {
                AddItem_to_NewSlot(isNew, itemInfo, amount, durability);
                break;
            }
            // if the slot has the same item and the current amount is not the max amount
            else if (guestSlots[i].currentItem == itemInfo && guestSlots[i].currentAmount < itemInfo.itemMaxAmount)
            {
                guestSlots[i].Stack_Slot(amount);

                // if the slot current amount is over max amount
                if (guestSlots[i].currentAmount > itemInfo.itemMaxAmount)
                {
                    int leftOver = guestSlots[i].currentAmount - itemInfo.itemMaxAmount;

                    // this guest check
                    if (leftOver > 0 && systemTypeNum == 2)
                    {
                        Craft_Item(true, 2, itemInfo, leftOver, 0);
                    }
                    // host check
                    else if (leftOver > 0 && systemTypeNum == 1)
                    {
                        if (Slot_Available() || Stack_Available(itemInfo))
                        {
                            Craft_Item(true, 2, itemInfo, leftOver, 0);
                        }
                        else
                        {
                            hostSystem.Craft_Item(true, 2, itemInfo, leftOver, 0);
                        }
                    }
                    // equip check
                    else if (leftOver > 0 && systemTypeNum == 3)
                    {
                        hostSystem.equipSystem.Craft_Item(true, 3, itemInfo, leftOver, 0);
                    }

                    if (checkSystem != null)
                    {
                        checkSystem.SubtractCount_Ingredients(itemInfo, leftOver);
                    }

                    guestSlots[i].currentAmount = guestSlots[i].currentItem.itemMaxAmount;
                    guestSlots[i].amountText.text = guestSlots[i].currentAmount.ToString();
                }
                break;
            }
        }
    }
    public void Use_Items(Item_Info itemInfo, int useAmount)
    {
        checkSystem.SubtractCount_Ingredients(itemInfo, useAmount);

        int useAmountTracking = useAmount;

        for (int i = 0; i < guestSlots.Length; i++)
        {
            if (itemInfo == guestSlots[i].currentItem)
            {
                if (useAmountTracking <= guestSlots[i].currentAmount)
                {
                    guestSlots[i].currentAmount -= useAmountTracking;
                    guestSlots[i].Update_Slot_UI();

                    if (guestSlots[i].currentAmount == 0)
                    {
                        guestSlots[i].Empty_Slot();
                    }

                    break;
                }

                useAmountTracking -= guestSlots[i].currentAmount;

                if (useAmountTracking <= 0)
                {
                    break;
                }
                else if (useAmountTracking > 0)
                {
                    guestSlots[i].Empty_Slot();
                    continue;
                }
            }
        }
    }
}
