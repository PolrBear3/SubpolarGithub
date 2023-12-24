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
    public List<BoxCollider2D> boxAreas = new();
    public List<GameObject> prefabs = new();

    public List<Food_ScrObj> ingredientFoods = new();
    public List<Food_ScrObj> mergedFoods = new();

    // box Area
    public Vector2 Get_BoxArea_Position(int boxAreaNum)
    {
        BoxCollider2D boxArea = boxAreas[boxAreaNum];
        Vector2 center = boxArea.bounds.center;

        float randX = Random.Range(center.x - boxArea.bounds.extents.x, center.x + boxArea.bounds.extents.x);
        float randY = Random.Range(center.y - boxArea.bounds.extents.y, center.y + boxArea.bounds.extents.y);

        Vector2 randPos = new Vector2(randX, randY);
        return randPos;
    }

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
    private bool Is_FoodStateData_Match(List<FoodState_Data> foodsData, List<FoodState_Data> mergedFoodData)
    {
        if (foodsData.Count != mergedFoodData.Count) return false;

        int matchCount = mergedFoodData.Count;

        for (int i = 0; i < foodsData.Count; i++)
        {
            for (int j = 0; j < mergedFoodData.Count; j++)
            {
                if (foodsData[i].stateType != mergedFoodData[j].stateType) continue;
                if (foodsData[i].stateLevel != mergedFoodData[j].stateLevel) continue;
                matchCount--;
            }
        }

        if (matchCount > 0) return false;
        return true;
    }

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

            for (int j = 0; j < insertedIngredients.Count; j++)
            {
                if (insertedIngredients[j] == null) continue;
                if (!mergedFoodIngredients.Contains(insertedIngredients[j])) continue;

                matchCount--;
                mergedFoodIngredients.Remove(insertedIngredients[j]);
            }

            if (matchCount > 0) continue;
            return mergedFoods[i];
        }

        return null;
    }
    public Food_ScrObj Get_MergedFood(List<Food> foods)
    {
        List<Food_ScrObj> ingredients = new();

        for (int i = 0; i < foods.Count; i++)
        {
            ingredients.Add(foods[i].foodScrObj);
        }

        Food_ScrObj mergedFood = Get_MergedFood(ingredients);
        if (mergedFood == null) return null;

        for (int i = 0; i < foods.Count; i++)
        {
            for (int j = 0; j < mergedFood.ingredients.Count; j++)
            {
                if (foods[i].foodScrObj != mergedFood.ingredients[j].foodScrObj) continue;
                if (!Is_FoodStateData_Match(foods[i].data, mergedFood.ingredients[j].data)) return null;
            }
        }

        return mergedFood;
    }
}