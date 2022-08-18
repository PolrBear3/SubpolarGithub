using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    hand, back, throwable
}

public enum ItemType2
{
    item, ingredient
}

[System.Serializable]
public class Ingredient
{
    public Item_Info itemInfo;
    public int amount;
}

[CreateAssetMenu(menuName = "Create New Item")]
public class Item_Info : ScriptableObject
{
    public ItemType itemType;
    public ItemType2 itemType2;

    public Sprite itemIcon;
    public Sprite UIitemIcon;
    public string itemName;
    [TextArea (2,5)]
    public string itemDescription;

    public Ingredient[] ingredients;

    public int itemMaxAmount;
    public int itemMaxDurability;
}
