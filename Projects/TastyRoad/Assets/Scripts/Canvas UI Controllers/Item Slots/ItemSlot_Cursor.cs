using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot_Cursor : MonoBehaviour
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _vehicleMenu;
    public VehicleMenu_Controller vehicleMenu => _vehicleMenu;

    [Header("")]
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _amountText;

    [Header("")]
    [SerializeField] private UI_ClockTimer _holdTimer;
    public UI_ClockTimer holdTimer => _holdTimer;


    [Header("")]
    [SerializeField] private Sprite _defaultCursor;


    [HideInInspector] public ItemSlot_Data data;

    private ItemSlot _currentSlot;
    public ItemSlot currentSlot => _currentSlot;


    // UnityEngine
    private void Start()
    {
        Empty_Item();
    }


    /// <summary>
    /// Cursor slot location control
    /// </summary>
    public void Assign_CurrentSlot(ItemSlot assignSlot)
    {
        _currentSlot = assignSlot;

        transform.SetParent(_currentSlot.cursorPoint);
        transform.localPosition = Vector2.zero;
    }


    // Data
    public void Assign_Data(ItemSlot_Data data)
    {
        this.data = data;
    }


    // Cursor item Control
    public void Empty_Item()
    {
        data.hasItem = false;
        data.isLocked = false;

        data.currentFood = null;
        data.currentStation = null;

        data.currentAmount = 0;
        _amountText.color = Color.clear;

        _itemIcon.sprite = _defaultCursor;
    }

    public void Assign_Item(Food_ScrObj food)
    {
        if (food != null)
        {
            data.hasItem = true;
            data.currentFood = food;

            _itemIcon.sprite = food.sprite;

            _itemIcon.color = Color.white;
            _itemIcon.transform.localPosition = food.centerPosition * 0.01f;

            return;
        }

        Empty_Item();
    }
    public void Assign_Item(Station_ScrObj station)
    {
        if (station != null)
        {
            data.hasItem = true;
            data.currentStation = station;

            _itemIcon.sprite = station.miniSprite;

            _itemIcon.color = Color.white;
            _itemIcon.transform.localPosition = station.centerPosition * 0.01f;

            return;
        }

        Empty_Item();
    }

    public void Assign_Amount(int assignAmount)
    {
        data.currentAmount = assignAmount;

        if (data.currentAmount <= 0)
        {
            Empty_Item();
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
}
