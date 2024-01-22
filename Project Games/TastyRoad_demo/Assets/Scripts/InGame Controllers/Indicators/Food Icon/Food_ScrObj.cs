using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "New Food!")]
public class Food_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public Sprite eatSprite;

    public string foodName;
    public int id;

    public List<FoodData> ingredients = new();
}