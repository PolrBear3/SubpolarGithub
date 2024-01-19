using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateBox_Sprite
{
    public bool isTransparent;

    public FoodState_Type type;
    public List<Sprite> boxSprite = new();
}

public class StateBox_Controller : MonoBehaviour
{
    private FoodData_Controller _foodData;

    public List<StateBox_Sprite> stateBoxSprites = new();
    public List<SpriteRenderer> currentStateBoxes = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out FoodData_Controller foodData)) { _foodData = foodData; }
    }

    private void Start()
    {
        Clear_StateBoxes();
    }

    // Get State Box Sprite
    private Sprite StateBox(FoodState_Type type, int level)
    {
        for (int i = 0; i < stateBoxSprites.Count; i++)
        {
            if (stateBoxSprites[i].type == type)
            {
                return stateBoxSprites[i].boxSprite[level - 1];
            }
        }

        return null;
    }

    // Get Current FoodState Data that are Not Transparent
    private List<FoodState_Data> NonTransparent_FoodState_Data()
    {
        List<FoodState_Data> currentData = new(_foodData.currentFoodData.stateData);

        for (int i = 0; i < stateBoxSprites.Count; i++)
        {
            if (stateBoxSprites[i].isTransparent == false) continue;
            currentData.Remove(_foodData.Current_FoodState(stateBoxSprites[i].type));
        }

        return currentData;
    }

    // Clear All State Box
    public void Clear_StateBoxes()
    {
        for (int i = 0; i < currentStateBoxes.Count; i++)
        {
            currentStateBoxes[i].sprite = null;
        }
    }

    // Update All State Box from Current State Data
    public void Update_StateBoxes()
    {
        Clear_StateBoxes();

        List<FoodState_Data> nonTransparentData = NonTransparent_FoodState_Data();

        for (int i = 0; i < nonTransparentData.Count; i++)
        {
            currentStateBoxes[i].sprite = StateBox(nonTransparentData[i].stateType, nonTransparentData[i].stateLevel);
        }
    }
}