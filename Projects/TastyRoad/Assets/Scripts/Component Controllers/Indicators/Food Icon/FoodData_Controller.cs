using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_Controller : MonoBehaviour
{
    private FoodData _currentData;
    public FoodData currentData => _currentData;

    [Header("")]
    [SerializeField] private SpriteRenderer _foodIcon;
    [SerializeField] private SpriteRenderer _amountBar;



    // UnityEngine
    private void Start()
    {
        Hide_Icon();
    }



    // Current Data
    public void Set_CurrentData(FoodData data)
    {
        _currentData = data;
    }

    public void Swap_Data(FoodData_Controller otherController)
    {
        FoodData otherData = otherController.currentData;

        // other controller data
        otherController.Set_CurrentData(_currentData);

        // this controller data
        Set_CurrentData(otherData);
    }



    // Icon Control
    public void Show_Icon()
    {
        _foodIcon.sprite = _currentData.foodScrObj.sprite;
        _foodIcon.transform.localPosition = _currentData.foodScrObj.centerPosition / 100f;
        _foodIcon.color = Color.white;
    }

    public void Hide_Icon()
    {
        _foodIcon.color = Color.clear;
        _foodIcon.sprite = null;
    }
}