using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots_System : MonoBehaviour
{
    private Slots_DataBase dataBase;
    
    public bool isStaticSystem;
    public Slot[] slots;

    private void Awake()
    {
        dataBase = GameObject.FindGameObjectWithTag("GameController").GetComponent<Slots_DataBase>();
    }
    private void Start()
    {
        Find_EmptySlots();
    }

    //
    private void Find_EmptySlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == null)
            {
                slots[i].Empty_Slot();
            }
        }
    }
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Slot_DeSelect();
        }
    }

    //
    public void Save_Slot_DataBase(Slot slot)
    {
        if (isStaticSystem) { dataBase.Save_StaticSlot(slot); }
        else if (!isStaticSystem) { dataBase.Save_NormalSlot(slot); }
    }

    //
    public void Craft_Item(Item_Info item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == item)
            {
                slots[i].itemAmount += 1;
                slots[i].Assign_Slot(item, slots[i].itemAmount);
                break;
            }
            else if (slots[i].currentItem == null)
            {
                slots[i].Assign_Slot(item, 1);
                break;
            }
        }
    }
}
