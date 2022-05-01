using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    hand, back, throwable
}

[CreateAssetMenu(menuName = "Create New Item")]
public class Item_Info : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int itemMaxAmount;
}
