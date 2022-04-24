using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Back Item")]
public class Back_Info : Default_Info
{
    public int maxFuel;

    public void Awake()
    {
        itemType = ItemType.back;
    }
}
