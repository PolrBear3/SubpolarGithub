using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Sprite _emptyStation;

    [Header("")]
    [SerializeField] private SpriteRenderer _statusSign;
    [SerializeField] private Sprite[] _signSprites;

    [Header("")]
    [Range(0, 99)][SerializeField] private int _discountPrice;


    private StationData _currentStation;
    public StationData currentStation => _currentStation;

    private StockData _stockData;
    public StockData stockData => _stockData;

    private bool _sold;
    public bool sold => _sold;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (currentStation == null) _currentStation = new();

        // load data
        Restock(_currentStation.stationScrObj);
        Toggle_Discount(_stockData.isDiscount);

        // subscriptions
        _interactable.InteractEvent += Set_Dialog;

        _interactable.OnAction1Event += Purchase;
        _interactable.OnAction2Event += Toggle_Discount;
    }

    private void OnDestroy()
    {
        _interactable.InteractEvent -= Set_Dialog;

        _interactable.OnAction1Event -= Purchase;
        _interactable.OnAction2Event -= Toggle_Discount;
    }


    // Set
    public void Set_StockData(StockData data)
    {
        _stockData = data;
    }


    private void Set_Data()
    {
        if (_currentStation == null)
        {
            // get random station
            Station_ScrObj randStation = _interactable.mainController.dataController.Station_ScrObj();

            // set station scrobj
            _currentStation = new(randStation);
        }
    }
    public void Set_Data(Station_ScrObj setStation)
    {
        if (setStation == null) return;

        // set station scrobj
        _currentStation = new(setStation);
    }


    private void Set_Dialog()
    {
        if (_sold) return;

        Station_ScrObj currentStation = _currentStation.stationScrObj;
        Sprite stationIcon = currentStation.dialogIcon;

        // calculation
        int price = currentStation.price;

        if (_stockData.isDiscount && price > 0)
        {
            price = Mathf.Clamp(price - _discountPrice, 0, currentStation.price);
        }

        string currentAmountString = "\nyou have " + _interactable.mainController.GoldenNugget_Amount() + " <sprite=0>";
        string dialogInfo = price + " <sprite=0> to purchase." + currentAmountString;

        gameObject.GetComponent<DialogTrigger>().Update_Dialog(new DialogData(stationIcon, dialogInfo));
    }


    // Functions
    private void Purchase()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_sold)
        {
            // Not Enough space in vehicle storage!
            dialog.Update_Dialog(2);
            return;
        }

        Station_ScrObj currentStation = _currentStation.stationScrObj;

        // calculation
        int price = currentStation.price;

        if (_stockData.isDiscount && price > 0)
        {
            price = Mathf.Clamp(price - _discountPrice, 0, currentStation.price);
        }

        if (_interactable.mainController.GoldenNugget_Amount() < price)
        {
            // Not Enough nuggets to purchase!
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller stationMenu = _interactable.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = stationMenu.controller.slotsController;

        bool slotFull = slotsController.Empty_SlotData(stationMenu.currentDatas) == null;

        if (slotFull)
        {
            // Not Enough space in vehicle storage!
            dialog.Update_Dialog(1);
            return;
        }

        // add to vehicle
        stationMenu.Add_StationItem(currentStation, 1);

        // station coin launch animation
        _launcher.Parabola_CoinLaunch(currentStation.miniSprite, _interactable.detection.player.transform.position);

        // 
        Update_toSold();
    }


    private void Update_toSold()
    {
        // set data
        _sold = true;
        Toggle_Discount(false);

        _currentStation = new();

        // set sprite
        _sr.sortingLayerName = "Background";
        _sr.sortingOrder += 1;

        _sr.sprite = _emptyStation;
        _statusSign.sprite = _signSprites[2];
    }


    private void Toggle_Discount()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_sold == false)
        {
            dialog.Update_Dialog(3);
            return;
        }

        _stockData.Toggle_Discount(!_stockData.isDiscount);

        if (_stockData.isDiscount)
        {
            _statusSign.sprite = _signSprites[1];

            dialog.Update_Dialog(4);
            return;
        }

        dialog.Update_Dialog(5);

        if (_sold)
        {
            _statusSign.sprite = _signSprites[2];
            return;
        }

        _statusSign.sprite = _signSprites[0];
    }
    public void Toggle_Discount(bool toggleOn)
    {
        _stockData = new(toggleOn);

        if (_stockData.isDiscount)
        {
            _statusSign.sprite = _signSprites[1];
            return;
        }

        if (_sold)
        {
            _statusSign.sprite = _signSprites[2];
            return;
        }

        _statusSign.sprite = _signSprites[0];
    }


    public void Restock()
    {
        // set data
        Set_Data();

        _sold = false;

        // set sprite
        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        _sr.sprite = _currentStation.stationScrObj.sprite;

        if (_stockData.isDiscount) return;
        _statusSign.sprite = _signSprites[0];
    }
    public void Restock(Station_ScrObj restockStation)
    {
        if (restockStation == null)
        {
            Update_toSold();
            return;
        }

        // set data
        Set_Data(restockStation);

        _sold = false;

        // set sprite
        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        _sr.sprite = _currentStation.stationScrObj.sprite;

        if (_stockData.isDiscount) return;
        _statusSign.sprite = _signSprites[0];
    }
}
