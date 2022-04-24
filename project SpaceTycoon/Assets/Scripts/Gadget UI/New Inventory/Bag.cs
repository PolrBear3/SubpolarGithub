using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Bag")]
public class Bag : ScriptableObject
{
    public List<Item> bagStorage = new List<Item>();

    // Add Item in list
    public void Add_Item(Item item, int addAmount)
    {
        bool inStorage = false;

        for (int i=0; i < bagStorage.Count; i++)
        {
            // if the same item is in bag, increase the amount of it
            if (bagStorage[i] == item)
            {
                inStorage = true;
                bagStorage[i].Increase_Amount(addAmount);
                break;
            }

        }
    }

    // Subtract Item from list
    public void Subtract_Item(Item item, int subtractAmount)
    {

    }

    // move to Inventory
}
