using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller _mainController;

    [HideInInspector] public Detection_Controller _detectionController;
    [HideInInspector] public BasicAnimation_Controller _animationController;

    [HideInInspector] public Player_Movement _movement;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();

        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { _detectionController = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { _animationController = animationController; }
        if (gameObject.TryGetComponent(out Player_Movement movement)) { _movement = movement; }
    }

    private void Start()
    {
        _mainController.Track_CurrentCharacter(gameObject);
    }

    // Player Input
    private void OnInteract()
    {
        if (_detectionController.Closest_Interactable() == null) return;
        if (!_detectionController.Closest_Interactable().TryGetComponent(out IInteractable interactable)) return;
        interactable.Interact();
    }
}
