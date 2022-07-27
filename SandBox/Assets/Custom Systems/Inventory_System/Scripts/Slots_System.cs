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
    public void Over_MaxAmount_Devide(Slot slot, Item_Info itemInfo)
    {
        if (slot.itemAmount > slot.currentItem.itemMaxAmount)
        {
            int leftOver = slot.itemAmount - slot.currentItem.itemMaxAmount;

            if (Slot_Available())
            {
                Add_Item(slot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                hostSystem.Stack_Item(itemInfo, leftOver);
            }

            slot.itemAmount = slot.currentItem.itemMaxAmount;
            slot.amountText.text = slot.itemAmount.ToString();
        }
    }
    private void Add_Item(Item_Info itemInfo, int amount)
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

    // in
    public void Stack_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemAmount != itemInfo.itemMaxAmount)
            {
                if (Stack_Available(itemInfo))
                {
                    slots[i].Stack_Slot(amount);
                    Over_MaxAmount_Devide(slots[i], itemInfo);
                    break;
                }
                else if (Slot_Available())
                {
                    Add_Item(itemInfo, amount);
                    break;
                }
            }
        }
    }

    // craft test
    public void Craft_Item(Item_Info itemInfo)
    {
        Stack_Item(itemInfo, 1);
    }
}
