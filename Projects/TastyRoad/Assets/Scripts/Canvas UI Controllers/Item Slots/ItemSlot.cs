using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public ItemSlot_Data data;

    private Vector2 _gridNum;
    public Vector2 gridNum => _gridNum;

    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;



    [Header("")]
    [SerializeField] private Transform _cursorPoint;
    public Transform cursorPoint => _cursorPoint;

    [SerializeField] private Image _bookmarkIcon;



    [Header("")]
    [SerializeField] private GameObject _conditionIndicator;

    [SerializeField] private ConditionSprites[] _conditionSprites;
    [SerializeField] private Image[] _conditionBoxes;



    // UnityEngine
    private void Awake()
    {
        _iconImage.color = Color.clear;
        _amountText.color = Color.clear;
        _bookmarkIcon.color = Color.clear;

        _conditionIndicator.SetActive(false);
    }



    // Mini Icon control
    public void Toggle_BookMark()
    {
        if (data.hasItem == false) return;

        if (data.bookMarked == false)
        {
            data.bookMarked = true;
            _bookmarkIcon.color = Color.white;

            return;
        }

        data.bookMarked = false;
        _bookmarkIcon.color = Color.clear;
    }
    public void Toggle_BookMark(bool toggleOn)
    {
        if (data.hasItem == false) return;

        if (toggleOn)
        {
            data.bookMarked = true;
            _bookmarkIcon.color = Color.white;

            return;
        }

        data.bookMarked = false;
        _bookmarkIcon.color = Color.clear;
    }



    // Assign Data
    public void Assign_GridNum(Vector2 setNum)
    {
        _gridNum = setNum;
    }



    //
    public void Empty_ItemBox()
    {
        data.bookMarked = false;

        Toggle_BookMark(false);
        data.hasItem = false;

        data.currentFood = null;
        data.currentStation = null;

        _iconImage.sprite = null;
        _iconImage.color = Color.clear;

        data.currentAmount = 0;
        _amountText.color = Color.clear;
    }



    // sprite update included
    public void Assign_Item(Food_ScrObj food)
    {
        if (food != null)
        {
            data.hasItem = true;
            data.currentFood = food;

            _iconImage.sprite = food.sprite;

            _iconImage.color = Color.white;
            _iconImage.transform.localPosition = food.centerPosition;

            return;
        }

        Empty_ItemBox();
    }
    public void Assign_Item(Station_ScrObj station)
    {
        if (station != null)
        {
            data.hasItem = true;
            data.currentStation = station;

            _iconImage.sprite = station.miniSprite;

            _iconImage.color = Color.white;
            _iconImage.transform.localPosition = station.centerPosition;

            return;
        }

        Empty_ItemBox();
    }



    // text update included
    public void Assign_Amount(int assignAmount)
    {
        data.currentAmount = assignAmount;

        if (data.currentAmount <= 0)
        {
            Empty_ItemBox();
            return;
        }

        if (data.currentAmount == 1)
        {
            _amountText.color = Color.clear;
            return;
        }

        _amountText.text = data.currentAmount.ToString();
        _amountText.color = Color.black;
    }
    public void Update_Amount(int updateAmount)
    {
        data.currentAmount += updateAmount;

        if (data.currentAmount <= 0)
        {
            Empty_ItemBox();
            return;
        }

        if (data.currentAmount == 1)
        {
            _amountText.color = Color.clear;
            return;
        }

        _amountText.text = data.currentAmount.ToString();
        _amountText.color = Color.black;
    }



    // Condition Indication Control
    private Sprite ConditionBox_Sprite(FoodCondition_Type type, int level)
    {
        for (int i = 0; i < _conditionSprites.Length; i++)
        {
            if (_conditionSprites[i].type == type)
            {
                return _conditionSprites[i].sprites[level - 1];
            }
        }
        return null;
    }
    public void Assign_State(List<FoodCondition_Data> conditionData)
    {
        _conditionIndicator.SetActive(true);

        for (int i = 0; i < _conditionBoxes.Length; i++)
        {
            if (i > conditionData.Count - 1)
            {
                _conditionBoxes[i].color = Color.clear;
                continue;
            }

            _conditionBoxes[i].color = Color.white;
            _conditionBoxes[i].sprite = ConditionBox_Sprite(conditionData[i].type, conditionData[i].level);
        }
    }
}