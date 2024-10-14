using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StationStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    [SerializeField] private CoinLauncher _launcher;

    private Station_ScrObj _currentStation;
    public Station_ScrObj currentStation => _currentStation;

    private int _price;
    public int price => _price;

    private bool _sold;
    public bool sold => _sold;

    private bool _isDiscount;
    public bool isDiscount => _isDiscount;

    [Header("")]
    [SerializeField] private Sprite _emptyStation;

    [Header("")]
    [SerializeField] private SpriteRenderer _statusSign;
    [SerializeField] private Sprite[] _signSprites;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Update_toSold();

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
    private void Set_Data()
    {
        if (_currentStation == null)
        {
            // get random station
            Station_ScrObj randStation = _interactable.mainController.dataController.Station_ScrObj();

            // set station scrobj
            _currentStation = randStation;
        }

        // set initial price
        _price = _currentStation.price;
    }
    public void Set_Data(Station_ScrObj setStation)
    {
        if (setStation == null) return;

        // set station scrobj
        _currentStation = setStation;

        // set initial price
        _price = _currentStation.price;
    }


    private void Set_Dialog()
    {
        if (_sold) return;

        Sprite stationIcon = _currentStation.dialogIcon;
        string dialogInfo = _currentStation.price + " coin\nto purchase";

        gameObject.GetComponent<DialogTrigger>().Update_Dialog(new DialogData(stationIcon, dialogInfo));
    }


    public void Update_Price(int price)
    {
        _price = price;
    }


    // Functions
    private void Purchase()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_sold)
        {
            dialog.Update_Dialog(2);
            return;
        }

        // check if player has enough coins
        if (_interactable.mainController.GoldenNugget_Amount() < _price)
        {
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller stationMenu = _interactable.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = stationMenu.controller.slotsController;

        bool slotFull = slotsController.Empty_SlotData(stationMenu.currentDatas) == null;

        // check if station menu slot is available
        if (slotFull)
        {
            dialog.Update_Dialog(1);
            return;
        }

        // add to vehicle
        stationMenu.Add_StationItem(_currentStation, 1);

        // station coin launch animation
        _launcher.Parabola_CoinLaunch(_currentStation.miniSprite, _interactable.detection.player.transform.position);

        // 
        Update_toSold();
    }


    private void Update_toSold()
    {
        // set data
        _sold = true;
        _isDiscount = false;

        _currentStation = null;

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
            dialog.Update_Dialog(4);
            return;
        }

        _isDiscount = !_isDiscount;

        if (_isDiscount)
        {
            _statusSign.sprite = _signSprites[1];
            dialog.Update_Dialog(new DialogData(dialog.datas[3].icon, "Discount tag has been set."));

            return;
        }

        dialog.Update_Dialog(new DialogData(dialog.datas[3].icon, "Discount tag has been removed."));

        if (_sold)
        {
            _statusSign.sprite = _signSprites[2];
            return;
        }

        _statusSign.sprite = _signSprites[0];
    }
    public void Toggle_Discount(bool toggleOn)
    {
        _isDiscount = toggleOn;

        if (_isDiscount)
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
        if (_sold == false) return;

        // set data
        Set_Data();

        _sold = false;

        // set sprite
        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        _sr.sprite = _currentStation.sprite;

        if (_isDiscount) return;
        _statusSign.sprite = _signSprites[0];
    }
    public void Restock(Station_ScrObj restockStation)
    {
        if (_sold == false) return;

        // set data
        Set_Data(restockStation);

        _sold = false;

        // set sprite
        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        _sr.sprite = _currentStation.sprite;

        if (_isDiscount) return;
        _statusSign.sprite = _signSprites[0];
    }
}
