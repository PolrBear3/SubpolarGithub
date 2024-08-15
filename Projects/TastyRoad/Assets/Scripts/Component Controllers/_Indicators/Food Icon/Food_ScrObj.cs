using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "New Food!")]
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
    private Food_ScrObj food_ScrObj;

    public Food_ScrObj(Food_ScrObj food_ScrObj)
    {
        this.food_ScrObj = food_ScrObj;
    }
}