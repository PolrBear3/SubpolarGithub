using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateBox_Sprite
{
    public bool isTransparent;

    public FoodState_Type type;
    public List<Sprite> boxSprites = new();
}

public class StateBox_Controller : MonoBehaviour
{
    [SerializeField] private FoodData_Controller _foodData;

    [SerializeField] private List<SpriteRenderer> _currentStateBoxes = new();
    public List<SpriteRenderer> currentStateBoxes => _currentStateBoxes;

    [SerializeField] private List<StateBox_Sprite> _stateBoxSprites;
    public List<StateBox_Sprite> stateBoxSprites => _stateBoxSprites;



    // UnityEngine
    private void Start()
    {
        if (_foodData.currentFoodData.stateData.Count > 0) return;
        Clear_StateBoxes();
    }



    // Get State Box Sprite
    private Sprite StateBox(FoodState_Type type, int level)
    {
        for (int i = 0; i < _stateBoxSprites.Count; i++)
        {
            if (_stateBoxSprites[i].type == type)
            {
                return _stateBoxSprites[i].boxSprites[level - 1];
            }
        }

        return null;
    }

    // Get Current FoodState Data that are not Transparent
    private List<FoodState_Data> NonTransparent_FoodState_Data()
    {
        List<FoodState_Data> currentData = new(_foodData.currentFoodData.stateData);

        for (int i = 0; i < _stateBoxSprites.Count; i++)
        {
            if (_stateBoxSprites[i].isTransparent == false) continue;
            currentData.Remove(_foodData.Current_FoodState(_stateBoxSprites[i].type));
        }

        return currentData;
    }



    // Clear All State Box
    public void Clear_StateBoxes()
    {
        for (int i = 0; i < _currentStateBoxes.Count; i++)
        {
            _currentStateBoxes[i].sprite = null;
        }
    }

    // Update All State Box from Current State Data
    public void Update_StateBoxes()
    {
        Clear_StateBoxes();

        List<FoodState_Data> nonTransparentData = NonTransparent_FoodState_Data();

        for (int i = 0; i < nonTransparentData.Count; i++)
        {
            _currentStateBoxes[i].sprite = StateBox(nonTransparentData[i].stateType, nonTransparentData[i].stateLevel);
        }
    }
}