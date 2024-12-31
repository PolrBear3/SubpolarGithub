using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{
    private Player_Controller _controller;

    [Header("")]
    [SerializeField][Range(0, 100)] private float _holdTime;

    private float _defaultTimerXPos;

    private float _pressStartTime;
    private Coroutine _pressDelayCoroutine;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller controller)) { _controller = controller; }
    }

    private void Start()
    {
        _controller.Player_Input().actions["Interact"].started += ctx => OnPressStart();
        _controller.Player_Input().actions["Interact"].canceled += ctx => OnPressEnd();

        _defaultTimerXPos = _controller.timer.transform.localPosition.x;
    }


    // Player Input
    private void TimerPosition_Update()
    {
        float yPos = _controller.timer.transform.localPosition.y;

        if (_controller.foodIcon.hasFood)
        {
            _controller.timer.transform.localPosition = new Vector2(_defaultTimerXPos, yPos);
            return;
        }

        _controller.timer.transform.localPosition = new Vector2(0f, yPos);
    }

    private void OnPressStart()
    {
        if (Main_Controller.gamePaused) return;

        _pressDelayCoroutine = StartCoroutine(OnPressStart_Coroutine());
    }
    private IEnumerator OnPressStart_Coroutine()
    {
        TimerPosition_Update();

        _pressStartTime = Time.time;  // Record the time when the button is pressed

        _controller.timer.Set_Time((int)_holdTime);
        _controller.timer.Run_Time();

        yield return new WaitForSeconds(.2f);
        _controller.timer.Toggle_Transparency(false);
    }

    private void OnPressEnd()
    {
        if (Main_Controller.gamePaused) return;

        StopCoroutine(_pressDelayCoroutine);
        _pressDelayCoroutine = null;

        float pressDuration = Time.time - _pressStartTime;

        _controller.timer.Stop_Time();
        _controller.timer.Toggle_Transparency(true);

        if (pressDuration >= _holdTime)
        {
            Hold_Interact();
            return;
        }

        Interact();
    }


    private void Refresh_DetectedInteractables(IInteractable excludeInteractable)
    {
        Detection_Controller detection = _controller.detectionController;

        for (int i = 0; i < detection.All_Interactables().Count; i++)
        {
            if (detection.All_Interactables()[i] == excludeInteractable) continue;
            detection.All_Interactables()[i].UnInteract();
        }
    }

    private IInteractable Closest_Interactable()
    {
        Detection_Controller detection = _controller.detectionController;

        if (detection.Closest_Interactable() == null) return null;
        if (!detection.Closest_Interactable().TryGetComponent(out IInteractable interactable)) return null;

        Refresh_DetectedInteractables(interactable);

        return interactable;
    }


    private void Interact()
    {
        if (Main_Controller.gamePaused) return;

        IInteractable interactable = Closest_Interactable();
        if (interactable == null) return;

        interactable.Interact();
    }

    private void Hold_Interact()
    {
        if (Main_Controller.gamePaused) return;

        IInteractable interactable = Closest_Interactable();

        if (interactable == null)
        {
            Drop_CurrentFood();
            return;
        }

        interactable.Hold_Interact();
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

        foodIcon.Toggle_SubDataBar(true);
    }
}