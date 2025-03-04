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
    [Range(0, 100)][SerializeField] private int _discountPercentage;


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
        if (_stockData == null) _stockData = new(false);

        // load data
        Restock();
        Toggle_Discount(_stockData.isDiscount);

        // subscriptions
        _interactable.OnIInteract += Set_Dialog;

        _interactable.OnAction1Input += Purchase;
        _interactable.OnAction2Input += Toggle_Discount;
    }

    private void OnDestroy()
    {
        _interactable.OnIInteract -= Set_Dialog;

        _interactable.OnAction1Input -= Purchase;
        _interactable.OnAction2Input -= Toggle_Discount;
    }


    public void Set_Dialog()
    {
        if (_sold) return;

        Station_ScrObj currentStation = _currentStation.stationScrObj;
        Sprite stationIcon = currentStation.dialogIcon;

        // calculation
        int price = _currentStation.amount;

        if (_stockData.isDiscount && price > 0)
        {
            float discountValue = 1f - (_discountPercentage / 100f);
            price = Mathf.RoundToInt(price * discountValue);
        }
        string currentAmountString = "\nyou have " + Main_Controller.instance.GoldenNugget_Amount() + " <sprite=56>";
        string dialogInfo = price + " <sprite=56> to purchase." + currentAmountString;

        gameObject.GetComponent<DialogTrigger>().Update_Dialog(new DialogData(stationIcon, dialogInfo));
    }


    // Public Constructors
    public StationData Set_StationData(StationData data)
    {
        if (data == null) return null;

        _currentStation = new(data);
        return _currentStation;
    }

    public StockData Set_StockData(StockData data)
    {
        if (data == null) return null;

        _stockData = new(data);
        return _stockData;
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

        // discount calculation
        int price = _currentStation.amount;

        if (_stockData.isDiscount && price > 0)
        {
            float discountValue = 1f - (_discountPercentage / 100f);
            price = Mathf.RoundToInt(price * discountValue);
        }

        Main_Controller main = Main_Controller.instance;

        // pay calculation
        if (main.GoldenNugget_Amount() < price)
        {
            // Not Enough nuggets to purchase!
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller stationMenu = main.currentVehicle.menu.stationMenu;
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
        Toggle_Discount(false);
    }

    public void Update_toSold()
    {
        // set data
        _sold = true;
        _currentStation = null;

        // set sprite
        _sr.sprite = _emptyStation;
        _statusSign.sprite = _signSprites[2];

        _sr.sortingLayerName = "Background";
        _sr.sortingOrder = 1;

        //
        _interactable.bubble.Toggle_Height(true);
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
        _stockData.Toggle_Discount(toggleOn);

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


    public void Restock(Station_ScrObj restockStation)
    {
        if (restockStation == null)
        {
            Update_toSold();
            return;
        }

        // set data
        _currentStation = new(restockStation);
        _currentStation.Set_Amount(restockStation.price);

        _sold = false;

        // set sprite
        _sr.sprite = _currentStation.stationScrObj.sprite;

        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder = 0;

        if (_stockData.isDiscount) return;
        _statusSign.sprite = _signSprites[0];

        //
        _interactable.bubble.Toggle_Height(false);
    }
    public void Restock()
    {
        if (_currentStation == null)
        {
            Update_toSold();
            return;
        }

        Restock(_currentStation.stationScrObj);
    }
}
