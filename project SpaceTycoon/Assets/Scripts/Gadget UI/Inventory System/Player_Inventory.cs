using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public Gadget_MainController controller;
    
    public Animator openmenuButton;
    public GameObject inventoryMenu;
    
    public Box_Holder boxHolder;

    private void Update()
    {
        // test
        Add_Item();
    }

    public void Open_Inventory_Menu()
    {
        controller.Open_Gadget_Menu(inventoryMenu, openmenuButton);
    }

    public void Close_Inventory_Menu()
    {
        controller.Close_All_Gadget_Menus(openmenuButton);
    }

    // craft jetpack test function
    void Add_Item()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            boxHolder.Craft_Item(ItemType2.ingredient, 0 , 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            boxHolder.Craft_Item(ItemType2.item, 0, 1);
        }
    }
}
