using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Interaction : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;

    [Header("")]
    [SerializeField] private GameObject _stateBoxes;

    [Header("")]
    [SerializeField] private SpriteRenderer _wakeSpriteRenderer;
    [SerializeField] private SpriteRenderer _eatAnimationSR;

    [Header("")]
    [SerializeField] private SpriteRenderer _goldCoinSR;

    [Header("")]
    [SerializeField] private float _animTransitionTime;
    public float animTransitionTime => _animTransitionTime;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _defaultTimeLimit;
    [SerializeField][Range(0, 100)] private int _generosityMultiplier;

    [Header("")]
    [SerializeField][Range(0, 100)] private float _itemDropRate;
    [SerializeField][Range(0, 100)] private float _collectCardDropRate;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _dropAmountRange;


    private FoodData _servedFoodData;
    public FoodData servedFoodData => _servedFoodData;

    private int _foodScore;
    public int foodScore => _foodScore;

    private Coroutine _startTimeCoroutine;
    private Coroutine _timeLimitCoroutine;
    private Coroutine _eatAnimCoroutine;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }
    }

    private void Start()
    {
        Main_Controller.Change_SpriteAlpha(_wakeSpriteRenderer, 0f);

        // subscriptions
        _controller.interactable.InteractEvent += Interact;
        _controller.interactable.UnInteractEvent += UnInteract;

        _controller.interactable.OnHoldInteract += Interact_FacePlayer;
        _controller.interactable.OnHoldInteract += Exchange_Item;

        _controller.interactable.detection.EnterEvent += Collect_Coin;
        _controller.interactable.detection.ExitEvent += Collect_Coin;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.interactable.InteractEvent -= Interact;
        _controller.interactable.UnInteractEvent -= UnInteract;

        _controller.interactable.OnHoldInteract -= Interact_FacePlayer;
        _controller.interactable.OnHoldInteract -= Exchange_Item;

        _controller.interactable.OnAction1Event -= Serve_FoodOrder;

        _controller.interactable.detection.EnterEvent -= Collect_Coin;
        _controller.interactable.detection.ExitEvent -= Collect_Coin;
    }


    // ActionBubble_Interactable Subscription
    private void Interact()
    {
        Interact_FacePlayer();

        // coin collect
        if (FoodOrder_Served())
        {
            Collect_Coin();
            return;
        }

        if (Main_Controller.orderOpen == false || _controller.foodIcon.hasFood == false) return;

        // show current food order
        _controller.timer.Toggle_Transparency(_controller.interactable.bubble.bubbleOn);

        _controller.foodIcon.Show_Condition();
        StateBox_Toggle();

        _controller.interactable.OnAction1Event += Serve_FoodOrder;
    }

    private void UnInteract()
    {
        StateBox_Toggle();

        if (Main_Controller.orderOpen && _controller.foodIcon.hasFood && _servedFoodData == null)
        {
            _controller.timer.Toggle_Transparency(false);
        }

        _controller.interactable.OnAction1Event -= Serve_FoodOrder;
    }


    // Stop and Face Player if Interacted
    private void Interact_FacePlayer()
    {
        _controller.basicAnim.Flip_Sprite(_controller.interactable.detection.player.gameObject);

        NPC_Movement movement = _controller.movement;
        movement.Stop_FreeRoam();

        if (movement.isLeaving == true)
        {
            movement.Leave(movement.Random_IntervalTime());
        }
        else
        {
            movement.Free_Roam(movement.currentRoamArea, movement.Random_IntervalTime());
        }
    }


    // Toggle On Off StateBox
    private void StateBox_Toggle()
    {
        // on
        if (_controller.interactable.bubble.bubbleOn == true)
        {
            LeanTween.alpha(_stateBoxes, 1f, 0.01f);
        }
        // off
        else
        {
            LeanTween.alpha(_stateBoxes, 0f, 0.01f);
        }
    }


    // wake animation for order open
    public void Wake_Animation()
    {
        StartCoroutine(Wake_Animation_Coroutine());
    }
    private IEnumerator Wake_Animation_Coroutine()
    {
        Main_Controller.Change_SpriteAlpha(_wakeSpriteRenderer, 1f);

        yield return new WaitForSeconds(animTransitionTime);

        Main_Controller.Change_SpriteAlpha(_wakeSpriteRenderer, 0f);
    }


    /// <summary>
    /// Start a time limit for food order at roam area arrival
    /// </summary>
    public void Start_TimeLimit()
    {
        if (_controller.timer.timeRunning == true) return;

        _startTimeCoroutine = StartCoroutine(Start_TimeLimit_Coroutine());
    }
    private IEnumerator Start_TimeLimit_Coroutine()
    {
        Clock_Timer timer = _controller.timer;

        // set the time limit according to npc inpatient level
        int extraTime = (int)_controller.characterData.patienceLevel;
        timer.Set_Time(_defaultTimeLimit + extraTime);

        // lock interaction until npc reaches roam area
        _controller.interactable.LockInteract(true);

        // wait until npc at roam area
        while (_controller.movement.At_CurrentRoamArea() == false)
        {
            yield return null;
        }

        // start time limit
        timer.Run_Time();
        TimeLimit_Over();

        // unlock interaction
        _controller.interactable.LockInteract(false);
    }

    /// <summary>
    /// Checks if time is over and initiates player penalty
    /// </summary>
    public void TimeLimit_Over()
    {
        if (_timeLimitCoroutine != null)
        {
            _timeLimitCoroutine = null;
            return;
        }

        _timeLimitCoroutine = StartCoroutine(TimeLimit_Over_Coroutine());
    }
    private IEnumerator TimeLimit_Over_Coroutine()
    {
        Clock_Timer timer = _controller.timer;

        while (timer.timeRunning) yield return null;

        // hide timer
        _controller.timer.Toggle_Transparency(true);

        // unlock interaction
        _controller.interactable.LockInteract(false);

        // bubble update
        _controller.interactable.bubble.Empty_Bubble();
        _controller.interactable.bubble.Toggle(false);
        StateBox_Toggle();

        _controller.interactable.OnAction1Event -= Serve_FoodOrder;
        Clear_Data();

        // data update
        Character_Data data = _controller.characterData;
        data.Update_Generosity(-data.patienceLevel);

        Update_ServedData();

        // return roam area to current location
        SpriteRenderer currentLocation = _controller.mainController.currentLocation.roamArea;
        NPC_Movement move = _controller.movement;
        move.Stop_FreeRoam();
        move.Free_Roam(currentLocation);
    }


    // Checks
    /// <returns>
    /// NPC will decide whether or not they should get in line for food order (hunger level)
    /// </returns>
    public bool Want_FoodOrder()
    {
        // current order open check
        if (Main_Controller.orderOpen == false) return false;

        // hunger percentage check
        if (Main_Controller.Percentage_Activated(_controller.characterData.hungerLevel)) return true;
        else return false;
    }

    public bool FoodOrder_Served()
    {
        return _servedFoodData != null && _controller.foodIcon.hasFood;
    }


    // Set Random Food Order and State
    public void Assign_FoodOrder()
    {
        // check if food is already served
        if (_servedFoodData != null) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        // check if food order is not set
        if (foodIcon.hasFood == false)
        {
            // set random food from bookmarks (vehicle arhive menu)
            List<Food_ScrObj> bookmarkedFoods = new(_controller.mainController.bookmarkedFoods);

            if (bookmarkedFoods.Count <= 0) return;

            Food_ScrObj randomFood = bookmarkedFoods[Random.Range(0, bookmarkedFoods.Count)];

            foodIcon.Set_CurrentData(new FoodData(randomFood));

            // action bubble sprite update
            _controller.interactable.bubble.Set_Bubble(_controller.foodIcon.currentData.foodScrObj, null);

            // set random state
            ConditionSprites[] allConditions = foodIcon.conditionSprites;

            for (int i = 0; i < allConditions.Length; i++)
            {
                // food order rotten state restriction
                if (allConditions[i].type == FoodCondition_Type.rotten) continue;

                bool setState = Random.value > 0.5f;

                if (setState == false) continue;

                int randLevel = Random.Range(1, allConditions[i].sprites.Length + 1);

                foodIcon.currentData.Update_Condition(new FoodCondition_Data(allConditions[i].type, randLevel));
            }
        }
    }


    // Serve Food Order
    private void Serve_FoodOrder()
    {
        // check if food is already served
        if (_servedFoodData != null) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false) return;

        // check if player has food
        FoodData_Controller playerFoodIcon = _controller.interactable.detection.player.foodIcon;
        if (playerFoodIcon.hasFood == false)
        {
            _controller.interactable.UnInteract();
            return;
        }

        // check if order food is correct
        FoodData playerFoodData = playerFoodIcon.currentData;
        if (playerFoodData.foodScrObj != foodIcon.currentData.foodScrObj)
        {
            _controller.interactable.UnInteract();
            return;
        }

        // stop time limit
        if (_startTimeCoroutine != null) StopCoroutine(_startTimeCoroutine);
        if (_timeLimitCoroutine != null) StopCoroutine(_timeLimitCoroutine);

        _controller.timer.Stop_Time();
        _controller.timer.Toggle_Transparency(true);

        // update data
        _servedFoodData = new FoodData(playerFoodIcon.currentData);
        Update_ServedData();

        // clear player food data
        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Toggle_SubDataBar(true);
        playerFoodIcon.Show_Condition();

        // save food score
        _foodScore = Calculated_FoodScore();

        AbilityManager.OnPointIncrease(1);

        // food eat animation
        Eat_Animation();
    }
    public void Serve_FoodOrder(FoodData serveFoodData)
    {
        // check if food is already served
        if (_servedFoodData != null) return;

        // check if food serve is available
        if (_controller.foodIcon.hasFood == false) return;

        // stop time limit
        if (_startTimeCoroutine != null) StopCoroutine(_startTimeCoroutine);
        if (_timeLimitCoroutine != null) StopCoroutine(_timeLimitCoroutine);

        _controller.timer.Stop_Time();
        _controller.timer.Toggle_Transparency(true);

        // update data
        _servedFoodData = serveFoodData;
        Update_ServedData();

        // save food score
        _foodScore = Calculated_FoodScore();

        AbilityManager.OnPointIncrease(1);

        // food eat animation
        Eat_Animation();
    }

    private void Eat_Animation()
    {
        _eatAnimCoroutine = StartCoroutine(Eat_Animation_Coroutine());
    }
    private IEnumerator Eat_Animation_Coroutine()
    {
        NPC_Movement movement = _controller.movement;
        Food_ScrObj servedFood = _controller.foodIcon.currentData.foodScrObj;

        // stop free roam
        movement.Stop_FreeRoam();
        SpriteRenderer currentArea = _controller.movement.currentRoamArea;

        // lock interaction
        _controller.interactable.LockInteract(true);

        // food sprite
        _eatAnimationSR.sprite = servedFood.sprite;

        // wait
        yield return new WaitForSeconds(animTransitionTime);

        // eat food sprite
        _eatAnimationSR.sprite = servedFood.eatSprite;
        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_eat", transform.position);

        // wait
        yield return new WaitForSeconds(animTransitionTime);

        // no sprite
        _eatAnimationSR.sprite = null;
        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_eat", transform.position);

        // wait
        yield return new WaitForSeconds(animTransitionTime);

        // activate free roam
        movement.Free_Roam(currentArea);

        // unlock interaction
        _controller.interactable.LockInteract(false);

        // lock action bubble
        _controller.interactable.bubble.Empty_Bubble();

        // activate coin sprite
        _goldCoinSR.color = Color.white;

        _eatAnimCoroutine = null;
        yield break;
    }


    // Food Served 
    private int Calculated_FoodScore()
    {
        //
        int defaultScore = _controller.foodIcon.currentData.foodScrObj.price;

        //
        int timeBlock = _controller.timer.timeBlockCount;

        //
        int stateMatch = _controller.foodIcon.currentData.Conditions_MatchCount(_servedFoodData.conditionDatas);

        //
        int bookMarkCount = _controller.mainController.bookmarkedFoods.Count;

        // dialog update
        string dialogInfo = "Time bonus: " + timeBlock + "\nCook bonus: " + stateMatch + "\nBookmark bonus: " + bookMarkCount;

        DialogTrigger dialogTrigger = gameObject.GetComponent<DialogTrigger>();
        dialogTrigger.Set_DefaultDialog(new DialogData(dialogTrigger.defaultData.icon, dialogInfo));

        //
        return defaultScore + timeBlock + stateMatch + bookMarkCount;
    }


    public void Update_ServedData()
    {
        Character_Data data = _controller.characterData;

        data.Update_Hunger(-data.hungerLevel);

        if (_servedFoodData == null) return;
        FoodData orderFoodData = _controller.foodIcon.currentData;

        int stateMatch = orderFoodData.Conditions_MatchCount(_servedFoodData.conditionDatas) * _generosityMultiplier;
        data.Update_Generosity(_generosityMultiplier + stateMatch);
    }

    public void Collect_Coin()
    {
        if (_eatAnimCoroutine != null) return;
        if (FoodOrder_Served() == false) return;

        _controller.mainController.Add_GoldenNugget(_foodScore);
        _foodScore = 0;

        // toss coin to player animation
        Sprite coinSprite = _controller.mainController.dataController.goldenNugget.sprite;

        _controller.itemLauncher.Parabola_CoinLaunch(coinSprite, transform.position);

        // leave current location
        NPC_Movement move = _controller.movement;
        SpriteRenderer currentLocation = _controller.mainController.currentLocation.roamArea;

        move.Update_RoamArea(currentLocation);
        move.Leave(move.Random_IntervalTime());

        Clear_Data();

        // dialog update
        gameObject.GetComponent<DialogTrigger>().Update_Dialog();
    }

    public void Clear_Data()
    {
        // clear data
        _servedFoodData = null;

        _controller.foodIcon.Set_CurrentData(null);
        _controller.foodIcon.Show_Icon();
        _controller.foodIcon.Show_Condition();

        // hide coin sprite
        _goldCoinSR.color = Color.clear;
    }


    // Exchange Item
    private void Exchange_Item()
    {
        // check if item drop activated
        if (_controller.itemDropper.enabled == false) return;

        // check if food serve waiting
        if (_controller.foodIcon.hasFood) return;

        Player_Controller player = _controller.interactable.detection.player;
        FoodData_Controller playerFoodIcon = player.foodIcon;

        if (playerFoodIcon.hasFood == false) return;
        Food_ScrObj playerFood = playerFoodIcon.currentData.foodScrObj;

        // remove player current food
        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Toggle_SubDataBar(true);
        playerFoodIcon.Show_Condition();

        // disable item dropper
        _controller.itemDropper.enabled = false;

        // check drop rate
        if (Main_Controller.Percentage_Activated(_controller.characterData.generosityLevel, _itemDropRate) == false) return;

        ItemDropper dropper = _controller.itemDropper;

        // collect card drop
        if (Main_Controller.Percentage_Activated(_controller.characterData.generosityLevel, _collectCardDropRate))
        {
            dropper.Drop_CollectCard();
            return;
        }

        // food drop
        int dropAmount = Random.Range(1, _dropAmountRange + 1);
        dropper.Drop_Food(new FoodData(dropper.Weighted_RandomFood(playerFood)), dropAmount);
    }
}