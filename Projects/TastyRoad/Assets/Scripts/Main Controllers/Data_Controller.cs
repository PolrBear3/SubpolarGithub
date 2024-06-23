using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    void UnInteract();
}

public interface ISignal
{
    void Signal();
}

public class Data_Controller : MonoBehaviour
{
    [Header("")] 
    public List<Location_ScrObj> locations = new();
    public List<Station_ScrObj> stations = new();
    public List<GameObject> characters = new();

    [Header("")]
    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();

    [Header("")]
    [SerializeField] private List<Coin_ScrObj> _coinTypes = new();
    public List<Coin_ScrObj> coinTypes => _coinTypes;


    // Check if State Data Matches (checks if visitor data matches home data)
    public bool FoodCondition_DataMatch(List<FoodCondition_Data> visitorData, List<FoodCondition_Data> homeData)
    {
        // state count match check
        if (visitorData.Count != homeData.Count) return false;

        int matchCount = homeData.Count;

        for (int i = 0; i < visitorData.Count; i++)
        {
            for (int j = 0; j < homeData.Count; j++)
            {
                // type check 
                if (visitorData[i].type != homeData[j].type) continue;
                // level check
                if (visitorData[i].level != homeData[j].level) continue;

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
    public Station_ScrObj Station_ScrObj(Station_ScrObj stationScrObj)
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stationScrObj != stations[i]) continue;
            return stations[i];
        }
        return null;
    }
    public Station_ScrObj Station_ScrObj(string stationName)
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stationName != stations[i].stationName) continue;
            return stations[i];
        }
        return null;
    }

    /// <returns>
    /// Random Station
    /// </returns>
    public Station_ScrObj Station_ScrObj(bool onlyRetrievable)
    {
        List<Station_ScrObj> stations = new List<Station_ScrObj>();

        for (int i = 0; i < this.stations.Count; i++)
        {
            if (onlyRetrievable == true && this.stations[i].unRetrievable) continue;
            stations.Add(this.stations[i]);
        }

        Station_ScrObj randStation = stations[Random.Range(0, stations.Count)];

        return randStation;
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

    public Food_ScrObj Food()
    {
        return AllFoods()[Random.Range(0, AllFoods().Count)];
    }
    public Food_ScrObj Food(int foodID)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodID != rawFoods[i].id) continue;
            return rawFoods[i];
        }

        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodID != cookedFoods[i].id) continue;
            return cookedFoods[i];
        }

        return null;
    }
    public Food_ScrObj Food(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < rawFoods.Count; i++)
        {
            if (foodScrObj != rawFoods[i]) continue;
            return rawFoods[i];
        }

        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodScrObj != cookedFoods[i]) continue;
            return cookedFoods[i];
        }

        return null;
    }


    // Get Raw Food
    public Food_ScrObj RawFood()
    {
        return rawFoods[Random.Range(0, rawFoods.Count)];
    }
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
    public Food_ScrObj CookedFood()
    {
        return cookedFoods[Random.Range(0, cookedFoods.Count)];
    }
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

    public Food_ScrObj CookedFood(FoodData_Controller dataController1, FoodData_Controller dataController2)
    {
        if (dataController1.hasFood == false || dataController2.hasFood == false) return null;

        // search for cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(dataController1.currentData);
        ingredients.Add(dataController2.currentData);

        Food_ScrObj cookedFood = CookedFood(ingredients);

        return cookedFood;
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
                if (FoodCondition_DataMatch(foodData[i].conditionDatas, cookedFood.ingredients[j].conditionDatas) == false) return null;
            }
        }

        return cookedFood;
    }
}