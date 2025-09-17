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
        BasicAnimation_Controller basicAnim = _controller.basicAnim;

        NPC_QuestSystem questSystem = _controller.questSystem;
        bool questActive = questSystem != null && questSystem.QuestSystem_Active();
       
        movement.Stop_FreeRoam();
        
        if (movement.isLeaving == true && questActive == false)
        {
            movement.Leave(movement.intervalTime);
            basicAnim.Flip_Sprite(_controller.interactable.detection.player.gameObject);
            
            return;
        }

        movement.Cancel_LeaveState();
        basicAnim.Flip_Sprite(_controller.interactable.detection.player.gameObject);
        
        movement.CurrentLocation_FreeRoam(movement.currentRoamArea, movement.intervalTime);
    }
}