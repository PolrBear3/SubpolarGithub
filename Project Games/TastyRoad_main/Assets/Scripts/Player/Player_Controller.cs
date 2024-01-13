using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour, IInteractable
{
    [HideInInspector] public Main_Controller mainController;

    [HideInInspector] public Detection_Controller detectionController;
    [HideInInspector] public BasicAnimation_Controller animationController;

    public FoodData_Controller currentFoodData;

    [HideInInspector] public Player_Movement movement;

    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentCharacter(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { this.detectionController = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { this.animationController = animationController; }
        if (gameObject.TryGetComponent(out Player_Movement movement)) { this.movement = movement; }
    }

    // IInteractable
    public void Interact()
    {
        
    }

    // Player Input
    private void OnInteract()
    {
        if (detectionController.Closest_Interactable() == null) return;
        if (!detectionController.Closest_Interactable().TryGetComponent(out IInteractable interactable)) return;

        interactable.Interact();
    }
}