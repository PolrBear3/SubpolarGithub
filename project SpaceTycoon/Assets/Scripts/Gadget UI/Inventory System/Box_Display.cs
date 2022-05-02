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

    public void Slot_Clicked(BoxSlot_UI clickedSlot)
    {
        Debug.Log("slot clicked");
    }
}