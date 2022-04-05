using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory_Item : MonoBehaviour
{
    public New_Item newItem;
    public int stackSize;

    public Inventory_Item(New_Item item)
    {
        newItem = item;
        Addto_Stack();
    }

    public void Addto_Stack()
    {
        stackSize++;
    }

    public void Removefrom_Stack()
    {
        stackSize--;
    }
}
