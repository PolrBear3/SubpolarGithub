using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;

    private Station_ScrObj _station;
    private bool _sold;

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
        _interactable.Action1 += Purchase;

        Set_Station();
    }

    private void OnDestroy()
    {
        _interactable.Action1 -= Purchase;
    }


    // Set
    private void Set_Station()
    {
        // get random station
        Station_ScrObj randStation = _interactable.mainController.dataController.Station_ScrObj(true);

        // set data and sprite
        _station = randStation;
        _sr.sprite = _station.sprite;
    }


    // Functions
    private void Purchase()
    {
        if (_sold) return;

        // check if player has enough coins

        // add to vehicle
        _interactable.mainController.currentVehicle.menu.stationMenu.Add_StationItem(_station, 1);

        // 
        Update_toSold();
    }

    private void Update_toSold()
    {
        // set data
        _sold = true;
        _interactable.Action1 -= Purchase;

        // set sprite
        _sr.sprite = _soldSign;
    }
}