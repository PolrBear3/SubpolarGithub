using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Inventory_Item> inventory = new List<Inventory_Item>();
    private Dictionary<New_Item, Inventory_Item> itemDictionary = new Dictionary<New_Item, Inventory_Item>();

    public void Add(New_Item itemData)
    {
        // if there is the same type of item in stack, just add in to the stack
        if (itemDictionary.TryGetValue(itemData, out Inventory_Item item))
        {
            item.Addto_Stack();
        }
        // if its a new item that is not in the stack
        else
        {
            Inventory_Item newItem = new Inventory_Item(itemData);
            inventory.Add(newItem);
            itemDictionary.Add(itemData, newItem);
        }
    }

    public void Remove(New_Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out Inventory_Item item))
        {
            item.Removefrom_Stack();

            if (item.stackSize == 0)
            {
                inventory.Remove(item);
                itemDictionary.Remove(itemData);
            }
        }
    }
}
