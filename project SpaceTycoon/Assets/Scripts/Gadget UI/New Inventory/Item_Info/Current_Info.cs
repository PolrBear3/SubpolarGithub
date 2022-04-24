using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Current Item")]
public class Current_Info : Default_Info
{
    public int maxFuel;

    public void Awake()
    {
        itemType = ItemType.current;
    }
}
