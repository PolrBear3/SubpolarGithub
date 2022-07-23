using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slots_System : MonoBehaviour
{
    public Slot[] slots;

    private void Start()
    {
        Empty_All_Slots();
    }

    //
    private void Empty_All_Slots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Empty_Slot();
        }
    }
    public void DeSelect_All_Slots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].DeSelect_Slot();
        }
    }

    //
    public void Craft_Item(Item_Info item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == item)
            {
                slots[i].slotAmount += 1;
                slots[i].Assign_Slot(item, slots[i].slotAmount);
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
