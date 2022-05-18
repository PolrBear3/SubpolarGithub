using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    private void Start()
    {
        inventoryMenu.SetActive(false);
        mouseItemIcon.SetActive(true);

        // test
        staticDisplay.Unlock_Slot_Level(1);
    }

    private void Update()
    {
        // test
        Add_Item();
        Input_Unlock_Bag_Slot();
    }

    public Gadget_MainController controller;
    public Static_Box_Display staticDisplay;
    public Box_Holder boxHolder;

    public Animator openmenuButton;
    public GameObject inventoryMenu, mouseItemIcon;
    
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
            boxHolder.Craft_Item(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            boxHolder.Craft_Item(1, 1);
        }
    }

    // unlock Bag Level test function
    void Input_Unlock_Bag_Slot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            staticDisplay.Unlock_Slot_Level(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            staticDisplay.Unlock_Slot_Level(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            staticDisplay.Unlock_Slot_Level(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            staticDisplay.Unlock_Slot_Level(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            staticDisplay.Unlock_Slot_Level(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            staticDisplay.Unlock_Slot_Level(6);
        }
    }
}
