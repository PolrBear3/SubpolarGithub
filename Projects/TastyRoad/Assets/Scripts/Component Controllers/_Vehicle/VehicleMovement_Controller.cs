using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleMovement_Controller : ActionBubble_Interactable
{
    private bool _onBoard;


    // UnityEngine
    private new void Start()
    {
        base.Start();

        // subscriptions
        Action1Event += Ride;
    }

    private void OnDestroy()
    {
        // subscriptions
        Action1Event -= Ride;
    }


    // InputSystem
    private void OnInteract()
    {
        Exit();
    }


    //
    private void Ride()
    {
        LockUnInteract(true);
        bubble.Toggle(false);

        Player_Controller player = mainController.Player();

        player.Player_Input().enabled = false;
        player.Toggle_Hide(true);

        _onBoard = true;
    }

    private void Exit()
    {
        if (_onBoard == false) return;

        LockUnInteract(false);
        UnInteract();

        Player_Controller player = mainController.Player();

        player.Player_Input().enabled = true;
        player.Toggle_Hide(false);

        _onBoard = false;
    }
}
