using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots_Controller : MonoBehaviour
{
    public static Slot saveSlot1, saveSlot2;

    //
    public static void Assign_SaveSlot(Slot slot)
    {
        if (saveSlot1 == null)
        {
            saveSlot1 = slot;
        }
        else if (saveSlot1 != null)
        {
            saveSlot2 = slot;
        }
    }

    //
    public static void CLear_All_SaveSlots()
    {
        saveSlot1 = null;
        saveSlot2 = null;
    }
    public static void Clear_SaveSlot1()
    {
        saveSlot1 = null;
    }
    public static void Clear_SaveSlot2()
    {
        saveSlot2 = null;
    }
}
