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
        GlobalTime_Controller.TimeTik_Update -= TimeTik_Update;
    }


    // Data Control
    public bool Is_SameFood(Food_ScrObj compareFood)
    {
        if (_hasFood == false) return false;
        if (compareFood != _currentData.foodScrObj) return false;

        return true;
    }


    public List<FoodData> AllDatas()
    {
        List<FoodData> foodDatas = new List<FoodData>(_subDatas);

        if (_currentData == null) return foodDatas;
        foodDatas.Add(_currentData);

        return foodDatas;
    }

    public void Update_AllDatas(List<FoodData> updateDatas)
    {
        _currentData = null;
        Set_CurrentData(updateDatas[updateDatas.Count - 1]);

        updateDatas.RemoveAt(updateDatas.Count - 1);
        _subDatas = updateDatas;
    }


    private void Handle_EmptyData()
    {
        _currentData = null;

        if (_subDatas.Count > 0)
        {
            _currentData = new FoodData(_subDatas[_subDatas.Count - 1]);
            _subDatas.RemoveAt(_subDatas.Count - 1);

            return;
        }

        _hasFood = false;
        GlobalTime_Controller.TimeTik_Update -= TimeTik_Update;
    }

    public void Set_CurrentData(FoodData setData)
    {
        if (setData == null)
        {
            Handle_EmptyData();
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
        GlobalTime_Controller.TimeTik_Update += TimeTik_Update;
    }

    public void Swap_Data(FoodData_Controller otherController)
    {
        FoodData saveData = _currentData;

        _currentData = null;
        Set_CurrentData(otherController.currentData);

        otherController._currentData = null;
        otherController.Set_CurrentData(saveData);
    }


    private void TimeTik_Update()
    {
        _currentData.Update_TikCount(1);

        TimeTikEvent?.Invoke();
    }


    // Sub Data
    public void Add_SubData(FoodData subData)
    {
        if (subData == null) return;

        _subDatas.Add(new(subData));
    }


    // All
    public void Toggle_Height(bool toggle)
    {
        if (toggle)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, _toggleHeight);
            return;
        }

        transform.localPosition = new Vector2(transform.localPosition.x, _defaultHeight);
    }


    // Icon
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
        _foodIcon.transform.localPosition = _currentData.foodScrObj.centerPosition / 100f;
        _foodIcon.color = Color.white;
    }

    public void Hide_Icon()
    {
        _foodIcon.color = Color.clear;
        _foodIcon.sprite = null;
    }


    // Amount Bar
    public void Show_AmountBar()
    {
        // empty
        if (_hasFood == false || _currentData.currentAmount <= 0)
        {
            _amountBar.Toggle(false);
            return;
        }

        _amountBar.Toggle(true);

        int maxAmount = _amountBar.amountBarSprite.Length;

        // full bar check
        if (_currentData.currentAmount >= maxAmount)
        {
            _amountBar.Load(maxAmount);
            return;
        }

        // update bar sprite
        _amountBar.Load(_currentData.currentAmount);
    }

    public void Toggle_AmountBar(bool toggle)
    {
        if (toggle == false)
        {
            _amountBar.Toggle(false);
            return;
        }

        Show_AmountBar();
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