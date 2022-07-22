using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public Gadget_MainController gadgetController;
    
    public Animator openmenuButton;
    public GameObject inventoryMenu;
    
    public Box_Holder boxHolder;

    public void Open_Inventory_Menu()
    {
        gadgetController.Open_Gadget_Menu(inventoryMenu, openmenuButton);
    }

    public void Close_Inventory_Menu()
    {
        gadgetController.Close_All_Gadget_Menus(openmenuButton);
    }
}
