using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC_FoodInteraction : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;

    [Space(20)]
    [SerializeField] private GameObject _collectIndicator;
    
    [Space(20)]
    [SerializeField][Range(0, 300)] private int _transferTime;

    [Space(20)]
    [SerializeField][Range(0, 100)] private float _conditionRequestRate;
    [SerializeField][Range(0, 100)] private int _conditionBonusPay;
    
    [Space(60)]
    [SerializeField] private Ability_ScrObj _foodOrderAbility;
    [SerializeField] private Ability_ScrObj _goldMagnetAbility;


    private int _foodOrderCount;

    private FoodData _transferData;
    private List<FoodData> _transferDatas = new();

    private bool _payAvailable;
    public bool payAvailable => _payAvailable;


    private Coroutine _timeCoroutine;
    public Coroutine timeCoroutine => _timeCoroutine;

    private Coroutine _transferCoroutine;
    public Coroutine transferCoroutine => _transferCoroutine;


    // MonoBehaviour
    private void Start()
    {
        _collectIndicator.SetActive(false);

        // subscriptions
        globaltime.instance.OnTimeTik += Set_FoodOrder;

        ActionBubble_Interactable interactable = _controller.interactable;
        interactable.OnInteract += Transfer_FoodOrder;
        interactable.OnInteract += Collect_Payment;

        NPC_GiftSystem giftSystem = _controller.giftSystem;
        giftSystem.OnDurationToggle += Toggle_FoodOrder;

        Detection_Controller detection = interactable.detection;
        detection.EnterEvent += Collect_Payment;
    }

    private void OnDestroy()
    {
        // subscriptions
        globaltime.instance.OnTimeTik -= Set_FoodOrder;

        ActionBubble_Interactable interactable = _controller.interactable;
        interactable.OnInteract -= Transfer_FoodOrder;
        interactable.OnInteract -= Collect_Payment;

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
    public void Update_RoamArea()
    {
        NPC_Movement movement = _controller.movement;

        // check if food order time failed || tranfer complete
        if (_foodOrderCount > 0 && _timeCoroutine == null && !_payAvailable)
        {
            movement.Leave(movement.Random_IntervalTime());
            return;
        }

        Main_Controller main = Main_Controller.instance;

        Location_Controller currentLocation = main.currentLocation;
        SpriteRenderer locationRoamArea = currentLocation.data.roamArea;

        if (_controller.foodIcon.hasFood == false)
        {
            movement.Free_Roam(locationRoamArea, 0f);
            return;
        }

        movement.CurrentLocation_FreeRoam(main.currentVehicle.interactArea, 1f);
    }


    // Set Food Order
    private FoodData New_FoodOrder()
    {
        List<Food_ScrObj> bookMarks = Main_Controller.instance.bookmarkedFoods;
        if (bookMarks.Count <= 0) return null;

        List<FoodWeight_Data> foodWeights = new();
        float totalWeight = 0f;

        FoodData_Controller foodIcon = _controller.foodIcon;

        foreach (Food_ScrObj food in bookMarks)
        {
            float duplicatePenalty = 100f / foodIcon.maxDataCount * foodIcon.FoodCount(food);
            float setWeight = Mathf.Clamp(100f - duplicatePenalty, 0f, 100f);

            totalWeight += setWeight;
            foodWeights.Add(new FoodWeight_Data(food, Mathf.RoundToInt(setWeight)));
        }

        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        while (foodWeights.Count > 0)
        {
            int randomIndex = Random.Range(0, foodWeights.Count);
            FoodWeight_Data randomFood = foodWeights[randomIndex];

            cumulativeWeight += randomFood.weight;

            if (randValue > cumulativeWeight)
            {
                foodWeights.Remove(randomFood);
                continue;
            }

            return new FoodData(randomFood.foodScrObj);
        }

        return null;
    }

    private bool SetOrder_Active()
    {
        if (_controller.movement.isLeaving) return false;
        if (_foodOrderCount > 0) return false;
        if (_timeCoroutine != null) return false;
        if (_payAvailable) return false;

        // check if additional food orders are left
        if (_controller.foodIcon.AllDatas().Count > 0) return false;

        List<Food_ScrObj> bookMarks = Main_Controller.instance.bookmarkedFoods;
        if (bookMarks.Count <= 0) return false;

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        if (currentLocation.FoodOrderNPC_Maxed()) return false;

        return true;
    }

    private void Set_FoodOrder()
    {
        if (SetOrder_Active() == false) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        int maxOrders = foodIcon.maxDataCount;
        maxOrders = Mathf.Clamp(maxOrders, 0, foodIcon.maxDataCount);

        for (int i = 0; i < maxOrders; i++)
        {
            FoodData setData = new(New_FoodOrder());
            if (setData == null) continue;

            foodIcon.Set_CurrentData(setData);
            _foodOrderCount++;

            if (Main_Controller.instance.Percentage_Activated(_conditionRequestRate) == false) continue;

            List<FoodCondition_Type> availableConditions = setData.foodScrObj.Available_SetConditions();
            FoodCondition_Type randCondition = availableConditions[Random.Range(0, availableConditions.Count)];

            int randLevel = Random.Range(1, 4);

            setData.Update_Condition(new FoodCondition_Data(randCondition, randLevel));
        }

        if (foodIcon.hasFood == false)
        {
            Main_Controller.instance.currentLocation.Track_FoodOrderNPC(_controller, false);
            return;
        }

        foodIcon.Show_Icon(0.5f);
        foodIcon.Show_Condition();

        Run_OrderTime();
        Update_RoamArea();
        
        Main_Controller.instance.currentLocation.Track_FoodOrderNPC(_controller, true);
    }


    // Run Time
    private void Run_OrderTime()
    {
        _timeCoroutine = StartCoroutine(Run_OrderTime_Coroutine());
    }
    private IEnumerator Run_OrderTime_Coroutine()
    {
        Clock_Timer orderTimer = _controller.timer;

        orderTimer.Toggle_ClockColor(false);
        orderTimer.Set_Time(_transferTime);
        orderTimer.Run_Time();
        orderTimer.Toggle_Transparency(false);

        while (orderTimer.timeRunning) yield return null;

        Fail_OrderTime();
        orderTimer.Toggle_Transparency(true);

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
        
        Main_Controller.instance.currentLocation.Track_FoodOrderNPC(_controller, false);
    }


    // Transfer Food Order
    public bool Transfer_FoodOrder(FoodData transferData)
    {
        if (_transferCoroutine != null) return false;
        if (_timeCoroutine == null) return false;

        if (transferData == null) return false;
        if (_controller.foodIcon.Is_SameFood(transferData.foodScrObj) == false) return false;

        StopCoroutine(_timeCoroutine);
        _timeCoroutine = null;

        _controller.timer.Stop_Time();
        _controller.timer.Toggle_ClockColor(true);

        _transferCoroutine = StartCoroutine(Transfer_FoodOrder_Coroutine());

        _transferData = new(transferData);
        _transferDatas.Add(new(transferData));
        
        AbilityManager.IncreasePoint(1);
        TutorialQuest_Controller.instance.Complete_Quest("Serve" + transferData.foodScrObj.name, 1);

        return true;
    }
    private IEnumerator Transfer_FoodOrder_Coroutine()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;

        foodIcon.Show_Icon();
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        foodIcon.Hide_Condition();

        yield return new WaitForSeconds(1f);

        foodIcon.Show_EatIcon();
        Audio_Controller.instance.Play_OneShot(gameObject, 1);

        yield return new WaitForSeconds(1f);

        foodIcon.Hide_Icon();
        Audio_Controller.instance.Play_OneShot(gameObject, 1);

        _controller.timer.Toggle_Transparency(true);

        yield return new WaitForSeconds(1f);

        // fail
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
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
    }


    // Payment
    public int Set_Payment()
    {
        int payAmount = 0;

        if (!_transferData.Has_Condition(FoodCondition_Type.rotten))
        {
            FoodData_Controller foodIcon = _controller.foodIcon;

            FoodData orderData = foodIcon.currentData;
            Food_ScrObj foodOrder = orderData.foodScrObj;

            // defalut calculation
            payAmount += foodOrder.price;

            // condition match calculation
            payAmount += orderData.Conditions_MatchCount(_transferData.conditionDatas) * _conditionBonusPay;
        }

        _payAvailable = payAmount > 0;
        _collectIndicator.SetActive(_payAvailable);

        return payAmount;
    }


    public void Collect_Payment(int payAmount)
    {
        if (_payAvailable == false) return;

        GoldSystem goldSystem = GoldSystem.instance;
        goldSystem.Update_CurrentAmount(payAmount);

        FoodData_Controller foodIcon = _controller.foodIcon;
        foodIcon.Set_CurrentData(null);

        _payAvailable = false;
        _collectIndicator.SetActive(false);

        _transferData = null;

        Sprite nuggetSprite = goldSystem.defaultIcon;
        _controller.itemLauncher.Parabola_CoinLaunch(nuggetSprite, transform.position);

        // sfx
        Audio_Controller.instance.Play_OneShot(gameObject, 2);

        bool npcFull = Main_Controller.instance.currentLocation.FoodOrderNPC_Maxed();
        
        if (foodIcon.hasFood == false || Main_Controller.instance.bookmarkedFoods.Count <= 0 || npcFull)
        {
            foodIcon.Update_AllDatas(null);
            foodIcon.Show_Icon();
            foodIcon.Show_Condition();

            Update_RoamArea();
            
            Main_Controller.instance.currentLocation.Track_FoodOrderNPC(_controller, false);
            return;
        }

        // run next food order
        Run_OrderTime();

        foodIcon.Show_Icon(0.5f);
        foodIcon.Show_Condition();

        Update_RoamArea();
    }

    private void Collect_Payment()
    {
        if (_payAvailable == false) return;
        Collect_Payment(Set_Payment());
    }
}