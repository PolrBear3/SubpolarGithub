using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleMovement_Controller : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private Vehicle_Controller _controller;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
    [SerializeField][Range(0, 10)] private float _defaultMoveSpeed;

    [SerializeField][Range(0, 10)] private float _maxMoveSpeed;
    public float maxMoveSpeed => _maxMoveSpeed;

    private float _moveSpeed;
    public float moveSpeed => _moveSpeed;

    private bool _onBoard;
    public bool onBoard => _onBoard;

    private Vector2 _currentDirection;

    private Vector2 _defaultPosition;
    private Vector2 _recentPosition;


    // UnityEngine
    private void Start()
    {
        // move speed
        _moveSpeed = _defaultMoveSpeed;

        // set to recent position
        _controller.positionClaimer.UnClaim_CurrentPositions();

        _controller.transform.position = _recentPosition;
        _controller.positionClaimer.Claim_CurrentPositions();

        // set player position
        Player_Controller player = _interactable.mainController.Player();
        player.transform.position = _controller.driverSeatPoint.position;

        // subscriptions
        WorldMap_Controller.NewLocation_Event += Moveto_DefaultPosition;

        _interactable.OnInteractInput += Exit;

        _interactable.OnAction1Input += Ride;
        _interactable.OnAction2Input += Open_WorldMap;
    }

    private void Update()
    {
        ResrictPosition_Update();
        ExitRestricted_IndicationUpdate();
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }

    private void OnDestroy()
    {
        // subscriptions
        WorldMap_Controller.NewLocation_Event -= Moveto_DefaultPosition;

        _interactable.OnInteractInput -= Exit;

        _interactable.OnAction1Input -= Ride;
        _interactable.OnAction2Input -= Open_WorldMap;
    }


    // InputSystem
    private void OnMovement(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        _currentDirection = input;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("VehicleMovement_Controller/_moveSpeed", _moveSpeed);

        ES3.Save("VehicleMovement_Controller/_defaultPosition", _defaultPosition);
        ES3.Save("VehicleMovement_Controller/_recentPosition", _recentPosition);
    }

    public void Load_Data()
    {
        _moveSpeed = ES3.Load("VehicleMovement_Controller/_moveSpeed", _defaultMoveSpeed);

        if (ES3.KeyExists("VehicleMovement_Controller/_recentPosition") == false)
        {
            _recentPosition = _controller.transform.position;
            _defaultPosition = _controller.transform.position;

            return;
        }

        _defaultPosition = ES3.Load("VehicleMovement_Controller/_defaultPosition", _defaultPosition);
        _recentPosition = ES3.Load("VehicleMovement_Controller/_recentPosition", _recentPosition);
    }


    // Movement
    private void Movement_Update()
    {
        if (_onBoard == false) return;

        Vector2 moveDirection = new(_currentDirection.x, _currentDirection.y);
        _controller.transform.Translate(_moveSpeed * Time.deltaTime * moveDirection);
    }

    public void Update_MovementSpeed(float updateValue)
    {
        _moveSpeed = Mathf.Clamp(_moveSpeed + updateValue, 0, _maxMoveSpeed);
    }


    private void ResrictPosition_Update()
    {
        if (_onBoard == false) return;

        Location_Controller location = _controller.mainController.currentLocation;
        Transform vehicle = _controller.transform;

        if (location.Restricted_Position(vehicle.position) == false) return;

        vehicle.position = location.Redirected_Position(vehicle.position);
    }

    private void ExitRestricted_IndicationUpdate()
    {
        if (_onBoard == false) return;

        if (Exit_Available() == false)
        {
            LeanTween.color(_controller.spritesFile, Color.red, 0.01f);
            return;
        }

        LeanTween.color(_controller.spritesFile, Color.white, 0.01f);
    }


    // World Map
    private void Open_WorldMap()
    {
        if (_onBoard) return;

        _interactable.mainController.worldMap.Map_Toggle(true);
    }

    private void Moveto_DefaultPosition()
    {
        _controller.positionClaimer.UnClaim_CurrentPositions();

        _controller.transform.position = _defaultPosition;
        _recentPosition = _defaultPosition;

        _controller.positionClaimer.Claim_CurrentPositions();
    }


    // Actions
    private void Ride()
    {
        _interactable.LockUnInteract(true);
        _interactable.bubble.Toggle(false);

        _controller.Toggle_TransparencyLock(true);
        _controller.positionClaimer.UnClaim_CurrentPositions();

        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = false;
        player.Toggle_Hide(true);

        _onBoard = true;
    }


    private bool Exit_Available()
    {
        Location_Controller location = _controller.mainController.currentLocation;
        Custom_PositionClaimer claimer = _controller.positionClaimer;

        for (int i = 0; i < claimer.All_InteractPositions().Count; i++)
        {
            if (location.Restricted_Position(claimer.All_InteractPositions()[i])) return false;

            if (claimer.Is_ClaimPosition(claimer.All_InteractPositions()[i]) == false) continue;
            Vector2 redirectedPos = location.Redirected_SnapPosition(claimer.All_InteractPositions()[i]);

            if (_controller.mainController.Position_Claimed(redirectedPos)) return false;
        }

        return true;
    }

    private void Exit()
    {
        if (_onBoard == false) return;

        if (Exit_Available() == false) return;

        _interactable.LockUnInteract(false);
        _interactable.UnInteract();

        // set vehicle to snap point
        Location_Controller location = _controller.mainController.currentLocation;
        Transform vehicle = _controller.transform;

        Vector2 targetPos = location.Redirected_SnapPosition(vehicle.position);
        vehicle.transform.position = targetPos;

        _controller.Toggle_TransparencyLock(false);
        _controller.positionClaimer.Claim_CurrentPositions();

        // update player
        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = true;
        player.Toggle_Hide(false);

        player.transform.position = _controller.driverSeatPoint.position;

        _recentPosition = _controller.transform.position;
        _onBoard = false;
    }
}
