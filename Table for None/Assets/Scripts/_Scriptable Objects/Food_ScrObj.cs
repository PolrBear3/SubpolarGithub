using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Food!")]
public class Food_ScrObj : ScriptableObject
{
    public bool demoBuild;
    
    [Space(20)]
    public Sprite sprite;
    public Sprite eatSprite;
    
    public Color spriteColor;

    [Space(20)]
    public string foodName;
    [SerializeField] private LocalizedString _localizedString;
    
    [Space(20)]
    public int id;

    [Space(20)]
    [Range(0, 999)] public int price;

    [Space(20)]
    public List<FoodData> ingredients = new();
    public FoodCondition_Type[] restrictedCondtions;

    [Space(20)]
    [Range(0, 999)] public int unlockAmount;
    public FoodData[] unlocks;


    // Gets
    public string LocalizedName()
    {
        if (_localizedString == null) return foodName;
        if (string.IsNullOrEmpty(_localizedString.TableReference) && string.IsNullOrEmpty(_localizedString.TableEntryReference)) return foodName;
        
        return _localizedString.GetLocalizedString();
    }
    
    
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

        if (ingredients.Count <= 0)
        {
            foods.Add(this);
        }
        else
        {
            foreach (FoodData data in ingredients)
            {
                foods.Add(data.foodScrObj);
            }
        }

        return foods;
    }

    public List<FoodData> Conditioned_Ingredients()
    {
        if (ingredients.Count <= 0) return null;
        
        List<FoodData> ingredientDatas = new();

        foreach (FoodData data in ingredients)
        {
            ingredientDatas.Add(data);
        }

        return ingredientDatas;
    }


    public bool Has_Ingredient(Food_ScrObj ingredient)
    {
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (ingredient != ingredients[i].foodScrObj) continue;
            return true;
        }

        return false;
    }
    
    public bool Ingredients_Match(List<FoodData> ingredientDatas)
    {
        List<FoodData> datas = new(ingredientDatas);
        
        for (int i = 0; i < ingredients.Count; i++)
        {
            for (int j = datas.Count - 1; j >= 0; j--)
            {
                if (datas[j] == null) continue;
                if (datas[j].foodScrObj != ingredients[i].foodScrObj) continue;
                if (!ingredients[i].Conditions_Match(datas[j].conditionDatas)) continue;

                datas.RemoveAt(j);
            }
        }

        return datas.Count <= 0;
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