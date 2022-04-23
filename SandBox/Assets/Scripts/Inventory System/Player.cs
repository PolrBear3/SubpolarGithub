using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory_Object inventory;

    public Item_Object[] items = new Item_Object[0];

    public void JetPack_Craft_Button()
    {
        inventory.Add_Item(items[0], 1);
    }
    public void JetPack_Trash_Button()
    {
        inventory.TakeAway_Item(items[0], 1);
    }
}
