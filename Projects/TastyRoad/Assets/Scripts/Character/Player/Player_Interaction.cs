using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour, IInteractable
{
    private Player_Controller _controller;



    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller controller)) { _controller = controller; }
    }

    private void Start()
    {
        WorldMap_Controller.NewLocation_Event += Player_NewLocationEvents;
    }



    // Player Input
    private void OnInteract()
    {
        if (Main_Controller.gamePaused) return;

        Detection_Controller detection = _controller.detectionController;

        if (detection.Closest_Interactable() == null) return;
        if (!detection.Closest_Interactable().TryGetComponent(out IInteractable interactable)) return;

        // refresh and update current interacting prefab before > interactable.Interact();
        for (int i = 0; i < detection.All_Interactables().Count; i++)
        {
            if (detection.All_Interactables()[i] == interactable) continue;
            detection.All_Interactables()[i].UnInteract();
        }

        interactable.Interact();
    }



    // IInteractable
    public void Interact()
    {

    }

    public void UnInteract()
    {

    }



    // New Location Events
    private void Player_NewLocationEvents()
    {
        // update food rotten state
        _controller.foodIcon.Update_State(FoodState_Type.rotten, 1);
    }
}