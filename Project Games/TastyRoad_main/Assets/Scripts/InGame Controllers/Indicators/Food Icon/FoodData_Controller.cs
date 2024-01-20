using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodData_Controller : MonoBehaviour
{
    [HideInInspector] public StateBox_Controller stateBoxController;

    private SpriteRenderer _sr;
    [SerializeField] private TMP_Text _amountText;

    [HideInInspector] public FoodData currentFoodData;

    [HideInInspector] public bool hasFood;

    [SerializeField] private bool _iconTransparent;
    [SerializeField] private bool _amountTransparent;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out StateBox_Controller stateBoxController)) { this.stateBoxController = stateBoxController; }
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }

        _sr.color = Color.clear;
        _amountText.color = Color.clear;
    }

    // Transparency Toggle
    public void FoodIcon_Transparency(bool isTransparent)
    {
        _iconTransparent = isTransparent;

        if (_iconTransparent == false && currentFoodData.foodScrObj != null)
        {
            _sr.color = Color.white;
        }
        else
        {
            _sr.color = Color.clear;
        }
    }

    public void AmountText_Transparency(bool isTransparent)
    {
        _amountTransparent = isTransparent;

        if (_amountTransparent == false && currentFoodData.currentAmount > 1)
        {
            _amountText.color = Color.black;
            _amountText.text = currentFoodData.currentAmount.ToString();
        }
        else
        {
            _amountText.color = Color.clear;
        }
    }

    // Food Control
    public void Assign_Food(Food_ScrObj foodScrObj)
    {
        // empty food assigned
        if (foodScrObj == null)
        {
            Clear_Food();
            return;
        }

        // same food assigned
        if (currentFoodData.foodScrObj == foodScrObj)
        {
            Update_Amount(1);
            return;
        }

        // new food update
        hasFood = true;

        currentFoodData.currentAmount = 1;
        currentFoodData.foodScrObj = foodScrObj;

        // transparency control
        if (_iconTransparent == false)
        {
            _sr.sprite = currentFoodData.foodScrObj.sprite;
            _sr.color = Color.white;
        }
    }

    public void Clear_Food()
    {
        hasFood = false;

        currentFoodData.foodScrObj = null;
        currentFoodData.currentAmount = 0;

        _sr.color = Color.clear;
        _sr.sprite = null;

        _amountText.color = Color.clear;
    }

    // Check and Compare Other State Data with this State Data
    public bool Same_StateData(List<FoodState_Data> stateData)
    {
        int matchCount = currentFoodData.stateData.Count;

        if (matchCount != stateData.Count) return false;

        for (int i = 0; i < stateData.Count; i++)
        {
            List<FoodState_Data> thisStateData = currentFoodData.stateData;

            for (int j = 0; j < thisStateData.Count; j++)
            {
                if (stateData[i].stateType != thisStateData[j].stateType) continue;
                if (stateData[i].stateLevel != thisStateData[j].stateLevel) return false;

                matchCount--;
            }
        }

        if (matchCount <= 0) return true;
        else return false;
    }

    // Get State Data
    public FoodState_Data Current_FoodState(FoodState_Type stateType)
    {
        for (int i = 0; i < currentFoodData.stateData.Count; i++)
        {
            if (currentFoodData.stateData[i].stateType != stateType) continue;

            return currentFoodData.stateData[i];
        }

        return null;
    }

    // State Control
    public void Assign_State(List<FoodState_Data> data)
    {
        Clear_State();
        currentFoodData.stateData = data;

        stateBoxController.Update_StateBoxes();
    }

    public void Update_State(FoodState_Type stateType, int level)
    {
        FoodState_Data targetState = Current_FoodState(stateType);

        // new state
        if (targetState == null)
        {
            FoodState_Data newState = new();
            newState.stateType = stateType;
            newState.stateLevel = level;

            currentFoodData.stateData.Add(newState);
        }
        // same state found
        else
        {
            // set maximum state level according to boxsprites
            if (targetState.stateLevel < stateBoxController.stateBoxSprites[0].boxSprites.Count)
            {
                targetState.stateLevel += level;
            }
        }

        stateBoxController.Update_StateBoxes();
    }

    public void Clear_State()
    {
        currentFoodData.stateData.Clear();

        stateBoxController.Clear_StateBoxes();
    }

    // Amount Control
    public void Update_Amount(int updateAmount)
    {
        currentFoodData.currentAmount += updateAmount;

        if (currentFoodData.currentAmount <= 0)
        {
            Clear_Food();
            return;
        }

        if (currentFoodData.currentAmount > 1 && _amountTransparent == false)
        {
            _amountText.color = Color.black;
            _amountText.text = currentFoodData.currentAmount.ToString();
        }
        else
        {
            _amountText.color = Color.clear;
        }
    }
}