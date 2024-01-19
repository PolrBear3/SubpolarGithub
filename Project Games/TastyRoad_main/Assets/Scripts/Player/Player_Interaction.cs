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
        _controller.foodIcon.Assign_Food(_controller.mainController.dataController.RawFood(0));
        _controller.foodIcon.Update_State(FoodState_Type.sliced, 1);
    }

    // Player Input
    private void OnInteract()
    {
        Detection_Controller detection = _controller.detectionController;

        if (detection.Closest_Interactable() == null) return;
        if (!detection.Closest_Interactable().TryGetComponent(out IInteractable interactable)) return;

        interactable.Interact();
    }

    // IInteractable
    public void Interact()
    {

    }
}
