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
        Unlock_Bag_Level1_Slot();
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
        inventoryMenu.SetActive(true);
        openmenuButton.SetBool("isPressed", true);
    }

    public void Close_Inventory_Menu()
    {
        controller.Close_All_Menu();
        openmenuButton.SetBool("isPressed", false);
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
            Unlock_Bag_Level2_Slot();
        }
    }
    
    void Unlock_Bag_Level1_Slot()
    {
        staticDisplay.Unlock_Bag_Level(0, 4, 1);
    }
    void Unlock_Bag_Level2_Slot()
    {
        staticDisplay.Unlock_Bag_Level(4, 8, 2);
    }
}
