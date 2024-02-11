using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    void UnInteract();
}

public class Data_Controller : MonoBehaviour
{
    private Main_Controller _mainController;

    public List<GameObject> locations = new();
    public List<Station_ScrObj> stations = new();
    public List<GameObject> characters = new();

    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();



    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Main_Controller mainController)) { _mainController = mainController; }
    }



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



    // Get Station
    public Station_ScrObj Station_ScrObj(int stationID)
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stationID != stations[i].id) continue;
            return stations[i];
        }
        return null;
    }



    // Get All Food
    public List<Food_ScrObj> AllFoods()
    {
        List<Food_ScrObj> allFoods = new();

        // add all raw foods
        for (int i = 0; i < rawFoods.Count; i++)
        {
            allFoods.Add(rawFoods[i]);
        }

        // add all cooked foods
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            allFoods.Add(cookedFoods[i]);
        }

        return allFoods;
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

    public bool Is_RawFood(Food_ScrObj foodScrObj)
    {
        if (foodScrObj == null) return false;

        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodScrObj == rawFoods[i]) return true;
        }
        return false;
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
    public Food_ScrObj CookedFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodScrObj != cookedFoods[i]) continue;
            return cookedFoods[i];
        }
        return null;
    }
    public Food_ScrObj CookedFood(List<Food_ScrObj> foods)
    {
        for (int i = 0; i < AllFoods().Count; i++)
        {
            List<Food_ScrObj> cookedFoodIngredients = new();

            if (foods.Count != AllFoods()[i].ingredients.Count) continue;

            // create ingredient check list
            for (int j = 0; j < AllFoods()[i].ingredients.Count; j++)
            {
                cookedFoodIngredients.Add(AllFoods()[i].ingredients[j].foodScrObj);
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

            return AllFoods()[i];
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