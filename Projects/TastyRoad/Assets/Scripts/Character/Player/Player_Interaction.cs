using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    private Player_Controller _controller;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller controller)) { _controller = controller; }
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

    private void OnHoldInteract()
    {
        Drop_CurrentFood();
    }


    // Player Interact Actions
    private void Drop_CurrentFood()
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