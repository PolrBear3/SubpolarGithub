using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Slots_System : MonoBehaviour
{
    private Slots_DataBase dataBase;
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

}
