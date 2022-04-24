using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Throwable1 Item")]
public class Throwable1_Info : Default_Info
{
    public void Awake()
    {
        itemType = ItemType.throwable2;
    }
}
