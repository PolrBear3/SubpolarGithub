using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Icon : MonoBehaviour
{
    private void Awake()
    {
        display = GameObject.Find("Inventory Gadget").GetComponent<Inventory_Display>();
    }

    Inventory_Display display;
    public Default_Info itemInfo;
    public GameObject toBagButton, toInventoryButton;

    public void Move_toBag()
    {
        display.Move_fromInventory_toBag(itemInfo, toBagButton, toInventoryButton);
    }

    public void Move_toInventory()
    {
        display.Move_fromBag_toInventory(itemInfo, toBagButton, toInventoryButton);
    }
}