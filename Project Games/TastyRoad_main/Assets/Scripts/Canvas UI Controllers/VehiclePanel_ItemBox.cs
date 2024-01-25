using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VechiclePanel_ItemBox : MonoBehaviour
{
    private Image _borderImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    [Header("Border Sprites")]
    [SerializeField] private Sprite _unselectedBorder;
    [SerializeField] private Sprite _selectedBorder;

    [HideInInspector] public int boxNum;

    [HideInInspector] public bool hasItem;
    [HideInInspector] public Food_ScrObj currentFood;
    [HideInInspector] public int currentAmount;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Image borderImage)) { _borderImage = borderImage; }

        Assign_Item(null);
    }

    //
    public void BoxSelect_Toggle(bool isSelected)
    {
        if (isSelected == true)
        {
            _borderImage.sprite = _selectedBorder;

            return;
        }

        _borderImage.sprite = _unselectedBorder;
    }

    // sprite update included
    public void Assign_Item(Food_ScrObj food)
    {
        if (food != null)
        {
            hasItem = true;
            currentFood = food;

            _iconImage.sprite = food.sprite;
            _iconImage.color = Color.white;

            _iconImage.transform.localPosition = food.centerPosition;

            return;
        }

        hasItem = false;
        currentFood = null;

        _iconImage.sprite = null;
        _iconImage.color = Color.clear;

        _iconImage.transform.localPosition = Vector2.zero;

        Assign_Amount(0);
    }

    // text update included
    public void Assign_Amount(int assignAmount)
    {
        currentAmount = assignAmount;

        if (currentAmount <= 0)
        {
            currentFood = null;
            _amountText.color = Color.clear;

            return;
        }

        _amountText.text = currentAmount.ToString();
        _amountText.color = Color.black;
    }
    public void Update_Amount(int updateAmount)
    {
        currentAmount += updateAmount;

        if (currentAmount <= 0)
        {
            currentFood = null;
            _amountText.color = Color.clear;

            return;
        }

        _amountText.text = currentAmount.ToString();
        _amountText.color = Color.black;
    }
}