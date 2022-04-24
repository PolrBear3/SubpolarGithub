using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Throwable2 Item")]
public class Throwable2_Info : Default_Info
{
    public void Awake()
    {
        itemType = ItemType.throwable1;
    }
}
