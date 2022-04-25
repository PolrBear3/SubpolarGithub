using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Bag")]
public class Bag : ScriptableObject
{
    public List<Item> storage = new List<Item>();

    // Add Item in list
    public void Add_Item(Default_Info info, int addAmount)
    {
        bool inStorage = false;

        for (int i = 0; i < storage.Count; i++)
        {
            // if the same item is in storage and the amount is not max amount
            if (storage[i].info == info && 
                storage[i].currentAmount != storage[i].info.maxAmount) 
            {
                storage[i].Increase_Amount(addAmount);
                inStorage = true;
                break;
            }
        }

        // if there is 0 item in the storage, create new item in storage
        if (!inStorage)
        {
            storage.Add(new Item(info, addAmount));
        }
    }

    // Subtract Item from list
    public void Subtract_Item(Default_Info info, int subtractAmount)
    {
        for (int i = 0; i < storage.Count; i++)
        {
            if (storage[i].info == info)
            {
                storage[i].Decrease_Amount(subtractAmount);
                
                if (storage[i].currentAmount == 0)
                {
                    storage.Remove(storage[i]);
                    break;
                }
                break;
            }
        }
    }
}