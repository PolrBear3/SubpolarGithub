using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public enum FoodState_Type { sliced, heated }

[System.Serializable]
public class FoodState_Data
{
    public FoodState_Type stateType;
    public int stateLevel;
}

[System.Serializable]
public class Ingredient
{
    public Food_ScrObj foodScrObj;
    public List<FoodState_Data> data = new();
}

public class Data_Controller : MonoBehaviour
{
    public List<Food_ScrObj> ingredientFoods = new();
    public List<Food_ScrObj> mergedFoods = new();

    // Ingredient Food
    public Food_ScrObj Get_Food(int foodID)
    {
        for (int i = 0; i < ingredientFoods.Count; i++)
        {
            if (foodID != ingredientFoods[i].id) continue;
            return ingredientFoods[i];
        }
        return null;
    }
    public Food_ScrObj Get_Food(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < ingredientFoods.Count; i++)
        {
            if (foodScrObj != ingredientFoods[i]) continue;
            return ingredientFoods[i];
        }
        return null;
    }

    // Merged Food
    public Food_ScrObj Get_MergedFood(int foodID)
    {
        for (int i = 0; i < mergedFoods.Count; i++)
        {
            if (foodID != mergedFoods[i].id) continue;
            return mergedFoods[i];
        }
        return null;
    }
    public Food_ScrObj Get_MergedFood(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < mergedFoods.Count; i++)
        {
            if (foodScrObj != mergedFoods[i]) continue;
            return mergedFoods[i];
        }
        return null;
    }
    public Food_ScrObj Get_MergedFood(List<Food_ScrObj> foodIngredients)
    {
        for (int i = 0; i < mergedFoods.Count; i++)
        {
            List<Food_ScrObj> insertedIngredients = foodIngredients;
            List<Food_ScrObj> mergedFoodIngredients = new();
            int matchCount = mergedFoods[i].ingredients.Count;

            if (insertedIngredients.Count != matchCount) continue;

            for (int j = 0; j < mergedFoods[i].ingredients.Count; j++)
            {
                mergedFoodIngredients.Add(mergedFoods[i].ingredients[j].foodScrObj);
            }

            for (int k = 0; k < insertedIngredients.Count; k++)
            {
                if (!mergedFoodIngredients.Contains(insertedIngredients[k])) continue;

                matchCount--;
                mergedFoodIngredients.Remove(insertedIngredients[k]);
            }

            if (matchCount > 0) continue;
            return mergedFoods[i];
        }

        return null;
    }
}