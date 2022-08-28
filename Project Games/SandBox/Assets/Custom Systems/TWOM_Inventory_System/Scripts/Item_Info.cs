using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Ingredients
{
    public Item_Info itemInfo;
    public int amount;
}

[CreateAssetMenu (menuName = "Inventory System/New Item")]
public class Item_Info : ScriptableObject
{
    public Sprite itemSprite;
    public string itemName;
    [TextArea(2,4)]
    public string itemDescription;
    public int itemMaxAmount;

    public Ingredients[] ingredients;
}
