using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot_Cursor : MonoBehaviour
{
    private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;


    [Header("")]
    [SerializeField] private ItemSlots_Controller _slotsController;

    [Header("")]
    [SerializeField] private Image _cursorImage;
    public Image cursorImage => _cursorImage;

    [SerializeField] private Sprite _defaultCursor;
    public Sprite defaultCursor => _defaultCursor;

    [SerializeField] private TextMeshProUGUI _amountText;

    [Header("")]
    [SerializeField] private UI_ClockTimer _holdTimer;
    public UI_ClockTimer holdTimer => _holdTimer;


    private ItemSlot_Data _data;

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


    // Sprite
    public void Update_DefaultCursor(Sprite updateSprite)
    {
        _defaultCursor = updateSprite;
    }


    // Data
    public void Assign_Data(ItemSlot_Data data)
    {
        _data = data;
    }

    public ItemSlot_Data Current_Data()
    {
        return _data;
    }


    // Control
    public void Empty_Item()
    {
        _data = new();

        _data.hasItem = false;
        _data.isLocked = false;

        _data.currentFood = null;
        _data.currentStation = null;

        _cursorImage.sprite = _defaultCursor;
        _cursorImage.rectTransform.anchoredPosition = Vector2.zero;

        _data.currentAmount = 0;
        _amountText.text = "";
    }


    public void Assign_Item(Food_ScrObj food)
    {
        if (food != null)
        {
            _data.hasItem = true;
            _data.currentFood = food;

            _cursorImage.sprite = food.sprite;

            _cursorImage.color = Color.white;
            _cursorImage.rectTransform.anchoredPosition = food.uiCenterPosition;

            return;
        }

        Empty_Item();
    }
    public void Assign_Item(Station_ScrObj station)
    {
        if (station != null)
        {
            _data.hasItem = true;
            _data.currentStation = station;

            _cursorImage.sprite = station.miniSprite;

            _cursorImage.color = Color.white;
            _cursorImage.rectTransform.anchoredPosition = new Vector2(0f, 6.5f);

            return;
        }

        Empty_Item();
    }

    public void Assign_Amount(int assignAmount)
    {
        if (_data.hasItem == false) return;

        _data.currentAmount = assignAmount;
        _amountText.text = _data.currentAmount.ToString();

        if (_data.currentAmount > 0) return;
        Empty_Item();
    }


    public void Navigate_toSlot(ItemSlot assignSlot)
    {
        if (assignSlot == null) return;
        _currentSlot = assignSlot;

        _rectTransform.SetParent(_currentSlot.cursorPoint);
        _rectTransform.anchoredPosition = Vector2.zero;
    }
    public void Navigate_toSlot(Vector2 direction)
    {
        Vector2 currentGridNum = new Vector2(_currentSlot.gridNum.x, _currentSlot.gridNum.y);
        Vector2 nextGridNum = new Vector2(currentGridNum.x + direction.x, currentGridNum.y + direction.y);

        ItemSlot nextSlot = _slotsController.ItemSlot(nextGridNum);
        Navigate_toSlot(nextSlot);
    }
}
