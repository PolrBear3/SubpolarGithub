using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Box_UI_Controller : MonoBehaviour
{
    public Dynamic_Box_Display inventoryPanel;

    private void OnEnable()
    {
        Box_Holder.boxSlotUpdateRequested += Display_Inventory;
    }

    private void OnDisable()
    {
        Box_Holder.boxSlotUpdateRequested -= Display_Inventory;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Display_Inventory(new Box_System(4));
        }

        if (inventoryPanel.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.R))
        {
            inventoryPanel.gameObject.SetActive(false);
        }
    }

    void Display_Inventory(Box_System invToDisplay)
    {
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.Refresh_Dynamic_Box(invToDisplay);
    }
}
