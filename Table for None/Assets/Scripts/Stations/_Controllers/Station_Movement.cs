using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Movement : MonoBehaviour
{
    [SerializeField] private Station_Controller _stationController;

    [Space(20)]
    [SerializeField] private GameObject _movementArrows;
    public GameObject movementArrows => _movementArrows;
    
    public Action OnLoadPosition;


    // UnityEngine
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
        if (placedStations.Count <= 1) return false;
        
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

        Vector2 dataPos = _stationController.data.position;
        Vector2 snapPosition = Utility.SnapPosition(dataPos);

        if (NonStation_PositionClaimed(snapPosition)) return false;
        
        Station_ScrObj currentStation = _stationController.stationScrObj;

        if (currentStation.overlapPlaceable && OverlapStation_Placed(dataPos) == false) return true;
        if (main.data.Position_Claimed(snapPosition)) return false;

        return true;
    }

    public Vector2 Offset_Position()
    {
        StationData data = _stationController.data;
        Station_ScrObj currentStation = data.stationScrObj;
        
        if (currentStation.overlapPlaceable == false) return (Vector2)data.position;

        Main_Controller main = Main_Controller.instance;
        
        List<Station_Controller> placedStations = main.CurrentStations(Utility.SnapPosition(data.position));
        placedStations.Remove(_stationController);
        
        for (int i = 0; i < placedStations.Count; i++)
        {
            if ((Vector2)placedStations[i].transform.position != data.position) return (Vector2)data.position;
        }
 
        if (placedStations.Count == 0) return (Vector2)data.position;
        return (Vector2)data.position + currentStation.offsetPosition;
    }

    
    public void Update_Position(Vector2 direction)
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.currentLocation;
        
        Vehicle_Controller vehicle = main.currentVehicle;
        Bounds interactArea = vehicle.interactArea.bounds;
        
        Vector2 dataPos = _stationController.data.position;
        Vector2 updatePos = Utility.SnapPosition(new Vector2(dataPos.x + direction.x, dataPos.y + direction.y));

        if (location.Is_OuterSpawnPoint(updatePos)) return;
        if (vehicle.Is_InteractArea(updatePos) == false) return;

        _stationController.data.Update_Position(updatePos);
        transform.position = Offset_Position();
    }
    
    /// <summary>
    /// Disables movement and set current station when game is loaded or spawned manually
    /// </summary>
    public void Load_Position()
    {
        _stationController.TransparentBlink_Toggle(false);
        _movementArrows.SetActive(false);

        Main_Controller.instance.data.Claim_Position(_stationController.data.position);
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
        _stationController.data.Update_Position(_stationController.data.position);
    }
}