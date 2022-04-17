using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Menu : MonoBehaviour
{
    private void Update()
    {
        Change_Bag_Test_Function();
    }

    public Gadget_MainController gadgetController;
    public Inventory_MainController inventoryController;

    public void Menu_On()
    {
        gadgetController.TurnOff_All_Menu();
        inventoryController.inventoryMenu.SetActive(true);
        inventoryController.anim.SetBool("isPressed", true);
        Gadget_MainController.isGadgetMenuOn = true;
    }

    public void Menu_Off()
    {
        gadgetController.TurnOff_All_Menu();
        inventoryController.anim.SetBool("isPressed", false);
    }

    // modify this part when 'item modify table' is made
    void Change_Bag_Test_Function()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventoryController.Reset_Bag_Level();
            inventoryController.bagLevel1.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventoryController.Reset_Bag_Level();
            inventoryController.bagLevel2.SetActive(true);
            foreach (var slots in inventoryController.Level2BagSlots)
            {
                slots.SetActive(true);
            }
        }
    }
}
