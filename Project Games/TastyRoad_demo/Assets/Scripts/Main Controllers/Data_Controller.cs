using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public class Data_Controller : MonoBehaviour
{
    public List<GameObject> locations = new();
    public List<GameObject> stations = new();
    public List<GameObject> characters = new();

    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();

    // Check if State Data Matches (checks if visitor data matches home data)
    public bool Food_StateData_Match(List<FoodState_Data> visitorData, List<FoodState_Data> homeData)
    {
        // state count match check
        if (visitorData.Count != homeData.Count) return false;

        int matchCount = homeData.Count;

        for (int i = 0; i < visitorData.Count; i++)
        {
            for (int j = 0; j < homeData.Count; j++)
            {
                // type check 
                if (visitorData[i].stateType != homeData[j].stateType) continue;
                // level check
                if (visitorData[i].stateLevel != homeData[j].stateLevel) continue;

                // match found !
                matchCount--;
            }
        }

        if (matchCount > 0) return false;
        else return true;
    }

    // Get Raw Food
    public Food_ScrObj RawFood(int foodID)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodID != rawFoods[i].id) continue;
            return rawFoods[i];
        }
        return null;
    }

    public Food_ScrObj RawFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodScrObj != rawFoods[i]) continue;
            return rawFoods[i];
        }
        return null;
    }

    // Get Cooked Food
    public Food_ScrObj CookedFood(int foodID)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodID != cookedFoods[i].id) continue;
            return cookedFoods[i];
        }
        return null;
    }

    public Food_ScrObj CookedFood(List<Food_ScrObj> foods)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            List<Food_ScrObj> cookedFoodIngredients = new();

            if (foods.Count != cookedFoods[i].ingredients.Count) continue;

            // create ingredient check list
            for (int j = 0; j < cookedFoods[i].ingredients.Count; j++)
            {
                cookedFoodIngredients.Add(cookedFoods[i].ingredients[j].foodScrObj);
            }

            // if match, remove from check list
            for (int j = 0; j < foods.Count; j++)
            {
                if (foods[j] == null) continue;
                if (!cookedFoodIngredients.Contains(foods[j])) continue;

                cookedFoodIngredients.Remove(foods[j]);
            }

            // if check list is 0, add to matchCookedFoods list
            if (cookedFoodIngredients.Count > 0) continue;

            return cookedFoods[i];
        }

        return null;
    }

    public Food_ScrObj CookedFood(List<FoodData> foodData)
    {
        // get cooked food
        List<Food_ScrObj> ingredients = new();

        for (int i = 0; i < foodData.Count; i++)
        {
            ingredients.Add(foodData[i].foodScrObj);
        }

        Food_ScrObj cookedFood = CookedFood(ingredients);

        if (cookedFood == null) return null;

        // state check
        for (int i = 0; i < foodData.Count; i++)
        {
            List<FoodData> ingredientsData = cookedFood.ingredients;

            for (int j = 0; j < ingredientsData.Count; j++)
            {
                if (foodData[i].foodScrObj != cookedFood.ingredients[j].foodScrObj) continue;
                if (Food_StateData_Match(foodData[i].stateData, cookedFood.ingredients[j].stateData) == false) return null;
            }
        }

        return cookedFood;
    }
}