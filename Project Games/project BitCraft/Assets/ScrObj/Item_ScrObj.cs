using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "New Item")]
public class Item_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public string itemName;
    public int maxAmount;
}
