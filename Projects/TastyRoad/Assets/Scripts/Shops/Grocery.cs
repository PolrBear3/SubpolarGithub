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
    private List<FoodData> _purchasableFoods = new();

    [Header("Hover Item")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _priceText;



    // UnityEngine
    private void Start()
    {
        if (ES3.KeyExists("Grocery_purchasableFoods") == false)
        {
            Update_AvailableFoods();
        }

        Update_HoverFood(0);
    }



    // InputSystem
    private void OnSelect()
    {
        Purchase_HoverFood();
    }

    private void OnCursorControl(InputValue value)
    {
        Update_HoverFood(value.Get<Vector2>().x);
    }

    private void OnExit()
    {
        _controller.Menu_Toggle(false);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (_purchasableFoods.Count <= 0) return;

        ES3.Save("Grocery_purchasableFoods", _purchasableFoods);
    }

    public void Load_Data()
    {
        _purchasableFoods = ES3.Load("Grocery_purchasableFoods", _purchasableFoods);
    }



    // Menu Class
    private void Update_AvailableFoods()
    {
        List<Food_ScrObj> dataFoods = _controller.mainController.dataController.rawFoods;

        // remove all cook necessary raw foods
        for (int i = 0; i < dataFoods.Count; i++)
        {
            if (dataFoods[i].ingredients.Count > 0) continue;
            _purchasableFoods.Add(new(dataFoods[i], 99));
        }

        // sort by price from lowest to highest
        _purchasableFoods.Sort((x, y) => x.foodScrObj.price.CompareTo(y.foodScrObj.price));
    }

    private void Update_HoverFood(float cursorDirection)
    {
        _hoverNum += (int)cursorDirection;

        if (_hoverNum < 0) _hoverNum = _purchasableFoods.Count - 1;
        else if (_hoverNum > _purchasableFoods.Count - 1) _hoverNum = 0;

        FoodData currentFood = _purchasableFoods[_hoverNum];

        // update center position
        _itemImage.rectTransform.localPosition = currentFood.foodScrObj.centerPosition * 0.1f;
        _itemImage.sprite = currentFood.foodScrObj.sprite;

        // update text
        _priceText.text = currentFood.foodScrObj.price.ToString();
    }

    private void Purchase_HoverFood()
    {
        FoodMenu_Controller foodMenu = _controller.mainController.currentVehicle.menu.foodMenu;
        FoodData currentFood = _purchasableFoods[_hoverNum];

        // player coin amount check
        if (Main_Controller.currentGoldCoin < currentFood.foodScrObj.price)
        {
            // not enough coin animation
            return;
        }

        // update food menu and current menu
        foodMenu.Add_FoodItem(currentFood.foodScrObj, 1);

        Main_Controller.currentGoldCoin -= currentFood.foodScrObj.price;

        Update_HoverFood(0);

        // player coin parabola launch animation
        Player_Controller player = _controller.detection.player;
        Coin_ScrObj goldCoin = _controller.mainController.dataController.coinTypes[0];
        player.coinLauncher.Parabola_CoinLaunch(goldCoin, transform.position - player.transform.position);
    }
}
