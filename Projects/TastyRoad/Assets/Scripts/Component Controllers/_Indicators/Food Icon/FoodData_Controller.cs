using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ConditionSprites
{
    [SerializeField] private FoodCondition_Type _type;
    public FoodCondition_Type type => _type;

    [SerializeField] private Sprite[] _sprites;
    public Sprite[] sprites => _sprites;
}

public class FoodData_Controller : MonoBehaviour
{
    private bool _hasFood;
    public bool hasFood => _hasFood;

    private FoodData _currentData;
    public FoodData currentData => _currentData;

    private List<FoodData> _subDatas = new();
    public List<FoodData> subDatas => _subDatas;


    public delegate void Event();
    public event Event TimeTikEvent;

    public event Event OnFoodShow;
    public event Event OnFoodHide;


    [Header("")]
    [SerializeField] private SpriteRenderer _foodIcon;
    public SpriteRenderer foodIcon => _foodIcon;

    [SerializeField] private AmountBar _amountBar;
    public AmountBar amountBar => _amountBar;


    [Header("")]
    [SerializeField] private SpriteRenderer[] _conditionBoxes;

    [SerializeField] private ConditionSprites[] _conditionSprites;
    public ConditionSprites[] conditionSprites => _conditionSprites;


    [Header("")]
    [SerializeField] private float _toggleHeight;
    private float _defaultHeight;

    [SerializeField] private bool _iconShowLocked;


    [Header("")]
    [SerializeField][Range(0, 100)] private int _maxDataCount;
    public int maxDataCount => _maxDataCount;

    [SerializeField][Range(0, 100)] private int _maxAmount;
    public int maxAmount => _maxAmount;


    [SerializeField][Range(0, 100)] private int _tikCountValue;

    private bool _tikCountLocked;


    // UnityEngine
    private void Awake()
    {
        _defaultHeight = transform.localPosition.y;
    }

    private void Start()
    {
        Show_Icon();
        Show_Condition();
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.instance.OnTimeTik -= TimeTik_Update;
    }


    // Data Control
    public List<FoodData> AllDatas()
    {
        List<FoodData> foodDatas = new(_subDatas);

        if (_currentData == null) return foodDatas;
        foodDatas.Add(_currentData);

        return foodDatas;
    }

    public void Update_AllDatas(List<FoodData> updateDatas)
    {
        // clear all data
        if (updateDatas == null)
        {
            _subDatas.Clear();
            Set_CurrentData(null);

            return;
        }

        if (updateDatas.Count <= 0) return;

        _currentData = null;
        Set_CurrentData(updateDatas[updateDatas.Count - 1]);

        updateDatas.RemoveAt(updateDatas.Count - 1);
        _subDatas = updateDatas;
    }


    /// <returns>
    /// empty executed data
    /// </returns>
    public FoodData Empty_TargetData(Food_ScrObj targetDataFood)
    {
        FoodData removeData = null;

        if (_hasFood == false) return removeData;

        if (_currentData.foodScrObj == targetDataFood)
        {
            removeData = new(_currentData);
            Empty_CurrentData();

            return removeData;
        }

        for (int i = 0; i < _subDatas.Count; i++)
        {
            if (_subDatas[i].foodScrObj != targetDataFood) continue;

            removeData = new(_subDatas[i]);
            _subDatas.RemoveAt(i);

            return removeData;
        }

        return removeData;
    }


    // Current Data
    public void Set_CurrentData(FoodData setData)
    {
        if (setData == null)
        {
            Empty_CurrentData();
            return;
        }

        bool hadFood = _hasFood;

        // push current food to sub data
        if (_hasFood)
        {
            Add_SubData(_currentData);
        }

        // set data
        _currentData = new FoodData(setData);

        if (hadFood) return;

        _hasFood = true;
        GlobalTime_Controller.instance.OnTimeTik += TimeTik_Update;
    }

    private void Empty_CurrentData()
    {
        _currentData = null;

        if (AllDatas().Count > 0)
        {
            _currentData = new FoodData(_subDatas[_subDatas.Count - 1]);
            _subDatas.RemoveAt(_subDatas.Count - 1);

            return;
        }

        _hasFood = false;
        GlobalTime_Controller.instance.OnTimeTik -= TimeTik_Update;
    }

    public void Swap_Data(FoodData_Controller otherController)
    {
        FoodData saveData = _currentData;

        _currentData = null;
        Set_CurrentData(otherController.currentData);

        otherController._currentData = null;
        otherController.Set_CurrentData(saveData);
    }


    public bool Is_SameFood(Food_ScrObj compareFood)
    {
        if (compareFood == null || _hasFood == false) return false;
        if (compareFood != _currentData.foodScrObj) return false;

        return true;
    }

    public bool Is_MaxAmount()
    {
        if (hasFood == false) return false;
        return _currentData.currentAmount >= _maxAmount;
    }
    public bool Is_MaxAmount(int updateAmount)
    {
        if (hasFood == false) return false;
        return _currentData.currentAmount + updateAmount > _maxAmount;
    }

    public int Current_Amount()
    {
        if (_hasFood == false) return 0;
        return _currentData.currentAmount;
    }


