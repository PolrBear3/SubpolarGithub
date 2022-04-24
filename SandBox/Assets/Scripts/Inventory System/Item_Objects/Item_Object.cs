using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum itemType { current, back, throwable1, throwable2 }

[CreateAssetMenu(menuName = "New Item")]
public class Item_Object : ScriptableObject
{
    public GameObject itemPrefab;
    public itemType itemType;
    public string itemName;
}
