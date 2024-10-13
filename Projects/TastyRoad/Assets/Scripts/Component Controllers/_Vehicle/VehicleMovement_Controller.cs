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
        _interactable.InteractEvent += Exit;

        _interactable.Action1Event += Ride;
        _interactable.Action2Event += Open_WorldMap;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.InteractEvent -= Exit;

        _interactable.Action1Event -= Ride;
        _interactable.Action2Event -= Open_WorldMap;
    }


    // InputSystem
    private void OnInteract()
    {
        Exit();
    }


    //
    private void Open_WorldMap()
    {
        _interactable.mainController.worldMap.Map_Toggle(true);
    }


    // Movement System
    private void Ride()
    {
        _interactable.LockUnInteract(true);
        _interactable.bubble.Toggle(false);

        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = false;
        player.Toggle_Hide(true);

        _onBoard = true;
    }

    private void Exit()
    {
        if (_onBoard == false) return;

        _interactable.LockUnInteract(false);

        Player_Controller player = _interactable.mainController.Player();

        player.Player_Input().enabled = true;
        player.Toggle_Hide(false);

        _onBoard = false;
    }
}
