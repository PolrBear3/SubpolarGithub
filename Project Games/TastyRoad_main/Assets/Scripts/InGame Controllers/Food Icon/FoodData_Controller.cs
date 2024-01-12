using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodData_Controller : MonoBehaviour
{
    [HideInInspector] public FoodState_Controller stateController;

    private SpriteRenderer _sr;
    [SerializeField] private TMP_Text _amountText;

    private Food_ScrObj _currentFood;
    [HideInInspector] public int _currentAmount;

    private bool _iconTransparent;
    private bool _amountTransparent;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out FoodState_Controller stateController)) { this.stateController = stateController; }
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }

    // Transparency Toggle
    public void FoodIcon_Transparency(bool isTransparent)
    {
        _iconTransparent = isTransparent;

        if (_iconTransparent == false)
        {
            Assign_Food(_currentFood);
        }
        else
        {
            _sr.color = Color.clear;
        }
    }

    public void AmountText_Transparency(bool isTransparent)
    {
        _amountTransparent = isTransparent;

        if (_amountTransparent == false)
        {
            Update_Amount(_currentAmount);
        }
        else
        {
            _amountText.color = Color.clear;
        }
    }

    // Food Control
    public void Assign_Food(Food_ScrObj foodScrObj)
    {
        if (foodScrObj != null && _iconTransparent == false)
        {
            _currentFood = foodScrObj;

            _sr.sprite = _currentFood.sprite;
            _sr.color = Color.white;

            Update_Amount(1);
        }
        else
        {
            Clear_Food();
        }
    }

    public void Clear_Food()
    {
        _currentFood = null;
        _currentAmount = 0;

        _sr.color = Color.clear;
        _sr.sprite = null;

        _amountText.color = Color.clear;
    }

    // Amount Control
    public void Update_Amount(int updateAmount)
    {
        _currentAmount += updateAmount;

        if (_currentAmount <= 0)
        {
            Clear_Food();
            return;
        }

        if (_currentFood != null && _currentAmount > 1 && _amountTransparent == false)
        {
            _amountText.color = Color.black;
            _amountText.text = _currentAmount.ToString();
        }
        else if (_amountTransparent == false)
        {
            _amountText.color = Color.clear;
        }
    }
}
