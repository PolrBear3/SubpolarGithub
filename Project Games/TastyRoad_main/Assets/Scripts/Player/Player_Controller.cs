using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller mainController;

    [HideInInspector] public Detection_Controller detectionController;
    [HideInInspector] public BasicAnimation_Controller animationController;

    public FoodData_Controller foodIcon;

    [HideInInspector] public Player_Movement movement;
    [HideInInspector] public Player_Interaction interaction;

    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentCharacter(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { this.detectionController = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { this.animationController = animationController; }

        if (gameObject.TryGetComponent(out Player_Movement movement)) { this.movement = movement; }
    }
}