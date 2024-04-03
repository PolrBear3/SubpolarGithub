using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public ItemSlot_Data data;

    private int _boxNum;
    public int boxNum => _boxNum;

    private Vector2 _gridNum;
    public Vector2 gridNum => _gridNum;

    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;



    [Header("")]
    [SerializeField] private Transform _cursorPoint;
    public Transform cursorPoint => _cursorPoint;

    [SerializeField] private Image _bookmarkIcon;



    [Header("")]
    [SerializeField] private GameObject _stateIndicator;
    [SerializeField] private List<StateBox_Sprite> stateBoxSprites = new();
    [SerializeField] private List<Image> stateBoxImages = new();



    // UnityEngine
    private void Awake()
    {
        _iconImage.color = Color.clear;
        _amountText.color = Color.clear;
        _bookmarkIcon.color = Color.clear;

        _stateIndicator.SetActive(false);
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
    public void Assign_BoxNum(int setNum)
    {
        _boxNum = setNum;
    }
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



    // StateBox Image Control
    private Sprite StateBox_Sprite(FoodState_Type type, int level)
    {
        for (int i = 0; i < stateBoxSprites.Count; i++)
        {
            if (stateBoxSprites[i].type == type)
            {
                return stateBoxSprites[i].boxSprites[level - 1];
            }
        }
        return null;
    }
    public void Assign_State(List<FoodState_Data> stateData)
    {
        if (stateData.Count <= 0) return;

        _stateIndicator.SetActive(true);

        for (int i = 0; i < stateBoxImages.Count; i++)
        {
            stateBoxImages[i].sprite = null;
            stateBoxImages[i].color = Color.clear;
        }

        for (int i = 0; i < stateData.Count; i++)
        {
            for (int j = 0; j < stateBoxImages.Count; j++)
            {
                if (stateBoxImages[j].sprite != null) continue;

                stateBoxImages[j].sprite = StateBox_Sprite(stateData[i].stateType, stateData[i].stateLevel);
                stateBoxImages[i].color = Color.white;
            }
        }
    }
}