using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag_Slot : MonoBehaviour
{
    [SerializeField] private Inventory_Controller _inventoryController;

    private RectTransform rectTransform;
    
    [SerializeField] private Image _itemImage;
    [SerializeField] private Text _amountText;

    private Item_ScrObj _currentItem;
    public Item_ScrObj currentItem { get => _currentItem; set => _currentItem = value; }

    private bool _hasItem;
    public bool hasItem { get => _hasItem; set => _hasItem = value; }

    private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    [SerializeField] private Slot _previousSlot;

    private bool _itemDragging = false;
    public bool itemDragging { get => _itemDragging; set => _itemDragging = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out RectTransform rectTransform)) { this.rectTransform = rectTransform; }
    }
    private void Update()
    {
        Drag_Item();
    }

    // Check System
    public bool Is_Same_Item(Item_ScrObj item)
    {
        if (_currentItem == null) return false;
        if (item != _currentItem) return false;
        return true;
    }
    public bool Is_Max_Amount()
    {
        if (_currentItem == null) return false;
        if (_currentAmount < _currentItem.maxAmount) return false;
        return true;
    }

    // Functions
    public void Save_Previous_Slot(Slot slot)
    {
        _previousSlot = slot;
    }

    public void Assign(Item_ScrObj item, int amount)
    {
        _hasItem = true;

        _currentItem = item;
        _currentAmount = amount;

        // Calculate_LeftOver();

        _itemImage.sprite = item.sprite;
        _itemImage.color = Color.white;

        _amountText.text = _currentAmount.ToString();
        Color textColor = _amountText.color;
        textColor.a = 1f;
        _amountText.color = textColor;

        _itemDragging = true;
    }
    public void Clear()
    {
        _hasItem = false;

        _currentItem = null;
        _currentAmount = 0;

        _itemImage.sprite = null;
        _itemImage.color = Color.clear;

        Color textColor = _amountText.color;
        textColor.a = 0f;
        _amountText.color = textColor;

        _itemDragging = false;
    }

    public void Drag_Item()
    {
        if (!_itemDragging) return;
        
        Vector2 mousePosition = Input.mousePosition;
        Vector2 canvasPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, mousePosition, null, out canvasPosition);

        rectTransform.anchoredPosition = canvasPosition;
    }
}