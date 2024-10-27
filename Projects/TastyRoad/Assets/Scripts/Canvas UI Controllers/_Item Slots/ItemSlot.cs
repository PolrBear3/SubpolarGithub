using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    [SerializeField][Range(0, 1)] private float _transparentValue;


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
        if (toggleOn == false || data.hasItem == false)
        {
            data.bookMarked = false;
            _bookmarkIcon.color = Color.clear;
            return;
        }

        data.bookMarked = true;
        _bookmarkIcon.color = Color.white;
    }


    // Unlock Control
    public void Toggle_Lock(bool isLock)
    {
        if (data.hasItem == false) return;

        _data.isLocked = isLock;

        // transparency control
        if (_data.isLocked == true)
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

    public ItemSlot Assign_Data(ItemSlot_Data data)
    {
        _data = data;

        return this;
    }


    // Visual Control
    public void Empty_ItemBox()
    {
        _data.Empty_Item();

        Toggle_BookMark(false);
        Toggle_Icons(false, false);

        _iconImage.sprite = null;
        _iconImage.color = Color.clear;
        _iconImage.rectTransform.anchoredPosition = Vector2.zero;

        _data.currentAmount = 0;
        _amountText.text = "";
    }


    public ItemSlot Update_SlotIcon()
    {
        if (_data.stationData != null)
        {
            _iconImage.sprite = _data.stationData.stationScrObj.dialogIcon;
            _iconImage.color = Color.white;
            _iconImage.rectTransform.anchoredPosition = new Vector2(0f, -6.5f);

            return this;
        }

        if (_data.currentFood != null)
        {
            _iconImage.sprite = _data.currentFood.sprite;
            _iconImage.color = Color.white;
            _iconImage.rectTransform.anchoredPosition = _data.currentFood.uiCenterPosition;

            return this;
        }

        Empty_ItemBox();
        return this;
    }

    public void AmountText_Update()
    {
        if (_data.currentAmount <= 0)
        {
            Empty_ItemBox();
            return;
        }

        if (_data.currentAmount == 1)
        {
            _amountText.text = "";
            return;
        }

        _amountText.text = _data.currentAmount.ToString();
    }
}