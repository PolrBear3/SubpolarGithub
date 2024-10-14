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


    // UnityEngine
    private void Start()
    {
        // subscriptions
        _interactable.OnInteractEvent += Exit;

        _interactable.OnAction1Event += Ride;
        _interactable.OnAction2Event += Open_WorldMap;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteractEvent -= Exit;

        _interactable.OnAction1Event -= Ride;
        _interactable.OnAction2Event -= Open_WorldMap;
    }


    //
    private void Open_WorldMap()
    {
        if (_onBoard) return;

        _interactable.mainController.worldMap.Map_Toggle(true);
    }


    // Movement System
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

        _controller.positionClaimer.Claim_CurrentPositions();

        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = true;
        player.Toggle_Hide(false);

        player.transform.position = _controller.driverSeatPoint.position;

        _onBoard = false;
    }
}
