using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class Box_System
{
    [SerializeField] private List<Box_Slot> _boxSlots;
    public List<Box_Slot> boxSlots => _boxSlots;

    public int boxSize => boxSlots.Count;

    public UnityAction<Box_Slot> boxSlotUpdate;

    // fill and make empty slots of size in bag
    public Box_System(int size)
    {
        _boxSlots = new List<Box_Slot>(size);

        for (int i = 0; i < size; i++)
        {
            _boxSlots.Add(new Box_Slot());
        }
    }

    public bool Add_to_Box(Item_Info itemToAdd, int amountToAdd)
    {
        if (Contains_Item(itemToAdd, out List<Box_Slot> boxSlot))
        {
            foreach(var slot in boxSlot)
            {
                if (slot.Room_left_Stack(amountToAdd))
                {
                    slot.Add_to_Stack(amountToAdd);
                    boxSlotUpdate?.Invoke(slot);
                    return true;
                }
            }
        }

        if (Has_Free_Slot(out Box_Slot freeSlot))
        {
            freeSlot.Update_Box_Slot(itemToAdd, amountToAdd);
            boxSlotUpdate?.Invoke(freeSlot);
            return true;
        }

        return false;
    }

    public bool Contains_Item(Item_Info itemToAdd, out List<Box_Slot> boxSlot)
    {
        boxSlot = boxSlots.Where(i => i.itemInfo == itemToAdd).ToList();
        return boxSlot == null ? false : true;
    }

    public bool Has_Free_Slot(out Box_Slot freeSlot)
    {
        freeSlot = boxSlots.FirstOrDefault(i => i.itemInfo == null);
        return freeSlot == null ? false : true;
    }
}
