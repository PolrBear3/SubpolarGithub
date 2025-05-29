using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private DialogTrigger _dialog;

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

    
    [Space(60)]
    [SerializeField] private VideoGuide_Trigger _guideTrigger;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (_stockData == null) _stockData = new(false);

        // load data
        Restock(_currentStation);
        Toggle_Discount(_stockData.isDiscount);

        // subscriptions
        _interactable.OnInteract += Toggle_Price;
        _interactable.OnInteract += _guideTrigger.Trigger_CurrentGuide;

        _interactable.OnAction1 += Purchase;
        _interactable.OnAction2 += Toggle_Discount;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteract -= Toggle_Price;
        _interactable.OnInteract -= _guideTrigger.Trigger_CurrentGuide;

        _interactable.OnAction1 -= Purchase;
        _interactable.OnAction2 -= Toggle_Discount;
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
    private int Price()
    {
        if (_sold) return 0;

        int price = _currentStation.stationScrObj.price;

        if (_currentStation.amount <= 0) return 0;
        
        if (_stockData.isDiscount && price > 0)
        {
            float discountValue = 1f - (_discountPercentage / 100f);
            price = Mathf.RoundToInt(price * discountValue);
        }

        return price;
    }

    private void Toggle_Price()
    {
        if (_sold) return;

        GoldSystem_TriggerData triggerData = new(_currentStation.stationScrObj.dialogIcon, -Price());
        GoldSystem.instance.Indicate_TriggerData(triggerData);
    }


    private void Purchase()
    {
        if (_sold)
        {
            _dialog.Update_Dialog(1);
            return;
        }

        StationMenu_Controller stationMenu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = stationMenu.controller.slotsController;
        
        bool slotFull = slotsController.Empty_SlotData(stationMenu.currentDatas) == null;

        if (slotFull)
        {
            // Not Enough space in vehicle storage!
            _dialog.Update_Dialog(0);
            return;
        }
        
        Station_ScrObj currentStation = _currentStation.stationScrObj;

        // pay calculation
        if (GoldSystem.instance.Update_CurrentAmount(-Price()) == false) return;

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

        _interactable.bubble.Toggle_Height(true);
    }


    private void Toggle_Discount()
    {
        if (_sold == false)
        {
            _dialog.Update_Dialog(2);
            return;
        }
        _stockData.Toggle_Discount(!_stockData.isDiscount);

        if (_stockData.isDiscount)
        {
            _statusSign.sprite = _signSprites[1];

            _dialog.Update_Dialog(3);
            return;
        }
        _dialog.Update_Dialog(4);

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

        _statusSign.sprite = _sold ? _signSprites[2] : _signSprites[0];
    }


    public void Restock(StationData restockData)
    {
        if (restockData == null || restockData.stationScrObj == null)
        {
            Update_toSold();
            return;
        }

        // set data
        _sold = false;
        _currentStation = new(restockData);

        // set sprite
        _sr.sprite = _currentStation.stationScrObj.sprite;

        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder = 0;

        _interactable.bubble.Toggle_Height(false);

        // tag sprite
        if (_stockData.isDiscount) return;
        _statusSign.sprite = _signSprites[0];
    }
}
