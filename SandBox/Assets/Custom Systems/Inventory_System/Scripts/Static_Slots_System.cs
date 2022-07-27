using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Slots_System : MonoBehaviour
{
    [HideInInspector]
    public Slots_DataBase dataBase;
    public Static_Slot[] staticSlots;

    private void Awake()
    {
        dataBase = GameObject.FindGameObjectWithTag("GameController").GetComponent<Slots_DataBase>();
    }
    private void Start()
    {
        Search_Start_EmptySlots();
    }

    // connection


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
    public void Over_MaxAmount_Devide(Static_Slot staticSlot, Item_Info itemInfo)
    {
        if (staticSlot.itemAmount > staticSlot.currentItem.itemMaxAmount)
        {
            int leftOver = staticSlot.itemAmount - staticSlot.currentItem.itemMaxAmount;

            if (Slot_Available())
            {
                Add_Item(staticSlot.currentItem, leftOver);
            }
            else if (!Slot_Available())
            {
                dataBase.slotsSystem.Stack_Item(itemInfo, leftOver);
            }

            staticSlot.itemAmount = staticSlot.currentItem.itemMaxAmount;
            staticSlot.amountText.text = staticSlot.itemAmount.ToString();
        }
    }
    private void Add_Item(Item_Info itemInfo, int amount)
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

    // in
    public void Stack_Item(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            if (staticSlots[i].itemAmount != itemInfo.itemMaxAmount)
            {
                if (Stack_Available(itemInfo))
                {
                    staticSlots[i].Stack_Slot(amount);
                    Over_MaxAmount_Devide(staticSlots[i], itemInfo);
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
}
