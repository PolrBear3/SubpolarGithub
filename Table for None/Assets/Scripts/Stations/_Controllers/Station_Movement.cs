using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Movement : MonoBehaviour
{
    private Station_Controller _stationController;

    [SerializeField] private GameObject _movementArrows;
    public GameObject movementArrows => _movementArrows;
    
    public Action OnLoadPosition;


    // UnityEngine
    private void Awake()
    {
        _stationController = GetComponent<Station_Controller>();
    }

    private void Start()
    {
        _movementArrows.SetActive(true);
    }


    // Main
    public void Update_Position(Vector2 direction)
    {
        Debug.Log(direction);
    }

    private void SnapPosition_Update()
    {
        Main_Controller main = Main_Controller.instance;
        Vehicle_Controller vehicle = main.currentVehicle;

        transform.position = Utility.SnapPosition(transform.position, vehicle.interactArea.bounds);
    }


    /// <returns>
    /// True if current snap position is not claimed and not in a location restricted area
    /// </returns>
    public bool PositionSet_Available()
    {
        Main_Controller main = Main_Controller.instance;

        Vector2 snapPosition = Utility.SnapPosition(transform.position);
        if (main.data.Position_Claimed(snapPosition)) return false;

        Location_Controller location = main.currentLocation;
        if (location.Restricted_Position(snapPosition)) return false;

        Vehicle_Controller vehicle = main.currentVehicle;
        if (vehicle.Is_InteractArea(transform.position) == false) return false;

        return true;
    }

    /// <summary>
    /// Toggle active on claimed positions
    /// </summary>
    private void RestrictBlink_Update()
    {
        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;

        bool isRestricted = PositionSet_Available() == false;
        bool isInteractArea = vehicle.Is_InteractArea(transform.position);

        _stationController.RestrictionBlink_Toggle(isRestricted || isInteractArea == false);
    }


    /// <summary>
    /// Disables movement and set current station after movement is controlled
    /// </summary>
    public void Set_Position()
    {
        if (PositionSet_Available() == false) return;

        Input_Controller.instance.OnAction1 -= Set_Position;

        Load_Position();
        _stationController.data.Update_Position(transform.position);
    }

    /// <summary>
    /// Disables movement and set current station when game is loaded or spawned manually
    /// </summary>
    public void Load_Position()
    {
        Vector2 snapPosition = Utility.SnapPosition(transform.position);

        _stationController.TransparentBlink_Toggle(false);
        _movementArrows.SetActive(false);

        SnapPosition_Claim();
        OnLoadPosition?.Invoke();
        
        enabled = false;
    }


    private void SnapPosition_Claim()
    {
        Vector2 snapPosition = Utility.SnapPosition(transform.position);

        transform.localPosition = snapPosition;
        Main_Controller.instance.data.Claim_Position(snapPosition);
    }
}