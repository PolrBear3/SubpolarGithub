using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Bag")]
public class Bag : ScriptableObject
{
    public List<Item> bagStorage = new List<Item>();

    // Add Item in list
    public void Add_Item(Default_Info info, int addAmount)
    {
        bool inStorage = false;

        for (int i = 0; i < bagStorage.Count; i++)
        {
            // if the same item is in storage and the amount is not max amount
            if (bagStorage[i].info == info && 
                bagStorage[i].currentAmount != bagStorage[i].info.maxAmount) 
            {
                bagStorage[i].Increase_Amount(addAmount);
                inStorage = true;
                break;
            }
        }

        // if there is 0 item in the storage, create new item in storage
        if (!inStorage)
        {
            bagStorage.Add(new Item(info, addAmount));
        }
    }

    // Subtract Item from list
    public void Subtract_Item(Default_Info info, int subtractAmount)
    {
        for (int i = 0; i < bagStorage.Count; i++)
        {
            if (bagStorage[i].info == info)
            {
                bagStorage[i].Decrease_Amount(subtractAmount);
            }
            if (bagStorage[i].currentAmount == 0)
            {
                bagStorage.Remove(bagStorage[i]);
            }
        }
    }
}