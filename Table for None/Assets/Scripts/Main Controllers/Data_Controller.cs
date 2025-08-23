using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Trigger_Interact();
    
    void Interact();
    void Hold_Interact();

    void Action1();
    void Action2();

    void UnInteract();
}

public interface IRestrictable
{
    bool IsRestricted();
}

public class Data_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private WorldData[] _worldData;
    public WorldData[] worldData => _worldData;

    [Space(20)]
    public List<Station_ScrObj> stations = new();
    public List<GameObject> characters = new();

    [Space(20)]
    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();


    // Get World and Location
    public WorldData World_Data(int targetWorldNum)
    {
        int indexNum = targetWorldNum - 1;
        if (indexNum < 0 || indexNum > worldData.Length - 1) return null;

        return _worldData[indexNum];
    }

    public int LocationCount_inWorld(int targetWorldNum)
    {
        int indexNum = targetWorldNum - 1;
        if (indexNum < 0 || indexNum > _worldData.Length - 1) return 0;

        for (int i = 0; i < _worldData.Length; i++)
        {
            if (indexNum != i) continue;
            return _worldData[i].LocationCount();
        }

        return 0;
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
    public Station_ScrObj Station_ScrObj()
    {
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

        if (Main_Controller.instance.demoBuild == false) return allFoods;

        for (int i = allFoods.Count - 1; i >= 0; i--)
        {
            if (allFoods[i].demoBuild) continue;
            allFoods.RemoveAt(i);
        }
        return allFoods;
    }
    public List<Food_ScrObj> AllFoods(List<FoodData> ingredientDatas)
    {
        List<Food_ScrObj> allFoods = AllFoods();

        for (int i = allFoods.Count - 1; i >= 0; i--)
        {
            if (AllFoods()[i].Ingredients_Match(ingredientDatas)) continue;
            allFoods.RemoveAt(i);
        }

        return allFoods;
    }
    /// <returns>
    /// cooked foods that has ingredient
    /// </returns>
    public List<Food_ScrObj> AllFoods(Food_ScrObj ingredient)
    {
        List<Food_ScrObj> foods = new();
        
        for (int i = 0; i < AllFoods().Count; i++)
        {
            if (Is_RawFood(AllFoods()[i])) continue;
            if (AllFoods()[i].Has_Ingredient(ingredient) == false) continue;
            
            foods.Add(AllFoods()[i]);
        }
        return foods;
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
    public Food_ScrObj Food(List<FoodData> ingredientDatas)
    {
        List<Food_ScrObj> allFoods = AllFoods();
        
        for (int i = 0; i < allFoods.Count; i++)
        {
            if (allFoods[i].Ingredients_Match(ingredientDatas) == false) continue;
            return allFoods[i];
        }

        return null;
    }
    

    // Get Raw Food
    public Food_ScrObj RawFood()
    {
        return rawFoods[Random.Range(0, rawFoods.Count)];
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
    public Food_ScrObj CookedFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodScrObj != cookedFoods[i]) continue;
            return cookedFoods[i];
        }
        return null;
    }
    
    /// <summary>
    /// Check if State Data Matches (checks if visitor data matches home data)
    /// </summary>
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
}