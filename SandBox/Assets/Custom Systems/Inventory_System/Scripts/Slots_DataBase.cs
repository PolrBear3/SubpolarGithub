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

    public void Moveto_StaticSlotsSystem(Item_Info itemInfo, int amount)
    {
        staticSlotsSystem.Stack_Item(itemInfo, amount);
    }
    public void Moveto_SlotsSystem(Item_Info itemInfo, int amount)
    {
        slotsSystem.Stack_Item(itemInfo, amount);
    }
}
