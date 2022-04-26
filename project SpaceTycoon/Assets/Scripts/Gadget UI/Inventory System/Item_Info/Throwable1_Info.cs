using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Throwable Item")]
public class Throwable_Info : Default_Info
{
    public void Awake()
    {
        itemType = ItemType.throwable;
    }
}
