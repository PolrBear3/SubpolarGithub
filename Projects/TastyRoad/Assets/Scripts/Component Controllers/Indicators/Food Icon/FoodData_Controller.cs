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
    private FoodData _currentData;
    public FoodData currentData => _currentData;

    private bool _hasFood;
    public bool hasFood => _hasFood;

    public delegate void Event();
    public event Event TimeTikEvent;

    [Header("")]
    [SerializeField] private SpriteRenderer _foodIcon;
    [SerializeField] private SpriteRenderer _amountBar;
    [SerializeField] private SpriteRenderer[] _conditionBoxes;

    [Header("")]
    [SerializeField] private Sprite[] _amountBarSprites;

    [Header("")]
    [SerializeField] private ConditionSprites[] _conditionSprites;
    public ConditionSprites[] conditionSprites => _conditionSprites;

    [Header("")]
    [SerializeField] private bool _iconShowLocked;
    [SerializeField] private bool _barShowLocked;

    [SerializeField] private float _durationTime;
    private Coroutine _amountBarCoroutine;


    // UnityEngine
    private void Start()
    {
        Show_Icon();
        Show_AmountBar();
        Show_Condition();
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= TimeTik_Update;
    }


    // Current Data
    public void Set_CurrentData(FoodData data)
    {
        // update to null if empty data
        if (data == null)
        {
            _currentData = null;
            _hasFood = false;

            GlobalTime_Controller.TimeTik_Update -= TimeTik_Update;

            return;
        }

        // increase amount if same food
        if (hasFood == true && data.foodScrObj == _currentData.foodScrObj)
        {
            _currentData.Update_Amount(data.currentAmount);

            return;
        }

        // set data
        _currentData = new FoodData(data);
        _hasFood = true;

        GlobalTime_Controller.TimeTik_Update += TimeTik_Update;
    }

    public void Swap_Data(FoodData_Controller otherController)
    {
        FoodData otherData = otherController.currentData;

        // set to null before swap to restrict amount increase in Set_CurrentData(FoodData data)

        // other controller data
        otherController.Set_CurrentData(null);
        otherController.Set_CurrentData(_currentData);

        // this controller data
        Set_CurrentData(null);
        Set_CurrentData(otherData);
    }


    // Lock Toggles
    public void ShowIcon_LockToggle(bool isLock)
    {
        _iconShowLocked = isLock;

        Show_Icon();
    }

    public void ShowAmountBar_LockToggle(bool isLock)
    {
        _barShowLocked = isLock;

        Show_AmountBar();
    }


    // Icon
    public void Show_Icon()
    {
        if (_iconShowLocked == true || _hasFood == false)
        {
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
        if (_barShowLocked == true || _hasFood == false || _currentData.currentAmount <= 0)
        {
            _amountBar.color = Color.clear;
            return;
        }

        _amountBar.color = Color.white;

        int maxAmount = _amountBarSprites.Length;

        // full bar check
        if (_currentData.currentAmount >= maxAmount)
        {
            _amountBar.sprite = _amountBarSprites[maxAmount - 1];
            return;
        }

        // update bar sprite
        _amountBar.sprite = _amountBarSprites[_currentData.currentAmount - 1];
    }

    public void Show_AmountBar_Duration()
    {
        if (_amountBarCoroutine != null)
        {
            StopCoroutine(_amountBarCoroutine);
            _amountBarCoroutine = null;
        }

        _amountBarCoroutine = StartCoroutine(Show_AmountBar_Duration_Coroutine());
    }
    private IEnumerator Show_AmountBar_Duration_Coroutine()
    {
        Show_AmountBar();

        yield return new WaitForSeconds(_durationTime);

        _amountBar.color = Color.clear;
    }


    // Time Tik
    private void TimeTik_Update()
    {
        _currentData.Update_TikCount(1);

        TimeTikEvent?.Invoke();
    }


    // Food Condition
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
    private ConditionSprites Get_ConditionSprites(FoodCondition_Type type)
    {
        for (int i = 0; i < _conditionSprites.Length; i++)
        {
            if (type != _conditionSprites[i].type) continue;

            return _conditionSprites[i];
        }
        return null;
    }
}