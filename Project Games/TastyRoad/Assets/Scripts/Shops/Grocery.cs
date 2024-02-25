using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Grocery : MonoBehaviour, ISaveLoadable
{
    [SerializeField] private Shop_Controller _controller;

    private int _hoverNum;
    private List<FoodData> _availableFoods = new();

    [Header("Hover Item")]
    [SerializeField] private Image _itemImage;

    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _priceText;



    // UnityEngine
    private void Start()
    {
        if (!ES3.KeyExists("Grocery_availableFoods"))
        {
            Update_AvailableFoods();
        }

        Update_HoverFood(0);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("Grocery_availableFoods", _availableFoods);
    }

    public void Load_Data()
    {
        _availableFoods = ES3.Load("Grocery_availableFoods", _availableFoods);
    }



    // InputSystem
    private void OnSelect()
    {
        Purchase_HoverFood();
    }

    private void OnCursorControl(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        Update_HoverFood((int)input.x);
    }

    private void OnExit()
    {
        _controller.Menu_Toggle(false);
    }



    //
    private void Update_AvailableFoods()
    {
        List<Food_ScrObj> dataFoods = _controller.mainController.dataController.rawFoods;

        // remove all cook necessary raw foods
        for (int i = 0; i < dataFoods.Count; i++)
        {
            if (dataFoods[i].ingredients.Count > 0) continue;
            _availableFoods.Add(new(dataFoods[i], 99));
        }

        // sort by price from lowest to highest
        _availableFoods.Sort((x, y) => x.foodScrObj.price.CompareTo(y.foodScrObj.price));
    }

    private void Update_HoverFood(int cursorDirection)
    {
        _hoverNum += cursorDirection;

        if (_hoverNum < 0) _hoverNum = _availableFoods.Count - 1;
        else if (_hoverNum > _availableFoods.Count - 1) _hoverNum = 0;

        FoodData currentFood = _availableFoods[_hoverNum];

        // update center position
        _itemImage.rectTransform.localPosition = currentFood.foodScrObj.centerPosition * 0.1f;
        _itemImage.sprite = currentFood.foodScrObj.sprite;

        _amountText.text = currentFood.currentAmount.ToString();
        _priceText.text = currentFood.foodScrObj.price.ToString();
    }

    private void Purchase_HoverFood()
    {
        FoodMenu_Controller foodMenu = _controller.mainController.currentVehicle.menu.foodMenu;
        FoodData currentFood = _availableFoods[_hoverNum];

        // current amount check
        if (currentFood.currentAmount <= 0)
        {
            // not enough amount animation
            return;
        }

        // player coin amount check
        if (Main_Controller.currentCoin < currentFood.foodScrObj.price)
        {
            // not enough coin animation
            return;
        }

        // update food menu and current menu
        foodMenu.Add_FoodItem(currentFood.foodScrObj, 1);
        currentFood.currentAmount -= 1;

        Update_HoverFood(0);

        // player coin launch animation
        ItemLauncher playerLauncher = _controller.detection.player.itemLauncher;
        playerLauncher.Parabola_Launch(transform.position.x - playerLauncher.transform.position.x);
    }
}
