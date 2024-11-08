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
    private FoodData _headData;
    public FoodData headData => _headData;

    private bool _hasFood;
    public bool hasFood => _hasFood;

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


    // Current Data
    public bool Is_SameFood(Food_ScrObj compareFood)
    {
        if (_hasFood == false) return false;
        if (compareFood != _headData.foodScrObj) return false;

        return true;
    }


    public void Set_CurrentData(FoodData data)
    {
        // update to null if empty data
        if (data == null)
        {
            _headData = null;
            _hasFood = false;

            GlobalTime_Controller.TimeTik_Update -= TimeTik_Update;

            return;
        }

        // increase amount if same food
        if (hasFood == true && data.foodScrObj == _headData.foodScrObj)
        {
            _headData.Update_Amount(data.currentAmount);

            return;
        }

        // set data
        _headData = new FoodData(data);
        _hasFood = true;

        GlobalTime_Controller.TimeTik_Update += TimeTik_Update;
    }

    public void Swap_Data(FoodData_Controller otherController)
    {
        FoodData otherData = otherController._headData;

        // set to null before swap to restrict amount increase in Set_CurrentData(FoodData data)

        // other controller data
        otherController.Set_CurrentData(null);
        otherController.Set_CurrentData(_headData);

        // this controller data
        Set_CurrentData(null);
        Set_CurrentData(otherData);
    }


    private void TimeTik_Update()
    {
        _headData.Update_TikCount(1);

        TimeTikEvent?.Invoke();
    }


    // Sub Data
    public void Add_SubData(FoodData subData)
    {
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

        if (_headData.foodScrObj == null)
        {
            Set_CurrentData(null);
            Hide_Icon();
            return;
        }

        _foodIcon.sprite = _headData.foodScrObj.sprite;
        _foodIcon.transform.localPosition = _headData.foodScrObj.centerPosition / 100f;
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
        if (_hasFood == false || _headData.currentAmount <= 0)
        {
            _amountBar.Toggle(false);
            return;
        }

        _amountBar.Toggle(true);

        int maxAmount = _amountBar.amountBarSprite.Length;

        // full bar check
        if (_headData.currentAmount >= maxAmount)
        {
            _amountBar.Load(maxAmount);
            return;
        }

        // update bar sprite
        _amountBar.Load(_headData.currentAmount);
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
        bool conditionEmpty = _hasFood == false || _headData.conditionDatas == null;

        for (int i = 0; i < _conditionBoxes.Length; i++)
        {
            if (conditionEmpty || i >= _headData.conditionDatas.Count)
            {
                _conditionBoxes[i].sprite = null;
                _conditionBoxes[i].color = Color.clear;

                continue;
            }

            ConditionSprites conditionSprites = Get_ConditionSprites(_headData.conditionDatas[i].type);
            int conditionLevel = _headData.conditionDatas[i].level;

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