using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;


    // UnityEngine
    private void Start()
    {
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnIInteract += Interact_FacePlayer;
        interactable.OnHoldIInteract += Interact_FacePlayer;
    }

    private void OnDestroy()
    {
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnIInteract -= Interact_FacePlayer;
        interactable.OnHoldIInteract -= Interact_FacePlayer;
    }


    //
    private void Interact_FacePlayer()
    {
        NPC_Movement movement = _controller.movement;

        movement.Stop_FreeRoam();
        _controller.basicAnim.Flip_Sprite(_controller.interactable.detection.player.gameObject);

        if (movement.isLeaving == true)
        {
            movement.Leave(movement.Random_IntervalTime());
            return;
        }

        movement.Free_Roam(movement.currentRoamArea, movement.Random_IntervalTime());
    }
}