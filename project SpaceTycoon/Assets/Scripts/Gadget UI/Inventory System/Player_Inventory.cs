using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    private void Start()
    {
        inventoryMenu.SetActive(false);
        mouseItemIcon.SetActive(true);
    }

    public Gadget_MainController controller;
    public GameObject inventoryMenu;
    public GameObject mouseItemIcon;
    
    public void Open_Inventory_Menu()
    {
        inventoryMenu.SetActive(true);
    }

    public void Close_Inventory_Menu()
    {
        controller.Close_All_Menu();
    }

    private void Update()
    {
        Add_JetPack();
    }

    public List<Item_Info> items = new List<Item_Info>();

    void Add_JetPack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var boxHolder = GetComponent<Box_Holder>();
            boxHolder.boxSystem.Add_to_Box(items[0], 1);
        }
    }
}
