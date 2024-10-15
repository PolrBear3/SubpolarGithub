using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleMovement_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Vehicle_Controller _controller;
    [SerializeField] private ActionBubble_Interactable _interactable;


    private bool _onBoard;
    private Vector2 _currentDirection;


    // UnityEngine
    private void Start()
    {
        // subscriptions
        _interactable.OnInteractEvent += Exit;

        _interactable.OnAction1Event += Ride;
        _interactable.OnAction2Event += Open_WorldMap;
    }

    private void Update()
    {
        ResrictPosition_Update();
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteractEvent -= Exit;

        _interactable.OnAction1Event -= Ride;
        _interactable.OnAction2Event -= Open_WorldMap;
    }


    // InputSystem
    private void OnMovement(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        _currentDirection = input;
    }


    // Movement
    private void Movement_Update()
    {
        if (_onBoard == false) return;

        float moveSpeed = 3f;

        Vector2 moveDirection = new(_currentDirection.x, _currentDirection.y);
        _controller.transform.Translate(moveSpeed * Time.deltaTime * moveDirection);
    }

    private void ResrictPosition_Update()
    {
        Location_Controller location = _controller.mainController.currentLocation;
        Transform vehicle = _controller.transform;

        if (location.Restricted_Position(vehicle.position) == false) return;

        vehicle.position = location.Redirected_Position(vehicle.position);
    }


    // World Map
    private void Open_WorldMap()
    {
        if (_onBoard) return;

        _interactable.mainController.worldMap.Map_Toggle(true);
    }


    // Actions
    private void Ride()
    {
        _interactable.LockUnInteract(true);
        _interactable.bubble.Toggle(false);

        _controller.positionClaimer.UnClaim_CurrentPositions();

        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = false;
        player.Toggle_Hide(true);

        _onBoard = true;
    }

    private void Exit()
    {
        if (_onBoard == false) return;

        _interactable.LockUnInteract(false);
        _interactable.UnInteract();

        _controller.positionClaimer.Claim_CurrentPositions();

        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = true;
        player.Toggle_Hide(false);

        player.transform.position = _controller.driverSeatPoint.position;

        _onBoard = false;
    }
}
