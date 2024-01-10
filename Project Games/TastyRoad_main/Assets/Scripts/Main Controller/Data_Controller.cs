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
    [SerializeField] private Camera _mainCamera;

    public List<BoxCollider2D> boxAreas = new();

    public List<GameObject> stations = new();
    public List<GameObject> characters = new();

    public List<Food_ScrObj> rawFoods = new();
    public List<Food_ScrObj> cookedFoods = new();

    // Position
    public Vector2 Get_OuterCamera_Position(float horizontal, float vertical, float positionX, float positionY)
    {
        float horizontalPos = horizontal;
        float verticalPos = vertical;

        if (horizontalPos == 0) horizontalPos = 0.5f;
        else if (horizontalPos == -1f) horizontalPos = 0f;

        if (verticalPos == 0) verticalPos = 0.5f;
        else if (verticalPos == -1f) verticalPos = 0f;

        Vector2 cameraPosition = _mainCamera.ViewportToWorldPoint(new Vector2(horizontalPos, verticalPos));

        return new Vector2(cameraPosition.x + positionX, cameraPosition.y + positionY);
    }
    public Vector2 Get_BoxArea_Position(int boxAreaNum)
    {
        BoxCollider2D boxArea = boxAreas[boxAreaNum];
        Vector2 center = boxArea.bounds.center;

        float randX = Random.Range(center.x - boxArea.bounds.extents.x, center.x + boxArea.bounds.extents.x);
        float randY = Random.Range(center.y - boxArea.bounds.extents.y, center.y + boxArea.bounds.extents.y);

        Vector2 randPos = new Vector2(randX, randY);
        return randPos;
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

    // Get Ingredient Food
    private bool Is_FoodStateData_Match(List<State_Data> foodsData, List<State_Data> mergedFoodData)
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
    public Food_ScrObj CookedFood(List<Food_ScrObj> foodIngredients)
    {
        for (int i = 0; i < cookedFoods.Count; i++)
        {
            List<Food_ScrObj> insertedIngredients = foodIngredients;
            List<Food_ScrObj> mergedFoodIngredients = new();
            int matchCount = cookedFoods[i].ingredients.Count;

            if (insertedIngredients.Count != matchCount) continue;

            for (int j = 0; j < cookedFoods[i].ingredients.Count; j++)
            {
                mergedFoodIngredients.Add(cookedFoods[i].ingredients[j].foodScrObj);
            }

            for (int j = 0; j < insertedIngredients.Count; j++)
            {
                if (insertedIngredients[j] == null) continue;
                if (!mergedFoodIngredients.Contains(insertedIngredients[j])) continue;

                matchCount--;
                mergedFoodIngredients.Remove(insertedIngredients[j]);
            }

            if (matchCount > 0) continue;
            return cookedFoods[i];
        }

        return null;
    }
}