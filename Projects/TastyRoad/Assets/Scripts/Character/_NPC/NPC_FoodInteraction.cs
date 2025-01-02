using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_FoodInteraction : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _foodOrderRate;

    [Header("")]
    [SerializeField][Range(0, 300)] private int _transferTime;
    [SerializeField][Range(0, 100)] private int _additionalTime;


    private Coroutine _timeCoroutine;
    private Coroutine _transferCoroutine;


    // MonoBehaviour
    private void Start()
    {
        // subscriptions
        GlobalTime_Controller.TimeTik_Update += Set_FoodOrder;

        ActionBubble_Interactable interactable = _controller.interactable;
        interactable.OnIInteract += Transfer_FoodOrder;

        NPC_GiftSystem giftSystem = _controller.giftSystem;
        giftSystem.OnDurationToggle += Toggle_FoodOrder;
    }

    private void OnDestroy()
    {
        // subscriptions
        GlobalTime_Controller.TimeTik_Update -= Set_FoodOrder;

        ActionBubble_Interactable interactable = _controller.interactable;
        interactable.OnIInteract -= Transfer_FoodOrder;

        NPC_GiftSystem giftSystem = _controller.giftSystem;
        giftSystem.OnDurationToggle -= Toggle_FoodOrder;
    }


    // Indications
    public void Toggle_FoodOrder(bool toggle)
    {
        Clock_Timer timer = _controller.timer;
        FoodData_Controller foodIcon = _controller.foodIcon;

        if (toggle == false)
        {
            if (foodIcon.hasFood == false) return;

            foodIcon.Hide_Icon();
            foodIcon.Hide_Condition();

            timer.Toggle_Transparency(true);

            return;
        }

        foodIcon.Show_Icon();
        foodIcon.Show_Condition();

        timer.Toggle_Transparency(false);
    }


    // Movement
    private void Update_RoamArea()
    {
        NPC_Movement movement = _controller.movement;

        Location_Controller currentLocation = _controller.mainController.currentLocation;
        SpriteRenderer locationRoamArea = currentLocation.data.roamArea;

        if (_controller.foodIcon.hasFood == false)
        {
            movement.Free_Roam(locationRoamArea, 0f);
            return;
        }

        if (movement.currentRoamArea != locationRoamArea) return;
        movement.Free_Roam(_controller.mainController.currentVehicle.interactArea, 0f);
    }


    // Set Food Order
    private FoodData FoodOrder()
    {
        List<Food_ScrObj> bookMarks = _controller.mainController.bookmarkedFoods;
        if (bookMarks.Count <= 0) return null;

        return new FoodData(bookMarks[Random.Range(0, bookMarks.Count)]);
    }


    private bool SetOrder_Active()
    {
        if (_timeCoroutine != null) return false;

        // check if additional food orders are left
        if (_controller.foodIcon.AllDatas().Count > 0) return false;

        List<Food_ScrObj> bookMarks = _controller.mainController.bookmarkedFoods;
        if (bookMarks.Count <= 0) return false;

        return true;
    }

    private void Set_FoodOrder()
    {
        if (SetOrder_Active() == false) return;

        FoodData_Controller foodIcon = _controller.foodIcon;
        Character_Data characterData = _controller.characterData;

        float calculatedHunger = (100 - characterData.hungerLevel) / 100;
        float foodOrderRate = _foodOrderRate / 100;

        int maxOrders = Mathf.CeilToInt(calculatedHunger * foodIcon.maxDataCount * foodOrderRate);

        for (int i = 0; i < maxOrders; i++)
        {
            foodIcon.Set_CurrentData(FoodOrder());
        }

        if (foodIcon.hasFood == false) return;
        foodIcon.Show_Icon(0.5f);

        Run_OrderTime();
        Update_RoamArea();
    }


    // Run Time
    private void Run_OrderTime()
    {
        _timeCoroutine = StartCoroutine(Run_OrderTime_Coroutine());
    }
    private IEnumerator Run_OrderTime_Coroutine()
    {
        Clock_Timer orderTimer = _controller.timer;
        int additionalTime = Mathf.CeilToInt(_controller.characterData.patienceLevel / 10 * _additionalTime);

        orderTimer.Set_Time(_transferTime + additionalTime);
        orderTimer.Run_Time();
        orderTimer.Toggle_Transparency(false);

        while (orderTimer.timeRunning) yield return null;

        Fail_OrderTime();

        _timeCoroutine = null;
        yield break;
    }

    private void Fail_OrderTime()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;

        foodIcon.Update_AllDatas(null);
        foodIcon.Show_Icon();

        Update_RoamArea();
    }


    // Transfer Food Order
    private bool Transfer_FoodOrder(FoodData transferData)
    {
        if (_transferCoroutine != null) return false;
        if (_timeCoroutine == null) return false;

        if (transferData == null) return false;
        if (_controller.foodIcon.Is_SameFood(transferData.foodScrObj) == false) return false;

        StopCoroutine(_timeCoroutine);
        _timeCoroutine = null;

        _controller.timer.Stop_Time();
        _transferCoroutine = StartCoroutine(Transfer_FoodOrder_Coroutine(transferData));

        return true;
    }
    private IEnumerator Transfer_FoodOrder_Coroutine(FoodData transferData)
    {
        FoodData_Controller foodIcon = _controller.foodIcon;
        foodIcon.Show_Icon();

        yield return new WaitForSeconds(1f);

        foodIcon.Show_EatIcon();

        yield return new WaitForSeconds(1f);

        Character_Data data = _controller.characterData;
        data.Update_Hunger((100 - data.hungerLevel) / foodIcon.AllDatas().Count);

        foodIcon.Set_CurrentData(null);
        foodIcon.Hide_Icon();

        _controller.timer.Toggle_Transparency(true);

        yield return new WaitForSeconds(1f);

        if (foodIcon.hasFood == false)
        {
            _transferCoroutine = null;
            yield break;
        }

        Run_OrderTime();
        foodIcon.Show_Icon(0.5f);

        _transferCoroutine = null;
        yield break;
    }

    private void Transfer_FoodOrder()
    {
        if (_transferCoroutine != null) return;

        FoodData_Controller playerIcon = _controller.interactable.detection.player.foodIcon;

        if (Transfer_FoodOrder(playerIcon.currentData) == false) return;

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
    }


    // Payment
    private int Pay_FoodOrder()
    {
        int payAmount = 0;

        // remaining time calculation
        // food price

        return payAmount;
    }
}