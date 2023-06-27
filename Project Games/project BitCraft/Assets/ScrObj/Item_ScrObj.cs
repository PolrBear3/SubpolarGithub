using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    public Item_ScrObj ingredientItem;
    public int amount;
}

[CreateAssetMenu (menuName = "New Item")]
public class Item_ScrObj : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public string itemName;
    public int maxAmount;
    public List<Ingredient> ingredients;
}
