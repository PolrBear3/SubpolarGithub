using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VechiclePanel_ItemBox : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _selectIcon;

    [HideInInspector] public int boxNum;

    [HideInInspector] public bool hasItem;
    [HideInInspector] public Food_ScrObj currentFood;
    [HideInInspector] public int currentAmount;

    // UnityEngine
    private void Awake()
    {
        _iconImage.color = Color.clear;
        _amountText.color = Color.clear;
        _selectIcon.color = Color.clear;
    }

    //
    public void BoxSelect_Toggle(bool isSelected)
    {
        if (isSelected == true)
        {
            _selectIcon.color = Color.white;

            return;
        }

        _selectIcon.color = Color.clear;
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

        if (hasItem == false || currentAmount <= 0)
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

        if (hasItem == false || currentAmount <= 0)
        {
            currentFood = null;
            _amountText.color = Color.clear;

            return;
        }

        _amountText.text = currentAmount.ToString();
        _amountText.color = Color.black;
    }
}