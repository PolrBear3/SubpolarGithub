using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;

    private ItemSlot_Data _data;
    public ItemSlot_Data data => _data;

    private Vector2 _gridNum;
    public Vector2 gridNum => _gridNum;


    [Header("")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _bookmarkIcon;

    [SerializeField] private TextMeshProUGUI _amountText;


    [Header("")]
    [SerializeField] private RectTransform _cursorPoint;
    public RectTransform cursorPoint => _cursorPoint;


    [Header("")]
    [SerializeField] private GameObject _bookmarkUnlockedIcon;
    [SerializeField] private GameObject _ingredientUnlockedIcon;

    [SerializeField] [Range(0, 1)] private float _transparentValue;


    // UnityEngine
    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();

        _iconImage.color = Color.clear;
        _bookmarkIcon.color = Color.clear;
    }


    // Mini Icon Control
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


    // Unlock Control
    public void Toggle_Lock(bool isLock)
    {
        if (data.hasItem == false) return;

        _data.isLocked = isLock;

        // transparency control
        if (isLock == true)
        {
            Main_Controller.Change_ImageAlpha(_iconImage, _transparentValue);
        }
        else
        {
            Main_Controller.Change_ImageAlpha(_iconImage, 1f);
        }
    }

    /// <summary>
    /// Micro bookmark and ingredient icons
    /// </summary>
    public void Toggle_Icons(bool bookMark, bool ingredient)
    {
        _bookmarkUnlockedIcon.SetActive(bookMark);
        _ingredientUnlockedIcon.SetActive(ingredient);
    }


    // Data
    public void Assign_GridNum(Vector2 setNum)
    {
        _gridNum = setNum;
    }

    public void Assign_Data(ItemSlot_Data data)
    {
        _data = data;
    }


    //
    public void Empty_ItemBox()
    {
        _data = new();

        _data.bookMarked = false;
        Toggle_BookMark(false);

        _data.hasItem = false;
        _data.isLocked = false;

        Toggle_Icons(false, false);

        _data.currentFood = null;
        _data.currentStation = null;

        _iconImage.sprite = null;
        _iconImage.color = Color.clear;
        _iconImage.rectTransform.anchoredPosition = Vector2.zero;

        _data.currentAmount = 0;
        _amountText.text = "";
    }


    // sprite update included
    public ItemSlot Assign_Item(Food_ScrObj food)
    {
        if (food != null)
        {
            _data.hasItem = true;
            _data.currentFood = food;

            _iconImage.sprite = food.sprite;

            _iconImage.color = Color.white;
            _iconImage.rectTransform.anchoredPosition = food.uiCenterPosition;

            return this;
        }

        Empty_ItemBox();
        return this;
    }
    public ItemSlot Assign_Item(Station_ScrObj station)
    {
        if (station != null)
        {
            data.hasItem = true;
            data.currentStation = station;

            _iconImage.sprite = station.miniSprite;

            _iconImage.color = Color.white;
            _iconImage.transform.localPosition = station.centerPosition;

            return this;
        }

        Empty_ItemBox();
        return this;
    }

    /// <summary>
    /// Assigns current data item
    /// </summary>
    public ItemSlot Assign_Item()
    {
        if (_data.hasItem == false)
        {
            Empty_ItemBox();
            return this;
        }

        if (_data.currentFood != null)
        {
            Assign_Item(_data.currentFood);
            return this;
        }

        Assign_Item(_data.currentStation);
        return this;
    }


    // text update included
    public void Assign_Amount(int assignAmount)
    {
        data.currentAmount = assignAmount;
        _amountText.text = _data.currentAmount.ToString();

        if (data.currentAmount > 0) return;
        Empty_ItemBox();
    }
    public void Update_Amount(int updateAmount)
    {
        data.currentAmount += updateAmount;
        _amountText.text = _data.currentAmount.ToString();

        if (data.currentAmount > 0) return;
        Empty_ItemBox();
    }
}