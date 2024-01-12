using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public enum FoodState_Type { sliced, heated }

[System.Serializable]
public class State_Data
{
    public FoodState_Type stateType;
    public int stateLevel;
}

[System.Serializable]
public class Ingredient
{
    public Food_ScrObj foodScrObj;
    public List<State_Data> data = new();
}

public class Data_Controller : MonoBehaviour
{
    public List<GameObject> locations = new();
    public List<GameObject> stations = new();
    public List<GameObject> characters = new();

    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();

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
    public Food_ScrObj CookedFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            if (foodScrObj != cookedFoods[i]) continue;
            return cookedFoods[i];
        }
        return null;
    }
}