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
    // in
    public void Assign_to_EmptySlot(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < staticSlots.Length; i++)
        {
            // stack check

            if (staticSlots[i] == null)
            {
                staticSlots[i].Assign_Slot(itemInfo, amount);
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
