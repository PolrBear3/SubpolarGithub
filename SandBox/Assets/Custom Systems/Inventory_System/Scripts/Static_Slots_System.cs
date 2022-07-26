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

    // start
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

    // check
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
    public bool Other_Slot_Available()
    {
        if (dataBase.slotsSystem.Slot_Available())
        {
            return true;
        }
        else return false;
    }

    // in
    public void Assign_to_EmptySlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            // search if there is same item
            if (itemInfo == staticSlots[i].currentItem)
            {
                // slot stack function
                staticSlots[i].Stack_Slot(amount);
            }
            // if it is the new item in the array
            else
            {
                staticSlots[i].Assign_Slot(itemInfo, amount);
            }
        }
    }
    private void Assign_to_Empty_for_LeftOver(Item_Info itemInfo, int leftOverAmount)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            if (staticSlots[i].currentItem == null)
            {
                staticSlots[i].Assign_Slot(itemInfo, leftOverAmount);
                break;
            }
        }
    }


    // resest
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            staticSlots[i].DeSelect_Slot();
        }
    }
}
