using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Food!")]
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
    [Range(0, 999)] public int price;

    [Header("")]
    public List<FoodData> ingredients = new();

    [Header("")]
    [Range(0, 999)] public int unlockAmount;
    public FoodData[] unlocks;


    public List<Food_ScrObj> Unlocks()
    {
        List<Food_ScrObj> unlockFoods = new();

        foreach (FoodData data in unlocks)
        {
            unlockFoods.Add(data.foodScrObj);
        }

        return unlockFoods;
    }


    /// <returns>
    /// all ingredients Food_ScrObj and if it is a raw food, returns this
    /// </returns>
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

    /// <returns>
    /// all ingredients of unlocks with no duplicates
    /// </returns>
    public List<Food_ScrObj> Unlock_Ingredients()
    {
        List<Food_ScrObj> foods = new();

        for (int i = 0; i < unlocks.Length; i++)
        {
            // is Raw Food
            if (unlocks[i].foodScrObj.ingredients.Count <= 0)
            {
                foods.Add(this);
            }
            // is Cooked Food
            else
            {
                foreach (FoodData food in unlocks[i].foodScrObj.ingredients)
                {
                    if (foods.Contains(food.foodScrObj)) continue;

                    foods.Add(food.foodScrObj);
                }
            }
        }

        return foods;
    }
}