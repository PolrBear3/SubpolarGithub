using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public Gadget_MainController gadgetController;
    public Host_System bag;
    
    public Animator openmenuButton;
    public GameObject inventoryMenuGameObject;

    public void Open_Inventory_Menu()
    {
        gadgetController.Open_Gadget_Menu(inventoryMenuGameObject, openmenuButton);

        // deselect all slots if guest system is connected and the slot is currently selected
        if (bag.guestSystem_Connected())
        {
            bag.guestSystem.DeSelect_All_Slots();
        }
    }
    public void Close_Inventory_Menu()
    {
        gadgetController.Close_All_Gadget_Menus(openmenuButton);

        // Deselect all slots when closing out of inventory
        bag.DeSelect_All_Slots();

        // deselect all slots if guest system is connected and the slot is currently selected
        if (bag.guestSystem_Connected())
        {
            bag.guestSystem.DeSelect_All_Slots();
        }
    }
}
