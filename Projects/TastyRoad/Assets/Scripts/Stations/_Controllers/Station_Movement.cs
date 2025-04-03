using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Movement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    private Station_Controller _stationController;


    [Header("")]
    [SerializeField] private GameObject _movementArrows;
    public GameObject movementArrows => _movementArrows;


    public Action OnLoadPosition;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Station_Controller stationController)) { _stationController = stationController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }
    }

    private void Start()
    {
        _movementArrows.SetActive(true);
    }

    private void Update()
    {
        RestrictBlink_Update();
        SnapPosition_Update();
    }

    private void FixedUpdate()
    {
        Rigidbody_Update();
    }


    //
    private void Rigidbody_Update()
    {
        Vector2 inputDirection = Input_Controller.instance.movementDirection;
        float speed = 3f;

        _rigidBody.velocity = new Vector2(inputDirection.x * speed, inputDirection.y * speed);
    }

    private void SnapPosition_Update()
    {
        if (_rigidBody.velocity != Vector2.zero) return;

        Main_Controller main = Main_Controller.instance;
        Vehicle_Controller vehicle = main.currentVehicle;

        transform.position = main.SnapPosition(transform.position, vehicle.interactArea.bounds);
    }


    /// <returns>
    /// True if current snap position is not claimed and not in a location restricted area
    /// </returns>
    public bool PositionSet_Available()
    {
        Main_Controller main = Main_Controller.instance;

        Vector2 snapPosition = main.SnapPosition(transform.position);
        if (main.Position_Claimed(snapPosition)) return false;

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
        Vector2 snapPosition = Main_Controller.instance.SnapPosition(transform.position);

        _stationController.TransparentBlink_Toggle(false);
        _movementArrows.SetActive(false);

        MathRound_Snap_Position();

        OnLoadPosition?.Invoke();

        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        enabled = false;
    }


    private void MathRound_Snap_Position()
    {
        Vector2 snapPosition = Main_Controller.instance.SnapPosition(transform.position);

        transform.localPosition = snapPosition;
        Main_Controller.instance.Claim_Position(snapPosition);
    }
}