using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Drag_Slot : MonoBehaviour
{
    [SerializeField] private Inventory_Controller _inventoryController;

    private RectTransform _rectTransform;

    [SerializeField] private Image _itemImage;
    [SerializeField] private Text _amountText;

    private Item_ScrObj _currentItem;
    public Item_ScrObj currentItem { get => _currentItem; set => _currentItem = value; }

    private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    private bool _itemDragging = false;
    public bool itemDragging { get => _itemDragging; set => _itemDragging = value; }

    private bool _slotDetected = true;
    public bool slotDetected { get => _slotDetected; set => _slotDetected = value; }

    private bool _tileDetected = false;
    public bool tileDetected { get => _tileDetected; set => _tileDetected = value; }

    [Header("Interaction Data")]
    [SerializeField] private int _bundleDropAmount;
    public int bundleDropAmount { get => _bundleDropAmount; set => _bundleDropAmount = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out RectTransform rectTransform)) { _rectTransform = rectTransform; }
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
    public void Assign(Item_ScrObj item, int amount)
    {
        _currentItem = item;
        _currentAmount = amount;

        _itemImage.sprite = item.sprite;
        _itemImage.color = Color.white;

        _amountText.text = _currentAmount.ToString();
        Color textColor = _amountText.color;
        textColor.a = 1f;
        _amountText.color = textColor;

        _itemDragging = true;

        _inventoryController.controller.tilemapController.actionSystem.Highlight_ItemDrop_Tiles();
    }
    public void Clear()
    {
        _currentItem = null;
        _currentAmount = 0;

        _itemImage.sprite = null;
        _itemImage.color = Color.clear;

        Color textColor = _amountText.color;
        textColor.a = 0f;
        _amountText.color = textColor;

        _rectTransform.anchoredPosition = new Vector2(0, -587.55f);

        _itemDragging = false;

        _inventoryController.controller.tilemapController.actionSystem.UnHighlight_All_tiles();
    }

    public void Increase_Amount(int amount)
    {
        _currentAmount += amount;
        _amountText.text = _currentAmount.ToString();
    }
    public void Decrease_Amount(int amount)
    {
        _currentAmount -= amount;
        _amountText.text = _currentAmount.ToString();

        if (_currentAmount > 0) return;
        Clear();
    }

    // Updates
    public void Drag_Item()
    {
        if (!_itemDragging) return;
        
        Vector2 mousePosition = Input.mousePosition;
        Vector2 canvasPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, mousePosition, null, out canvasPosition);

        _rectTransform.anchoredPosition = canvasPosition;
    }
    public void Return_Drag_Item()
    {
        if (_inventoryController.Is_Inventory_Full()) _inventoryController.Add_Item(_currentItem, _currentAmount);
        else _inventoryController.Empty_Slot().Assign(_currentItem, _currentAmount);
        Clear();
    }
    public void Return_Drag_Item(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!itemDragging) return;
        if (_slotDetected) return;
        if (_tileDetected) return;

        if (_inventoryController.Is_Inventory_Full()) _inventoryController.Add_Item(_currentItem, _currentAmount);
        else _inventoryController.Empty_Slot().Assign(_currentItem, _currentAmount);
        Clear();
    }
}