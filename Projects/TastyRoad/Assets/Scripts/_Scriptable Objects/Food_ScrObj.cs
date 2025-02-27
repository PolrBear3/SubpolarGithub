using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Food!")]
public class Food_ScrObj : ScriptableObject
{
    [Header("")]
    public Sprite sprite;
    public Sprite eatSprite;

    [Header("")]
    public string foodName;
    public int id;

    [Header("")]
    [Range(0, 999)] public int price;

    [Header("")]
    public List<FoodData> ingredients = new();
    public FoodCondition_Type[] restrictedCondtions;

    [Header("")]
    [Range(0, 999)] public int unlockAmount;
    public FoodData[] unlocks;


    public List<FoodCondition_Type> Available_SetConditions()
    {
        List<FoodCondition_Type> conditions = new();

        foreach (FoodCondition_Type condition in Enum.GetValues(typeof(FoodCondition_Type)))
        {
            if (condition == FoodCondition_Type.rotten) continue;
            if (restrictedCondtions.Contains(condition)) continue;

            conditions.Add(condition);
        }

        return conditions;
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

    public bool Ingredients_Match(List<FoodData> ingredientDatas)
    {
        for (int i = 0; i < ingredients.Count; i++)
        {
            for (int j = ingredientDatas.Count - 1; j >= 0; j--)
            {
                if (ingredientDatas[j] == null) continue;
                if (ingredientDatas[j].foodScrObj != ingredients[i].foodScrObj) continue;
                if (!ingredients[i].Conditions_Match(ingredientDatas[j].conditionDatas)) continue;

                ingredientDatas.RemoveAt(j);
            }
        }

        return ingredientDatas.Count <= 0;
    }


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
    /// all ingredients of unlocks with no duplicates
    /// </returns>
    public List<Food_ScrObj> Unlocks_Ingredients()
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