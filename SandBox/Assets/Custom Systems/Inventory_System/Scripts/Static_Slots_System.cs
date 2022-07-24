using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Slots_System : MonoBehaviour
{
    private Slots_DataBase dataBase;
    public Static_Slot[] staticSlots;

    private void Awake()
    {
        dataBase = GameObject.FindGameObjectWithTag("GameController").GetComponent<Slots_DataBase>();
    }

    // out 
    public void Move_Slot_to_SlotsSystem(Item_Info itemInfo, int amount)
    {
        dataBase.Move_StaticSlotsSystem_to_SlotsSystem(itemInfo, amount);
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
}
