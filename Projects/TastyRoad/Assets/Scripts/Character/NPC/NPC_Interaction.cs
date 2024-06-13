using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Interaction : MonoBehaviour, IInteractable
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

    private bool _foodOrderServed;
    public bool foodOrderServed => _foodOrderServed;

    private int _foodScore;
    public int foodScore => _foodScore;

    [Header("Adjust Data")]
    [SerializeField] private float _animTransitionTime;
    public float animTransitionTime => _animTransitionTime;

    [SerializeField] private Vector2 roamDelayTime;

    [SerializeField] private int _defaultTimeLimit;

    private Coroutine _startTimeCoroutine;
    private Coroutine _timeLimitCoroutine;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }
    }

    private void Start()
    {
        Main_Controller.Change_SpriteAlpha(_wakeSpriteRenderer, 0f);
    }


    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
    }


    // IInteractable
    public void Interact()
    {
        Interact_FacePlayer();

        if (_foodOrderServed && _controller.foodIcon.hasFood)
        {
            Collect_Coin();
            return;
        }

        if (_controller.actionBubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        if (Main_Controller.orderOpen == false) return;
        if (_controller.foodIcon.hasFood == false) return;

        _controller.InputToggle(true);

        _controller.timer.Toggle_Transparency(!_controller.actionBubble.bubbleOn);

        ActionBubble_Toggle();
        StateBox_Toggle();

        _controller.Action1 += Serve_FoodOrder;
    }

    public void UnInteract()
    {
        _controller.InputToggle(false);

        _controller.actionBubble.Toggle(false);
        StateBox_Toggle();

        if (Main_Controller.orderOpen && _controller.foodIcon.hasFood && _foodOrderServed == false)
        {
            _controller.timer.Toggle_Transparency(false);
        }

        _controller.Action1 -= Serve_FoodOrder;
    }


    // Stop and Face Player if Interacted
    private void Interact_FacePlayer()
    {
        if (_controller.actionBubble.bubbleOn == true) return;

        _controller.basicAnim.Flip_Sprite(_controller.detection.player.gameObject);

        NPC_Movement movement = _controller.movement;

        movement.Stop_FreeRoam();

        if (movement.isLeaving == true)
        {
            movement.Leave(Random.Range(roamDelayTime.x, roamDelayTime.y));
        }
        else
        {
            movement.Free_Roam(movement.currentRoamArea, Random.Range(roamDelayTime.x, roamDelayTime.y));
        }
    }

    // Update Action Bubble
    private void ActionBubble_Toggle()
    {
        if (_foodOrderServed == true) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false) return;

        _controller.actionBubble.Update_Bubble(foodIcon.currentData.foodScrObj, null);
        _controller.foodIcon.Show_Condition();
    }

    // Toggle On Off StateBox
    private void StateBox_Toggle()
    {
        // on
        if (_controller.actionBubble.bubbleOn == true)
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
        int extraTime = (int)_controller.characterData.inPatienceLevel;
        timer.Set_Time(_defaultTimeLimit + extraTime);

        // wait until npc at roam area
        while (_controller.movement.At_CurrentRoamArea() == false)
        {
            yield return null;
        }

        // start time limit
        timer.Run_Time();
        TimeLimit_Over();
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

        // Uninteract functions
        _controller.InputToggle(false);

        _controller.actionBubble.Toggle(false);
        StateBox_Toggle();

        _controller.Action1 -= Serve_FoodOrder;

        // clear food data
        _controller.foodIcon.Set_CurrentData(null);
        _controller.foodIcon.Show_Icon();
        _controller.foodIcon.Show_Condition();

        // hunger level to 0
        Character_Data data = _controller.characterData;
        data.Update_Hunger(-data.hungerLevel);

        // return roam area to current location
        SpriteRenderer currentLocation = _controller.mainController.currentLocation.roamArea;
        NPC_Movement move = _controller.movement;
        move.Stop_FreeRoam();
        move.Free_Roam(currentLocation, Random.Range(roamDelayTime.x, roamDelayTime.y));
    }


    /// <returns>
    /// NPC will decide whether or not they should get in line for food order
    /// </returns>
    public bool Want_FoodOrder()
    {
        // current order open check
        if (Main_Controller.orderOpen == false) return false;

        // hunger percentage check
        if (Main_Controller.Percentage_Activated(_controller.characterData.hungerLevel)) return true;
        else return false;
    }

    // Set Random Food Order and State
    public void Assign_FoodOrder()
    {
        if (_foodOrderServed == true) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false)
        {
            // set random food from bookmarks (vehicle arhive menu)
            List<Food_ScrObj> bookmarkedFoods = new(_controller.mainController.bookmarkedFoods);

            if (bookmarkedFoods.Count <= 0) return;

            Food_ScrObj randomFood = bookmarkedFoods[Random.Range(0, bookmarkedFoods.Count)];

            foodIcon.Set_CurrentData(new FoodData(randomFood));

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
        if (_foodOrderServed == true) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false) return;

        // check if player has food
        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;
        if (playerFoodIcon.hasFood == false)
        {
            UnInteract();
            return;
        }

        _controller.actionBubble.Toggle(false);

        // check if order food is correct
        FoodData playerFoodData = playerFoodIcon.currentData;
        if (playerFoodData.foodScrObj != foodIcon.currentData.foodScrObj)
        {
            UnInteract();
            return;
        }

        // stop time limit
        if (_startTimeCoroutine != null) StopCoroutine(_startTimeCoroutine);
        if (_timeLimitCoroutine != null) StopCoroutine(_timeLimitCoroutine);

        _controller.timer.Stop_Time();
        _controller.timer.Toggle_Transparency(true);

        // save food score
        _foodScore = Calculated_FoodScore();

        // clear player food data
        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();

        // update data
        _foodOrderServed = true;
        _controller.characterData.Update_Hunger(-_controller.characterData.hungerLevel);

        // food eat animation
        Eat_Animation();
    }

    // Eat Food Animation after Food Order is Served
    private void Eat_Animation()
    {
        StartCoroutine(Eat_Animation_Coroutine());
    }
    private IEnumerator Eat_Animation_Coroutine()
    {
        Detection_Controller detection = _controller.detection;
        NPC_Movement movement = _controller.movement;
        Food_ScrObj servedFood = _controller.foodIcon.currentData.foodScrObj;

        // stop free roam
        movement.Stop_FreeRoam();
        SpriteRenderer currentArea = _controller.movement.currentRoamArea;

        // turn off collider
        detection.BoxCollider_Toggle(false);

        // food sprite
        _eatAnimationSR.sprite = servedFood.sprite;

        // wait
        yield return new WaitForSeconds(animTransitionTime);

        // eat food sprite
        _eatAnimationSR.sprite = servedFood.eatSprite;

        // wait
        yield return new WaitForSeconds(animTransitionTime);

        // no sprite
        _eatAnimationSR.sprite = null;

        // wait
        yield return new WaitForSeconds(animTransitionTime);

        // activate free roam
        movement.Free_Roam(currentArea, Random.Range(roamDelayTime.x, roamDelayTime.y));

        // turn on collider
        detection.BoxCollider_Toggle(true);

        // activate coin sprite
        _goldCoinSR.color = Color.white;
    }

    // Food Served 
    private int Calculated_FoodScore()
    {
        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;
        FoodData playerFoodData = playerFoodIcon.currentData;

        int defaultScore = playerFoodData.foodScrObj.price;

        // time left
        defaultScore += _controller.timer.Current_TimeBlock_Amount();

        // state match
        defaultScore += _controller.foodIcon.currentData.Conditions_MatchCount(playerFoodData.conditionDatas);

        // bookmark count
        defaultScore += _controller.mainController.currentVehicle.menu.archiveMenu.bookmarkedFoods.Count;

        return defaultScore;
    }

    private void Collect_Coin()
    {
        Main_Controller.currentGoldCoin += _foodScore;
        _foodScore = 0;

        // hide coin sprite
        _goldCoinSR.color = Color.clear;

        // toss coin to player animation
        Coin_ScrObj goldCoin = _controller.mainController.dataController.coinTypes[0];
        _controller.itemLauncher.Parabola_CoinLaunch(goldCoin, -_controller.detection.player.transform.position);

        // clear data
        _controller.foodIcon.Set_CurrentData(null);
        _controller.foodIcon.Show_Icon();
        _controller.foodIcon.Show_Condition();

        // leave
        NPC_Movement move = _controller.movement;
        SpriteRenderer currentLocation = _controller.mainController.currentLocation.roamArea;

        move.Update_RoamArea(currentLocation);
        move.Leave(Random.Range(roamDelayTime.x, roamDelayTime.y));
    }
}