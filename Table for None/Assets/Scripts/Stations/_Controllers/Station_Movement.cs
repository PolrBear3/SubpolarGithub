using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool NonStation_PositionClaimed(Vector2 searchPosition)
    {
        Main_Controller main = Main_Controller.instance;

        Vector2 snapPosition = Utility.SnapPosition(searchPosition);
        List<Station_Controller> placedStations = main.CurrentStations(snapPosition);

        placedStations.Remove(_stationController);
        return placedStations.Count == 0 && main.data.Position_Claimed(snapPosition);
    }
    
    private bool OverlapStation_Placed(Vector2 searchPosition)
    {
        Main_Controller main = Main_Controller.instance;
        List<Station_Controller> placedStations = main.CurrentStations(Utility.SnapPosition(searchPosition));
        
        for (int i = 0; i < placedStations.Count; i++)
        {
            if (placedStations[i] == _stationController) continue;
            if (placedStations[i].stationScrObj.overlapPlaceable == false) continue;

            return true;
        }
        return false;
    }

    public bool PositionSet_Available()
    {
        Main_Controller main = Main_Controller.instance;
        Vector2 snapPosition = Utility.SnapPosition(transform.position);

        if (NonStation_PositionClaimed(snapPosition)) return false;
        
        Station_ScrObj currentStation = _stationController.stationScrObj;
        
        if (currentStation.overlapPlaceable && OverlapStation_Placed(transform.position) == false) return true;
        if (main.data.Position_Claimed(snapPosition)) return false;

        return true;
    }
    
    
    public void Update_Position(Vector2 direction)
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.currentLocation;
        
        Vehicle_Controller vehicle = main.currentVehicle;
        Bounds interactArea = vehicle.interactArea.bounds;
        
        Vector2 updatePos = Utility.SnapPosition(new Vector2(transform.position.x + direction.x, transform.position.y + direction.y));

        if (location.Restricted_Position(updatePos)) return;
        if (vehicle.Is_InteractArea(updatePos) == false) return;

        transform.position = updatePos;
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
    
    
    private void SnapPosition_Claim()
    {
        Vector2 snapPosition = Utility.SnapPosition(transform.position);

        transform.position = snapPosition;
        Main_Controller.instance.data.Claim_Position(snapPosition);
    }
}