    public void Update_Amount(Food_ScrObj targetFood, int updateValue)
    {
        if (_hasFood == false || _currentData.foodScrObj != targetFood)
        {
            Set_CurrentData(new(targetFood, updateValue));
            return;
        }

        _currentData.Update_Amount(updateValue);

        if (_currentData.currentAmount > 0) return;
        Set_CurrentData(null);
    }


    // Time Tik
    public void Toggle_TikCount(bool toggle)
    {
        _tikCountLocked = !toggle;
    }

    private void TimeTik_Update()
    {
        if (_tikCountLocked)
        {
            TimeTikEvent?.Invoke();
            return;
        }

        List<FoodData> allDatas = AllDatas();

        foreach (FoodData data in allDatas)
        {
            data.Update_TikCount(_tikCountValue);
        }
        Update_AllDatas(allDatas);

        TimeTikEvent?.Invoke();
    }


    // Sub Data
    public bool Has_SameFood(Food_ScrObj compareFood)
    {
        for (int i = 0; i < AllDatas().Count; i++)
        {
            if (AllDatas()[i].foodScrObj != compareFood) continue;
            return true;
        }
        return false;
    }

    public int FoodCount(Food_ScrObj countFood)
    {
        int foodCount = 0;

        for (int i = 0; i < AllDatas().Count; i++)
        {
            if (AllDatas()[i].foodScrObj != countFood) continue;
            foodCount++;
        }

        return foodCount;
    }


    public bool DataCount_Maxed()
    {
        return AllDatas().Count >= _maxDataCount;
    }

    public int Empty_DataCount()
    {
        int currentDataCount = AllDatas().Count;
        int emptyDataCount = _maxDataCount - currentDataCount;

        Mathf.Clamp(emptyDataCount, 0, _maxDataCount);

        return emptyDataCount;
    }


    public void SetMax_SubDataCount(int setValue)
    {
        _maxDataCount = setValue;
    }

    public void Add_SubData(FoodData subData)
    {
        if (subData == null) return;

        _subDatas.Add(new(subData));
    }


    // Indications
    public void Toggle_Height(bool toggle)
    {
        if (toggle)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, _toggleHeight);
            return;
        }

        transform.localPosition = new Vector2(transform.localPosition.x, _defaultHeight);
    }


    public void ShowIcon_LockToggle(bool isLock)
    {
        _iconShowLocked = isLock;

        Show_Icon();
    }


    public void Show_Icon()
    {
        if (_iconShowLocked == true || _hasFood == false)
        {
            Hide_Icon();
            return;
        }

        if (_currentData.foodScrObj == null)
        {
            Set_CurrentData(null);
            Hide_Icon();
            return;
        }

        _foodIcon.sprite = _currentData.foodScrObj.sprite;
        _foodIcon.color = Color.white;

        OnFoodShow?.Invoke();
    }

    public void Show_Icon(float transparencyValue)
    {
        Show_Icon();

        if (_iconShowLocked == true || _hasFood == false) return;

        Main_Controller.instance.Change_SpriteAlpha(_foodIcon, transparencyValue);
    }

    public void Show_EatIcon()
    {
        if (_iconShowLocked == true || _hasFood == false)
        {
            Hide_Icon();
            return;
        }

        if (_currentData.foodScrObj == null)
        {
            Set_CurrentData(null);
            Hide_Icon();
            return;
        }

        _foodIcon.sprite = _currentData.foodScrObj.eatSprite;
        _foodIcon.color = Color.white;
    }


    public void Hide_Icon()
    {
        _foodIcon.color = Color.clear;
        _foodIcon.sprite = null;

        OnFoodHide?.Invoke();
    }


    // Amount Bar
    public void Toggle_AmountBar(bool toggle)
    {
        if (hasFood == false || toggle == false)
        {
            _amountBar.Toggle(false);
            return;
        }

        _amountBar.Load_Custom(_maxAmount, _currentData.currentAmount);
        _amountBar.Toggle(true);
    }

    public void Toggle_SubDataBar(bool toggle)
    {
        if (toggle == false || subDatas.Count <= 0)
        {
            _amountBar.Toggle(false);
            return;
        }

        _amountBar.Load_Custom(maxDataCount, AllDatas().Count);
        _amountBar.Toggle(true);
    }


    // Food Condition
    public ConditionSprites Get_ConditionSprites(FoodCondition_Type type)
    {
        for (int i = 0; i < _conditionSprites.Length; i++)
        {
            if (type != _conditionSprites[i].type) continue;

            return _conditionSprites[i];
        }
        return null;
    }


    public void Show_Condition()
    {
        bool conditionEmpty = _hasFood == false || _currentData.conditionDatas == null;

        for (int i = 0; i < _conditionBoxes.Length; i++)
        {
            if (conditionEmpty || i >= _currentData.conditionDatas.Count)
            {
                _conditionBoxes[i].sprite = null;
                _conditionBoxes[i].color = Color.clear;

                continue;
            }

            ConditionSprites conditionSprites = Get_ConditionSprites(_currentData.conditionDatas[i].type);
            int conditionLevel = _currentData.conditionDatas[i].level;

            _conditionBoxes[i].sprite = conditionSprites.sprites[conditionLevel - 1];
            _conditionBoxes[i].color = Color.white;
        }
    }

    public void Hide_Condition()
    {
        foreach (var conditionBox in _conditionBoxes)
        {
            conditionBox.sprite = null;
            conditionBox.color = Color.clear;
        }
    }
}