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

    // Get Position
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