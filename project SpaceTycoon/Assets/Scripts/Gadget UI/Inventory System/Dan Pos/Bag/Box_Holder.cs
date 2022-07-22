using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Box_Holder : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;

    [SerializeField] private int boxSize;

    [SerializeField] protected Box_System _boxSystem;
    public Box_System boxSystem => _boxSystem;

    public static UnityAction<Box_System> boxSlotUpdateRequested;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        _boxSystem = new Box_System(boxSize);
    }

    // slot space available check before crafting item
    bool Slot_Full_Check()
    {
        for (int i = 0; i < _boxSystem.boxSlots.Count; i++) // goes through all slots until it finds an available slot
        {
            if (_boxSystem.boxSlots[i].itemInfo == null)
            {
                return true; // if it finds the first available slot, it returns true and breaks the loop
            }
        }
        return false; // if it went through everything and couldn't find an available slot, it returns false
    }
    // check if wanted crafting item can be stacked when all slots are full
    bool Slot_StackFull_Check(Item_Info itemInfo, int amountToAdd)
    {
        if (_boxSystem.Contains_Item(itemInfo, out List<Box_Slot> boxSlot))
        {
            foreach (var slot in boxSlot)
            {
                if (slot.Room_left_Stack(amountToAdd))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // craft and add item to this boxHolder function
    public void Add_Item(ItemType2 itemType2, int itemNum, int amount)
    {
        if (itemType2 == ItemType2.ingredient)
        {
            var selectedItem = controller.ingredients;

            if (Slot_Full_Check() || Slot_StackFull_Check(selectedItem[itemNum], amount))
            {
                _boxSystem.Add_to_Box(selectedItem[itemNum], amount);
            }
        }
        else if (itemType2 == ItemType2.item)
        {
            var selectedItem = controller.items;

            if (Slot_Full_Check() || Slot_StackFull_Check(selectedItem[itemNum], amount))
            {
                _boxSystem.Add_to_Box(selectedItem[itemNum], amount);
            }
        }
    }
}
