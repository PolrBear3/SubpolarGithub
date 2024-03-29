using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "New Food!")]
public class Food_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public Sprite eatSprite;

    public Vector2 centerPosition;

    public string foodName;
    public int id;

    public int price;

    public List<FoodData> ingredients = new();
    private Food_ScrObj currentFood;

    public Food_ScrObj(Food_ScrObj currentFood)
    {
        this.currentFood = currentFood;
    }
}