using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public Gadget_MainController gadgetController;
    public Host_System[] hostSystems;
    
    public Animator openmenuButton;
    public GameObject inventoryMenu;

    public void Open_Inventory_Menu()
    {
        gadgetController.Open_Gadget_Menu(inventoryMenu, openmenuButton);

        // deselect all slots if guest system is connected and the slot is currently selected
        if (hostSystems[1].guestSystem_Connected())
        {
            hostSystems[1].guestSystem.DeSelect_All_Slots();
        }
    }
    public void Close_Inventory_Menu()
    {
        gadgetController.Close_All_Gadget_Menus(openmenuButton);
        
        // Deselect all slots when closing out of inventory
        for (int i = 0; i < hostSystems.Length; i++)
        {
            hostSystems[i].DeSelect_All_Slots();
        }

        // deselect all slots if guest system is connected and the slot is currently selected
        if (hostSystems[1].guestSystem_Connected())
        {
            hostSystems[1].guestSystem.DeSelect_All_Slots();
        }
    }

    // currentItem and bag host systems control for equipping
}
