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
            Gadget_MainController.isGadgetMenuOn = true;
        }
    }
}
