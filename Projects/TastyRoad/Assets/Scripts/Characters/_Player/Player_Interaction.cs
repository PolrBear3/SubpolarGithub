using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    private Player_Controller _controller;


    [Space(20)]
    [SerializeField][Range(0, 100)] private float _holdTime;
    [SerializeField] private GameObject _disposeFoodPrefab;

    
    private float _defaultTimerXPos;
    private Coroutine _timerCoroutine;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller controller)) { _controller = controller; }
        
        _defaultTimerXPos = _controller.timer.transform.localPosition.x;
    }

    private void Start()
    {
        IndicationTrigger_CurrentFood();

        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnInteractStart += Update_HoldTimer;
        input.OnInteract += Update_HoldTimer;
        input.OnHoldInteract += Update_HoldTimer;

        input.OnInteractStart += PositionUpdate_HoldTimer;

        input.OnInteract += Interact;
        input.OnHoldInteract += HoldInteract;
        
        input.OnAction1 += Action1;
        input.OnAction2 += Action2;

        _controller.foodIcon.OnCurrentDataSet += IndicationTrigger_CurrentFood;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnInteractStart -= Update_HoldTimer;
        input.OnInteract -= Update_HoldTimer;
        input.OnHoldInteract -= Update_HoldTimer;

        input.OnInteractStart -= PositionUpdate_HoldTimer;

        input.OnInteract -= Interact;
        input.OnHoldInteract -= HoldInteract;
        
        input.OnAction1 -= Action1;
        input.OnAction2 -= Action2;
        
        _controller.foodIcon.OnCurrentDataSet -= IndicationTrigger_CurrentFood;
    }


    // Input Controller
    private void Refresh_DetectedInteractables(IInteractable excludeInteractable)
    {
        Detection_Controller detection = _controller.detection;

        for (int i = 0; i < detection.All_Interactables().Count; i++)
        {
            if (detection.All_Interactables()[i] == excludeInteractable) continue;
            detection.All_Interactables()[i].UnInteract();
        }
    }

    private IInteractable Closest_Interactable()
    {
        Detection_Controller detection = _controller.detection;
        
        if (detection.Closest_Interactable() == null) return null;
        if (!detection.Closest_Interactable().TryGetComponent(out IInteractable interactable)) return null;

        Refresh_DetectedInteractables(interactable);
        return interactable;
    }


    // IInteractable
    private void Interact()
    {
        if (enabled == false) return;

        IInteractable interactable = Closest_Interactable();
        if (interactable == null) return;
        
        interactable.Interact();
    }

    private void HoldInteract()
    {
        if (enabled == false) return;

        IInteractable interactable = Closest_Interactable();
        if (interactable == null)
        {
            Dispose_CurrentFood();
            return;
        }
        
        interactable.Hold_Interact();
    }

    private void Action1()
    {
        IInteractable interactable = Closest_Interactable();
        if (interactable == null) return;
        
        interactable.Action1();
    }

    private void Action2()
    {
        IInteractable interactable = Closest_Interactable();
        if (interactable == null) return;
        
        interactable.Action2();
    }


    // Hold Timer
    private void PositionUpdate_HoldTimer()
    {
        float yPos = _controller.timer.transform.localPosition.y;

        if (_controller.foodIcon.hasFood)
        {
            _controller.timer.transform.localPosition = new Vector2(_defaultTimerXPos, yPos);
            return;
        }

        _controller.timer.transform.localPosition = new Vector2(0f, yPos);
    }

    private void Update_HoldTimer()
    {
        if (enabled == false) return;

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        if (Input_Controller.instance.isHolding == false)
        {
            _controller.timer.Stop_Time();
            _controller.timer.Toggle_Transparency(true);

            return;
        }

        _timerCoroutine = StartCoroutine(Update_HoldTimer_Coroutine());
    }
    private IEnumerator Update_HoldTimer_Coroutine()
    {
        _controller.timer.Set_Time((int)_holdTime);
        _controller.timer.Run_Time();

        // slight delay before showing timer
        yield return new WaitForSeconds(0.2f);

        _controller.timer.Toggle_Transparency(false);

        _timerCoroutine = null;
        yield break;
    }


    // Player Interact Actions
    public void Dispose_CurrentFood()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false) return;

        CoinLauncher launcher = _controller.coinLauncher;
        launcher.Parabola_CoinLaunch(foodIcon.currentData.foodScrObj.sprite, transform.position);

        StartCoroutine(FoodDispose_DelayCoroutine(foodIcon.currentData.foodScrObj, launcher.range));

        foodIcon.Set_CurrentData(null);
        foodIcon.Show_Icon();
        foodIcon.Show_Condition();
        foodIcon.Toggle_SubDataBar(true);
        
        IndicationTrigger_CurrentFood();
    }
    private IEnumerator FoodDispose_DelayCoroutine(Food_ScrObj disposeFood, float delayTime)
    {
        Vector2 dropPos = transform.position;
        
        yield return new WaitForSeconds(delayTime);
        
        GameObject disposePrefab = Instantiate(_disposeFoodPrefab, dropPos, Quaternion.identity);
        Dispose_FoodDrop disposeFoodDrop = disposePrefab.GetComponent<Dispose_FoodDrop>();
        
        disposeFoodDrop.Set_FoodData(disposeFood);
        disposeFoodDrop.Fade_Destroy();
    }
    
    
    public void IndicationTrigger_CurrentFood()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;
        InteractIndicator_Controller indicator = InteractIndicator_Controller.instance;
        
        if (foodIcon.hasFood == false)
        {
            indicator.Trigger(null, null);
            return;
        }
        
        Food_ScrObj currentFood = foodIcon.currentData.foodScrObj;
        
        Sprite foodSprite = currentFood.sprite;
        string foodName = currentFood.LocalizedName();
        
        indicator.Trigger(foodSprite, foodName);
    }
}