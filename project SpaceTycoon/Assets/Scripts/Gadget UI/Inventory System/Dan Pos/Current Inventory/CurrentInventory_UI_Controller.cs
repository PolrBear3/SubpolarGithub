using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CurrentInventory_UI_Controller : MonoBehaviour
{
    public Dynamic_CurrentInventory_Display inventoryPanel;

    private void OnEnable()
    {
        Box_Holder.boxSlotUpdateRequested += Display_Inventory;
    }

    private void OnDisable()
    {
        Box_Holder.boxSlotUpdateRequested -= Display_Inventory;
    }

    void Display_Inventory(Box_System invToDisplay)
    {
        inventoryPanel.Refresh_Dynamic_Box(invToDisplay);
    }
}
