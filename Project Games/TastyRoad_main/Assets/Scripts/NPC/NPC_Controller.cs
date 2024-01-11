using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour, IInteractable
{
    [HideInInspector] public Main_Controller _mainController;

    [HideInInspector] public Detection_Controller _detectionController;
    [HideInInspector] public BasicAnimation_Controller _animationController;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();

        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { _detectionController = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { _animationController = animationController; }
    }
    private void Start()
    {
        _mainController.Track_CurrentCharacter(gameObject);
    }

    // IInteractable
    public void Interact()
    {
        
    }
}