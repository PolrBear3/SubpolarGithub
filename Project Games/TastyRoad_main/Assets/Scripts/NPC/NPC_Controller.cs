using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour, IInteractable
{
    [HideInInspector] public Main_Controller mainController;

    [HideInInspector] public Detection_Controller detectionController;
    [HideInInspector] public BasicAnimation_Controller animationController;

    [HideInInspector] public NPC_Movement movement;

    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentCharacter(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { this.detectionController = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { this.animationController = animationController; }

        if (gameObject.TryGetComponent(out NPC_Movement movement)) { this.movement = movement; }
    }

    // IInteractable
    public void Interact()
    {
        
    }
}