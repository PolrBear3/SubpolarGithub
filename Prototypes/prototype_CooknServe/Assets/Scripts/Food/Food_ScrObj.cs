using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "New Food!")]
public class Food_ScrObj : ScriptableObject
{
    public Sprite emptySprite;
    public Sprite sprite;
    public string foodName;
    public int id;

    public List<Ingredient> ingredients = new();
}