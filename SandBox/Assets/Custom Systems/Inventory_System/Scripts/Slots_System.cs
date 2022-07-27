using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots_System : MonoBehaviour
{
    [HideInInspector]
    public Slots_DataBase dataBase;
    public Slot[] slots;


    private void Awake()
    {
        dataBase = GameObject.FindGameObjectWithTag("GameController").GetComponent<Slots_DataBase>();
    }
    private void Start()
    {
        Search_Start_EmptySlots();
    }

    public void Connect_toDataBase()
    {
        dataBase.Connect_Slots_System(this);
    }

    // start
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

    // check
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
    public bool Same_Item_Stack_Available(Item_Info itemInfo)
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
                Check_Add_Item(slot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                dataBase.staticSlotsSystem.Check_Add_Item(itemInfo, leftOver);
            }

            slot.itemAmount = slot.currentItem.itemMaxAmount;
            slot.amountText.text = slot.itemAmount.ToString();
        }
    }

    // in
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
    public void Check_Add_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemAmount != itemInfo.itemMaxAmount)
            {
                if (Same_Item_Stack_Available(itemInfo))
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

    // resest
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].DeSelect_Slot();
        }
    }

    // craft test
    public void Craft_Item(Item_Info itemInfo)
    {
        Check_Add_Item(itemInfo, 1);
    }
}
