using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    private Player_Controller _controller;

    private float _pressStartTime;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller controller)) { _controller = controller; }
    }

    void Start()
    {
        _controller.Player_Input().actions["Interact"].started += ctx => OnPressStart();
        _controller.Player_Input().actions["Interact"].canceled += ctx => OnPressEnd();
    }


    // Player Input
    private void OnPressStart()
    {
        _pressStartTime = Time.time;  // Record the time when the button is pressed
    }

    private void OnPressEnd()
    {
        float pressDuration = Time.time - _pressStartTime;
        float holdTime = 1;

        if (pressDuration >= holdTime)
        {
            HoldInteract();
            return;
        }

        Interact();
    }


    private void Interact()
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

    private void HoldInteract()
    {
        Drop_CurrentFood();
    }


    // Player Interact Actions
    public void Drop_CurrentFood()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false) return;

        CoinLauncher launcher = _controller.coinLauncher;
        launcher.Parabola_CoinLaunch(foodIcon.currentData.foodScrObj.sprite, transform.position);

        foodIcon.Set_CurrentData(null);
        foodIcon.Show_Icon();
        foodIcon.Show_Condition();
    }
}