using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/New Ingredient")]
public class Ingredient_Info : Default_Info
{
    public void Awake()
    {
        itemType = ItemType.ingredient;
    }
}
