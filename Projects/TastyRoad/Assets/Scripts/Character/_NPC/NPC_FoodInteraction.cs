using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_FoodInteraction : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;


    [Header("")]
    [SerializeField] private GameObject _collectIndicator;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _foodOrderRate;
    [SerializeField][Range(0, 100)] private float _conditionRequestRate;

    [Header("")]
    [SerializeField][Range(0, 300)] private int _transferTime;
    [SerializeField][Range(0, 100)] private int _additionalTime;

    [Header("")]
    [SerializeField][Range(0, 100)] private float _bonusPayPercentage;


    private FoodData _transferData;

    private bool _payAvailable;
    public bool payAvailable => _payAvailable;


    private Coroutine _timeCoroutine;
    public Coroutine timeCoroutine => _timeCoroutine;

    private Coroutine _transferCoroutine;


    // MonoBehaviour
    private void Start()
    {
        _collectIndicator.SetActive(false);

        // subscriptions
        GlobalTime_Controller.TimeTik_Update += Set_FoodOrder;

        ActionBubble_Interactable interactable = _controller.interactable;
        interactable.OnIInteract += Transfer_FoodOrder;
        interactable.OnIInteract += Collect_Payment;

        NPC_GiftSystem giftSystem = _controller.giftSystem;
        giftSystem.OnDurationToggle += Toggle_FoodOrder;

        Detection_Controller detection = interactable.detection;
        detection.EnterEvent += Collect_Payment;
    }

    private void OnDestroy()
    {
        // subscriptions
        GlobalTime_Controller.TimeTik_Update -= Set_FoodOrder;

        ActionBubble_Interactable interactable = _controller.interactable;
        interactable.OnIInteract -= Transfer_FoodOrder;
        interactable.OnIInteract -= Collect_Payment;

        NPC_GiftSystem giftSystem = _controller.giftSystem;
        giftSystem.OnDurationToggle -= Toggle_FoodOrder;

        Detection_Controller detection = interactable.detection;
        detection.EnterEvent -= Collect_Payment;
    }


    // Indications
    public void Toggle_FoodOrder(bool toggle)
    {
        Clock_Timer timer = _controller.timer;
        FoodData_Controller foodIcon = _controller.foodIcon;

        if (toggle == false)
        {
            foodIcon.Hide_Icon();
            foodIcon.Hide_Condition();

            timer.Toggle_Transparency(true);

            return;
        }

        if (foodIcon.hasFood == false) return;

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

        if (_payAvailable) return false;

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

            if (Main_Controller.Percentage_Activated(_conditionRequestRate) == false) continue;

            FoodCondition_Type randCondition = (FoodCondition_Type)Random.Range(0, 2);
            int randLevel = Random.Range(1, 4);

            foodIcon.currentData.Update_Condition(new FoodCondition_Data(randCondition, randLevel));
        }

        if (foodIcon.hasFood == false) return;

        foodIcon.Show_Icon(0.5f);
        foodIcon.Show_Condition();

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
        _timeCoroutine = null;
        _transferData = null;

        _collectIndicator.SetActive(false);

        FoodData_Controller foodIcon = _controller.foodIcon;

        foodIcon.Update_AllDatas(null);
        foodIcon.Show_Icon();
        foodIcon.Show_Condition();

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
        _transferCoroutine = StartCoroutine(Transfer_FoodOrder_Coroutine());

        _transferData = new(transferData);

        return true;
    }
    private IEnumerator Transfer_FoodOrder_Coroutine()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;

        foodIcon.Show_Icon();
        foodIcon.Hide_Condition();

        yield return new WaitForSeconds(1f);

        foodIcon.Show_EatIcon();

        yield return new WaitForSeconds(1f);

        foodIcon.Hide_Icon();
        _controller.timer.Toggle_Transparency(true);

        Character_Data data = _controller.characterData;
        data.Update_Hunger((100 - data.hungerLevel) / foodIcon.AllDatas().Count);

        if (Set_Payment() <= 0) Fail_OrderTime();

        Update_RoamArea();

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
        playerIcon.Show_Condition();
    }


    // Payment
    public int Set_Payment()
    {
        int payAmount = 0;

        FoodData_Controller foodIcon = _controller.foodIcon;
        Food_ScrObj foodOrder = foodIcon.currentData.foodScrObj;

        int bonusAmount = Mathf.CeilToInt(_bonusPayPercentage * 0.01f * foodOrder.price);

        // defalut calculation
        payAmount += foodOrder.price;

        // condition match calculation
        if (foodIcon.currentData.Conditions_MatchCount(_transferData.conditionDatas) >= foodIcon.currentData.conditionDatas.Count)
        {
            foreach (FoodCondition_Data data in foodIcon.currentData.conditionDatas)
            {
                payAmount += data.level * bonusAmount;
            }
        }

        // order time calculation
        payAmount += _controller.timer.timeBlockCount * bonusAmount;

        // rotten condition calculation
        payAmount -= payAmount / 3 * _transferData.Current_ConditionLevel(FoodCondition_Type.rotten);

        _payAvailable = payAmount > 0;
        _collectIndicator.SetActive(payAmount > 0);

        return payAmount;
    }

    public void Collect_Payment()
    {
        if (_payAvailable == false) return;

        _controller.mainController.Add_GoldenNugget(Set_Payment());

        FoodData_Controller foodIcon = _controller.foodIcon;
        foodIcon.Set_CurrentData(null);

        _payAvailable = false;
        _collectIndicator.SetActive(false);

        _transferData = null;

        Sprite nuggetSprite = _controller.mainController.dataController.goldenNugget.sprite;
        _controller.itemLauncher.Parabola_CoinLaunch(nuggetSprite, transform.position);

        Update_RoamArea();

        if (foodIcon.hasFood == false) return;

        // run next food order
        Run_OrderTime();

        foodIcon.Show_Icon(0.5f);
        foodIcon.Show_Condition();
    }
}