using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public Gadget_MainController controller;
    
    public Animator openmenuButton;
    public GameObject inventoryMenu;
    
    public Box_Holder boxHolder;

    private void Start()
    {
        // test
        boxHolder.Unlock_Slot_Level(1);
    }

    private void Update()
    {
        // test
        Add_Item();
        Input_Unlock_Bag_Slot();
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

    // unlock Bag Level test function
    void Input_Unlock_Bag_Slot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            boxHolder.Unlock_Slot_Level(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            boxHolder.Unlock_Slot_Level(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            boxHolder.Unlock_Slot_Level(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            boxHolder.Unlock_Slot_Level(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            boxHolder.Unlock_Slot_Level(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            boxHolder.Unlock_Slot_Level(6);
        }
    }
}
