using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Food!")]
public class Food_ScrObj : ScriptableObject
{
    [Header("")]
    public Sprite sprite;
    public Sprite eatSprite;

    [Header("")]
    public Vector2 centerPosition;
    public Vector2 uiCenterPosition;

    [Header("")]
    public string foodName;
    public int id;

    [Header("")]
    public int price;
    public List<FoodData> ingredients = new();

    //
    public List<Food_ScrObj> Ingredients()
    {
        List<Food_ScrObj> foods = new();

        // is Raw Food
        if (ingredients.Count <= 0)
        {
            foods.Add(this);
        }
        // is Cooked Food
        else
        {
            foreach (FoodData data in ingredients)
            {
                foods.Add(data.foodScrObj);
            }
        }

        return foods;
    }
}