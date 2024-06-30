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

    private Station_ScrObj _station;

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
        Set_Station();

        _interactable.InteractEvent += Set_Dialog;
        _interactable.Action1Event += Purchase;
    }

    private void OnDestroy()
    {
        _interactable.InteractEvent -= Set_Dialog;
        _interactable.Action1Event -= Purchase;
    }


    // Set
    private void Set_Station()
    {
        // get random station
        Station_ScrObj randStation = _interactable.mainController.dataController.Station_ScrObj();

        // set data and sprite
        _station = randStation;

        _sr.sprite = _station.sprite;
        _statusSign.sprite = _sellSign;
    }

    private void Set_Dialog()
    {
        Sprite stationIcon = _station.dialogIcon;
        string dialogInfo = _station.price + " coint\nto purchase";

        gameObject.GetComponent<DialogTrigger>().Update_Dialog(new DialogData(stationIcon, dialogInfo));
    }


    // Functions
    private void Purchase()
    {
        if (_sold) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // check if player has enough coins

        StationMenu_Controller stationMenu = _interactable.mainController.currentVehicle.menu.stationMenu;

        // check if station menu slot is available
        if (stationMenu.AvailableSlots_Count() <= 0)
        {
            DialogData data = new(_station.dialogIcon, "Not enough space in station storage!");
            dialog.Update_Dialog(data);

            return;
        }

        // add to vehicle
        stationMenu.Add_StationItem(_station, 1);

        // station coin launch animation
        _launcher.Parabola_CoinLaunch(_station.miniSprite, _interactable.detection.player.transform.position - transform.position);

        // 
        Update_toSold();
    }

    private void Update_toSold()
    {
        // set data
        _sold = true;
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

        Set_Station();

        // set data
        _sold = false;
        _interactable.Action1Event += Purchase;

        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder -= 1;

        //
        _interactable.LockInteract(false);
    }
}
