using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { current, back, throwable1, throwable2, ingredient }

public class Default_Info : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int maxAmount;
}