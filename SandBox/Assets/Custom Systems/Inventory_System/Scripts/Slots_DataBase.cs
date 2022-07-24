using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots_DataBase : MonoBehaviour
{
    public Slot staticSlot;
    public Slot normalSlot;

    public void Save_StaticSlot(Slot slot)
    {
        staticSlot = slot;
    }
    public void Save_NormalSlot(Slot slot)
    {
        normalSlot = slot;
    }

    public void Clear_StaticSlot()
    {
        staticSlot = null;
    }
    public void Clear_NormalSlot()
    {
        normalSlot = null;
    }
}
