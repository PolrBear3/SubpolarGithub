using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class StationShop : MonoBehaviour, ISaveLoadable
{
    [SerializeField] private Shop_Controller _controller;

    private int _hoverNum;
    private List<Station_ScrObj> _purchasableStations = new();

    [Header("Current Coin")]
    [SerializeField] private TextMeshProUGUI _currentCoinText;

    [Header("Hover Item")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _priceText;



    // UnityEngine
    private void Start()
    {
        if (ES3.KeyExists("StationShop_purchasableStations") == false)
        {
            Update_AvailableStations();
        }

        Update_HoverStation(0);
    }

    private void OnEnable()
    {
        _currentCoinText.text = Main_Controller.currentStationCoin.ToString();
    }



    // InputSystem
    private void OnSelect()
    {
        Purchase_HoverStation();
    }

    private void OnCursorControl(InputValue value)
    {
        Update_HoverStation(value.Get<Vector2>().x);
    }

    private void OnExit()
    {
        _controller.Menu_Toggle(false);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (_purchasableStations.Count <= 0) return;

        ES3.Save("StationShop_purchasableStations", _purchasableStations);
    }

    public void Load_Data()
    {
        _purchasableStations = ES3.Load("StationShop_purchasableStations", _purchasableStations);
    }



    // Menu Class
    private void Update_AvailableStations()
    {
        List<Station_ScrObj> allStations = _controller.mainController.dataController.stations;

        for (int i = 0; i < allStations.Count; i++)
        {
            if (allStations[i].unRetrievable) continue;
            _purchasableStations.Add(allStations[i]);
        }

        // sort by price from lowest to highest
        _purchasableStations.Sort((x, y) => x.price.CompareTo(y.price));
    }

    private void Update_HoverStation(float cursorDirection)
    {
        _hoverNum += (int)cursorDirection;

        if (_hoverNum < 0) _hoverNum = _purchasableStations.Count - 1;
        else if (_hoverNum > _purchasableStations.Count - 1) _hoverNum = 0;

        Station_ScrObj hoveringStation = _purchasableStations[_hoverNum];

        // update center position
        _itemImage.rectTransform.localPosition = hoveringStation.centerPosition * 0.1f;
        _itemImage.sprite = hoveringStation.miniSprite;

        _priceText.text = hoveringStation.price.ToString();
    }

    private void Purchase_HoverStation()
    {
        StationMenu_Controller stationMenu = _controller.mainController.currentVehicle.menu.stationMenu;
        Station_ScrObj hoveringStation = _purchasableStations[_hoverNum];

        // player coin amount check
        if (Main_Controller.currentStationCoin < hoveringStation.price)
        {
            // not enough coin animation
            return;
        }

        stationMenu.Add_StationItem(hoveringStation, 1);

        Main_Controller.currentStationCoin -= hoveringStation.price;
        _currentCoinText.text = Main_Controller.currentStationCoin.ToString();

        if (hoveringStation.price <= 0) return;

        // player gold coin parabola launch animation
        Player_Controller player = _controller.detection.player;
        Coin_ScrObj stationCoin = _controller.mainController.dataController.coinTypes[1];
        player.coinLauncher.Parabola_CoinLaunch(stationCoin, transform.position - player.transform.position);
    }
}
