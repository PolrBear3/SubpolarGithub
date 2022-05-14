using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mouse_Item_Icon : MonoBehaviour
{
    public Image itemSprite;
    public Text itemCount;
    public Box_Slot assignedBoxSlot;

    public static bool holdingItemOnMouse = false;
    public InventorySlot[] inventorySlot;

    private void Awake()
    {
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    void Update_Inventory_Slot_Status()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].Inventory_Slot_Status_Update();
        }
    }

    public void Update_Mouse_Slot(Box_Slot boxSlot)
    {
        holdingItemOnMouse = true;

        assignedBoxSlot.Assign_Item(boxSlot);
        itemSprite.sprite = boxSlot.itemInfo.itemIcon;
        itemCount.text = boxSlot.currentAmount.ToString();
        itemSprite.color = Color.white;
    }

    private void Update()
    {
        if (assignedBoxSlot.itemInfo != null)
        {
            Update_Inventory_Slot_Status();

            transform.position = Mouse.current.position.ReadValue();

            if (BoxSlot_UI.slotClicked && Mouse.current.leftButton.wasPressedThisFrame && !Is_Pointer_Over_UIObject())
            {
                Clear_Slot();
            }
        }
    }

    public void Clear_Slot()
    {
        holdingItemOnMouse = false;
        Update_Inventory_Slot_Status();

        assignedBoxSlot.Clear_Slot();
        itemCount.text = "";
        itemSprite.color = Color.clear;
        itemSprite.sprite = null;
    }

    public static bool Is_Pointer_Over_UIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
