using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Inventory")]
public class Inventory : ScriptableObject
{
    public List<Item> inventoryStorage = new List<Item>();

    // Add Item in list
    public void Add_Item(Default_Info info, int addAmount)
    {
        bool inStorage = false;

        for (int i = 0; i < inventoryStorage.Count; i++)
        {
            // if the same item is in storage and the amount is not max amount
            if (inventoryStorage[i].info == info &&
                inventoryStorage[i].currentAmount != inventoryStorage[i].info.maxAmount)
            {
                inventoryStorage[i].Increase_Amount(addAmount);
                inStorage = true;
                break;
            }
        }
        // if there is 0 item in the storage, create new item in storage
        if (!inStorage)
        {
            inventoryStorage.Add(new Item(info, addAmount));
        }
    }

    // Subtract Item from list
    public void Subtract_Item(Default_Info info, int subtractAmount)
    {
        for (int i = 0; i < inventoryStorage.Count; i++)
        {
            if (inventoryStorage[i].info == info)
            {
                inventoryStorage[i].Decrease_Amount(subtractAmount);
            }
            if (inventoryStorage[i].currentAmount == 0)
            {
                inventoryStorage.Remove(inventoryStorage[i]);
            }
        }
    }
}