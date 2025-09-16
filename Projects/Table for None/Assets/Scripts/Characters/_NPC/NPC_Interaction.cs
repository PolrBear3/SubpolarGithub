using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour
{
    [SerializeField] private NPC_Controller _controller;


    // UnityEngine
    private void Start()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnInteract += Interact_FacePlayer;
        interactable.OnHoldInteract += Interact_FacePlayer;
    }

    private void OnDestroy()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnInteract -= Interact_FacePlayer;
        interactable.OnHoldInteract -= Interact_FacePlayer;
    }


    // Main
    private void Interact_FacePlayer()
    {
        NPC_Movement movement = _controller.movement;

        movement.Stop_FreeRoam();
        _controller.basicAnim.Flip_Sprite(_controller.interactable.detection.player.gameObject);

        if (movement.isLeaving == true && _controller.questSystem.QuestSystem_Active() == false)
        {
            movement.Leave(movement.intervalTime);
            return;
        }

        movement.Cancel_LeaveState();
        movement.CurrentLocation_FreeRoam(movement.currentRoamArea, movement.intervalTime);
    }
}