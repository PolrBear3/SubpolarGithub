using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ItemSlot_Cursor : MonoBehaviour
{
    private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;


    [Header("")]
    [SerializeField] private ItemSlots_Controller _slotsController;

    [Header("")]
    [SerializeField] private Image _cursorImage;
    public Image cursorImage => _cursorImage;

    [SerializeField] private TextMeshProUGUI _amountText;

    [Header("")]
    [SerializeField] private UI_ClockTimer _holdTimer;
    public UI_ClockTimer holdTimer => _holdTimer;


    private ItemSlot_Data _data;
    public ItemSlot_Data data => _data;

    private ItemSlot _currentSlot;
    public ItemSlot currentSlot => _currentSlot;


    // UnityEngine
    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Empty_Item();
    }


    // Data
    public void Assign_Data(ItemSlot_Data data)
    {
        _data = data;
    }


    // Visual Cotrol
    public void Empty_Item()
    {
        _data = new();
        _data.Empty_Item();

        _cursorImage.rectTransform.anchoredPosition = Vector2.zero;

        _data.currentAmount = 0;
        _amountText.text = "";
    }


    public void Update_SlotIcon()
    {
        if (_data.stationData != null)
        {
            _cursorImage.sprite = _data.stationData.stationScrObj.dialogIcon;
            _cursorImage.color = Color.white;
            _cursorImage.rectTransform.anchoredPosition = new Vector2(0f, -6.5f);

            return;
        }

        if (_data.currentFood != null)
        {
            _cursorImage.sprite = _data.currentFood.sprite;
            _cursorImage.color = Color.white;
            _cursorImage.rectTransform.anchoredPosition = _data.currentFood.uiCenterPosition;

            return;
        }

        Empty_Item();
    }

    public void Update_AmountText()
    {
        if (_data.hasItem == false) return;

        if (_data.currentAmount <= 0)
        {
            Empty_Item();
            return;
        }

        _amountText.text = _data.currentAmount.ToString();
    }


    // Navigate Control
    public ItemSlot Navigated_NextSlot(Vector2 direction)
    {
        Vector2 currentGridNum = new Vector2(_currentSlot.gridNum.x, _currentSlot.gridNum.y);
        Vector2 nextGridNum = new Vector2(currentGridNum.x + direction.x, currentGridNum.y + direction.y);

        ItemSlot nextSlot = _slotsController.ItemSlot(nextGridNum);
        return nextSlot;
    }

    public void Navigate_toSlot(ItemSlot assignSlot)
    {
        if (assignSlot == null) return;
        _currentSlot = assignSlot;

        _rectTransform.SetParent(_currentSlot.cursorPoint);
        _rectTransform.anchoredPosition = Vector2.zero;
    }
}
