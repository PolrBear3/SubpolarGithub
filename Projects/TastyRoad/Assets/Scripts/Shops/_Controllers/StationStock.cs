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

    [Header("")]
    [SerializeField] private Sprite _emptyStation;

    [Header("")]
    [SerializeField] private SpriteRenderer _statusSign;
    [SerializeField] private Sprite _sellSign;
    [SerializeField] private Sprite _soldSign;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Update_toSold();

        _interactable.InteractEvent += Set_Dialog;
        _interactable.Action1Event += Purchase;
    }

    private void OnDestroy()
    {
        _interactable.InteractEvent -= Set_Dialog;
        _interactable.Action1Event -= Purchase;
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
        // set station scrobj
        _currentStation = setStation;

        // set initial price
        _price = _currentStation.price;
    }


    private void Set_Dialog()
    {
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
        if (_sold) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // check if player has enough coins
        if (_interactable.mainController.GoldenNugget_Amount() < _price)
        {
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller stationMenu = _interactable.mainController.currentVehicle.menu.stationMenu;

        // check if station menu slot is available
        if (stationMenu.slotsController.AvailableSlots_Count() <= 0)
        {
            DialogData data = new(_currentStation.dialogIcon, "Not enough space in station storage!");
            dialog.Update_Dialog(data);

            return;
        }

        // add to vehicle
        stationMenu.Add_StationItem(_currentStation, 1);

        // station coin launch animation
        _launcher.Parabola_CoinLaunch(_currentStation.miniSprite, _interactable.detection.player.transform.position - transform.position);

        // 
        Update_toSold();
    }

    private void Update_toSold()
    {
        // set data
        _sold = true;
        _currentStation = null;

        _interactable.Action1Event -= Purchase;

        // set sprite
        _sr.sortingLayerName = "Background";
        _sr.sortingOrder += 1;

        _sr.sprite = _emptyStation;
        _statusSign.sprite = _soldSign;

        //
        _interactable.LockInteract(true);
    }

    public void Restock()
    {
        if (_sold == false) return;

        Set_Data();

        // set data
        _sold = false;
        _interactable.Action1Event += Purchase;

        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        // set sprite
        _sr.sprite = _currentStation.sprite;
        _statusSign.sprite = _sellSign;

        //
        _interactable.LockInteract(false);
    }
    public void Restock(Station_ScrObj restockStation)
    {
        if (_sold == false) return;

        Set_Data(restockStation);

        // set data
        _sold = false;
        _interactable.Action1Event += Purchase;

        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        // set sprite
        _sr.sprite = _currentStation.sprite;
        _statusSign.sprite = _sellSign;

        //
        _interactable.LockInteract(false);
    }
}
