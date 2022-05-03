using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Box_Display : MonoBehaviour
{
    [SerializeField] Mouse_Item_Icon mouseBoxItem;

    protected Box_System _boxSystem;
    protected Dictionary<BoxSlot_UI, Box_Slot> _slotDictionary;

    public Box_System boxSystem => _boxSystem;
    public Dictionary<BoxSlot_UI, Box_Slot> slotDictionary => _slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void Assign_Slot(Box_System boxToDisplay);

    protected virtual void Update_Slot(Box_Slot updatedSlot)
    {
        foreach(var slot in slotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.Update_UISlot(updatedSlot);
            }
        }
    }

    public void Slot_Clicked(BoxSlot_UI clickedUISlot)
    {
        if (clickedUISlot.assignedBoxSlot.itemInfo != null && mouseBoxItem.assignedBoxSlot.itemInfo == null)
        {
            // key press grab split

            // grab
            mouseBoxItem.Update_Mouse_Slot(clickedUISlot.assignedBoxSlot);
            clickedUISlot.Clear_Slot();
            return;
        }

        // clicked slot item X, mouse has item > place mouse item to the clicked slot
        if (clickedUISlot.assignedBoxSlot.itemInfo == null && mouseBoxItem.assignedBoxSlot.itemInfo != null)
        {
            clickedUISlot.assignedBoxSlot.Assign_Item(mouseBoxItem.assignedBoxSlot);
            clickedUISlot.Update_UISlot();

            mouseBoxItem.Clear_Slot();
        }
    }

    private void Swap_Slots(BoxSlot_UI clickedUISlot)
    {
        var clonedSlot = new Box_Slot(mouseBoxItem.assignedBoxSlot.itemInfo, mouseBoxItem.assignedBoxSlot.currentAmount);
        mouseBoxItem.Clear_Slot();

        mouseBoxItem.Update_Mouse_Slot(clickedUISlot.assignedBoxSlot);

        clickedUISlot.Clear_Slot();
        clickedUISlot.assignedBoxSlot.Assign_Item(clonedSlot);
        clickedUISlot.Update_UISlot();
    }
}