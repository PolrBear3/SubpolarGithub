using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { current, back, throwable, ingredient }

public class Default_Info : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public GameObject itemIcon;
    public int maxAmount;
}