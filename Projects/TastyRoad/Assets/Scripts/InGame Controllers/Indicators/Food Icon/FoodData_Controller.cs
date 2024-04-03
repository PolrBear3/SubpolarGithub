using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_Controller : MonoBehaviour
{
    public StateBox_Controller stateBoxController;

    [SerializeField] private SpriteRenderer _icon;

    [Header("")]
    [SerializeField] private float _displayTime;
    [SerializeField] private SpriteRenderer _amountBar;
    [SerializeField] private List<Sprite> _amountBarSprites = new();
    private Coroutine _amountBarCoroutine;

    [HideInInspector] public FoodData currentFoodData;

    private bool _hasFood;
    public bool hasFood => _hasFood;

    [Header("")]
    [Range(1, 6)]
    [SerializeField] private int _maxAmount;
    public int maxAmount => _maxAmount;

    [SerializeField] private bool _iconTransparent;
    [SerializeField] private bool _amountTransparent;



    // UnityEngine
    private void Start()
    {
        if (_hasFood == true) return; 

        _icon.color = Color.clear;
        _amountBar.color = Color.clear;
    }



    // Transparency Toggle
    public void FoodIcon_Transparency(bool isTransparent)
    {
        _iconTransparent = isTransparent;

        if (_iconTransparent == false && _hasFood == true)
        {
            _icon.color = Color.white;

            return;
        }

        _icon.color = Color.clear;
    }
    public void AmountBar_Transparency(bool isTransparent)
    {
        _amountTransparent = isTransparent;

        if (_amountTransparent == false && currentFoodData.currentAmount > 0)
        {
            Update_AmountBar();
            _amountBar.color = Color.white;
        }
        else
        {
            _amountBar.color = Color.clear;
        }
    }



    //
    public void Load_FoodData()
    {
        // empty food assigned
        if (currentFoodData.foodScrObj == null)
        {
            Clear_Food();
            return;
        }

        _hasFood = true;

        // food sprite update
        _icon.transform.localPosition = currentFoodData.foodScrObj.centerPosition / 100;
        _icon.sprite = currentFoodData.foodScrObj.sprite;

        FoodIcon_Transparency(_iconTransparent);
        AmountBar_Transparency(_amountTransparent);
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

        // new food update
        _hasFood = true;

        currentFoodData.currentAmount = 1;
        currentFoodData.foodScrObj = foodScrObj;

        // food sprite update
        _icon.transform.localPosition = foodScrObj.centerPosition / 100;
        _icon.sprite = currentFoodData.foodScrObj.sprite;

        if (_iconTransparent == true) return;

        _icon.color = Color.white;
    }
    public void Clear_Food()
    {
        _hasFood = false;

        currentFoodData.foodScrObj = null;
        currentFoodData.currentAmount = 0;

        _icon.color = Color.clear;
        _icon.sprite = null;

        _amountBar.color = Color.clear;
    }



    // Amount Control
    public int Assign_Amount(int assignAmount)
    {
        if (_hasFood == false) return assignAmount;

        if (assignAmount > _maxAmount)
        {
            int leftOver = assignAmount - _maxAmount;
            currentFoodData.currentAmount = _maxAmount;

            return leftOver;
        }

        currentFoodData.currentAmount = assignAmount;

        if (currentFoodData.currentAmount <= 0)
        {
            Clear_Food();

            return 0;
        }

        AmountBar_Transparency(_amountTransparent);

        return 0;
    }
    public int Update_Amount(int updateAmount)
    {
        if (_hasFood == false) return updateAmount;

        int calculatedAmount = currentFoodData.currentAmount + updateAmount;
        int leftOver = calculatedAmount - _maxAmount;

        if (leftOver >= 1)
        {
            currentFoodData.currentAmount = _maxAmount;

            return leftOver;
        }

        currentFoodData.currentAmount = calculatedAmount;

        if (currentFoodData.currentAmount <= 0)
        {
            Clear_Food();

            return 0;
        }

        AmountBar_Transparency(_amountTransparent);

        return 0;
    }

    // Amount Bar Control
    private void Update_AmountBar()
    {
        if (currentFoodData.currentAmount <= 0) return;
        if (currentFoodData.currentAmount > _maxAmount) return;

        _amountBar.sprite = _amountBarSprites[currentFoodData.currentAmount - 1];
    }

    public void Show_AmountBar()
    {
        if (_amountBarCoroutine != null)
        {
            StopCoroutine(_amountBarCoroutine);
        }

        _amountBarCoroutine = StartCoroutine(Show_CurrentAmount_Coroutine());
    }
    private IEnumerator Show_CurrentAmount_Coroutine()
    {
        AmountBar_Transparency(false);

        yield return new WaitForSeconds(_displayTime);

        AmountBar_Transparency(true);
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

    public int StateData_MatchCount(List<FoodState_Data> stateData)
    {
        List<FoodState_Data> thisStateData = currentFoodData.stateData;

        int matchCount = 0;

        if (thisStateData.Count < stateData.Count)
        {
            matchCount = thisStateData.Count - stateData.Count;
        }

        for (int i = 0; i < stateData.Count; i++)
        {
            for (int j = 0; j < thisStateData.Count; j++)
            {
                if (stateData[i].stateType != thisStateData[j].stateType) continue;
                if (stateData[i].stateLevel != thisStateData[j].stateLevel) continue;

                matchCount++;
            }
        }

        return matchCount;
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
}