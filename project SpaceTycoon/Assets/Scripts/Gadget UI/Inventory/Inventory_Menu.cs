using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Menu : MonoBehaviour
{
    public Gadget_MainController gadgetController;
    public Inventory_MainController inventoryController;

    public void Menu_On()
    {
        if (Gadget_MainController.isGadgetMenuOn == false)
        {
            inventoryController.inventoryMenu.SetActive(true);
            inventoryController.anim.SetBool("isPressed", true);
            Gadget_MainController.isGadgetMenuOn = true;
        }
    }

    public void Menu_Off()
    {
        gadgetController.TurnOff_All_Menu();
        inventoryController.anim.SetBool("isPressed", false);
    }
}
