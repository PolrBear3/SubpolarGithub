using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    private void Update()
    {
        BagSlot_Empty_Check();
    }

    public Item_Slot slot1, slot2, slot3, slot4;
    public static bool slot1Empty, slot2Empty, slot3Empty, slot4Empty;

    void BagSlot_Empty_Check()
    {
        // 1
        if (slot1.slotEmpty == true)
        {
            slot1Empty = true;
        }
        else if (slot1.slotEmpty == false)
        {
            slot1Empty = false;
        }
        // 2
        if (slot2.slotEmpty == true)
        {
            slot2Empty = true;
        }
        else if (slot2.slotEmpty == false)
        {
            slot2Empty = false;
        }
        // 3
        if (slot3.slotEmpty == true)
        {
            slot3Empty = true;
        }
        else if (slot3.slotEmpty == false)
        {
            slot3Empty = false;
        }
        // 4
        if (slot4.slotEmpty == true)
        {
            slot4Empty = true;
        }
        else if (slot4.slotEmpty == false)
        {
            slot4Empty = false;
        }
    }
}
