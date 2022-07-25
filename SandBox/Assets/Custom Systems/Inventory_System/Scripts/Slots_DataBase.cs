using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots_DataBase : MonoBehaviour
{
    public Static_Slots_System staticSlotsSystem;
    public Slots_System slotsSystem;

    public void Connect_Slots_System(Slots_System slotsSystem)
    {
        this.slotsSystem = slotsSystem;
    }
    public void Reset_Slots_System()
    {
        slotsSystem = null;
    }

    public void Move_to_StaticSlotsSystem(Item_Info itemInfo, int amount)
    {
        staticSlotsSystem.Assign_to_EmptySlot(itemInfo, amount);
    }
    public void Move_to_Slots_System(Item_Info itemInfo, int amount)
    {
        slotsSystem.Assign_to_EmptySlot(itemInfo, amount);
    }
}
