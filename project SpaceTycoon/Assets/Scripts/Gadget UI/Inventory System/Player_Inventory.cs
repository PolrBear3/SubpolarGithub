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
        Add_JetPack();
        Input_Unlock_Bag_Slot();
    }

    public Gadget_MainController controller;
    public Static_Box_Display staticDisplay;

    public Animator openmenuButton;
    public GameObject inventoryMenu;
    public GameObject mouseItemIcon;
    
    public void Open_Inventory_Menu()
    {
        controller.Open_Gadget_Menu(inventoryMenu, openmenuButton);
    }

    public void Close_Inventory_Menu()
    {
        controller.Close_All_Gadget_Menus(openmenuButton);
    }

    // craft jetpack test function
    public List<Item_Info> items = new List<Item_Info>();
    void Add_JetPack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var boxHolder = GetComponent<Box_Holder>();
            boxHolder.boxSystem.Add_to_Box(items[0], 1);
        }
    }

    // unlock Bag Level test function
    void Input_Unlock_Bag_Slot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            staticDisplay.Unlock_Slot_Level(2);
        }
    }
}
