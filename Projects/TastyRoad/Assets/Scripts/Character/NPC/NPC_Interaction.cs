using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Interaction : MonoBehaviour, IInteractable
{
    private NPC_Controller _controller;

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

        if (Main_Controller.orderOpen) _controller.timer.Toggle_Transparency(false);

        UnInteract();
    }



    // IInteractable
    public void Interact()
    {
        Interact_FacePlayer();

        if (_controller.foodIcon.hasFood && _controller.timer.timeRunning == false)
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

        _controller.PlayerInput_Toggle(true);

        _controller.timer.Toggle_Transparency(!_controller.actionBubble.bubbleOn);

        ActionBubble_Toggle();
        StateBox_Toggle();

        _controller.Action1 += Serve_FoodOrder;
    }

    public void UnInteract()
    {
        _controller.PlayerInput_Toggle(false);

        _controller.actionBubble.Toggle(false);
        StateBox_Toggle();

        _controller.timer.Toggle_Transparency(false);

        _controller.Action1 -= Serve_FoodOrder;
    }



    // Stop and Face Player if Interacted
    private void Interact_FacePlayer()
    {
        if (_controller.actionBubble.bubbleOn == true) return;

        _controller.animationController.Flip_Sprite(_controller.detection.player.gameObject);

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

        _controller.actionBubble.Update_Bubble(foodIcon.currentFoodData.foodScrObj, null);
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
    /// Start a time limit for food order
    /// </summary>
    public void Start_TimeLimit()
    {
        // start the time
        Clock_Timer timer = _controller.timer;

        if (timer.timeRunning == true) return;

        // set the time limit according to npc inpatient level
        int extraTime = (int)_controller.characterData.inPatienceLevel;
        int setTime = _defaultTimeLimit + extraTime;

        timer.Set_Time(setTime);
        timer.Run_Time();

        TimeLimit_Over();
    }

    /// <summary>
    /// Checks if time is over and initiates player penalty
    /// </summary>
    private void TimeLimit_Over()
    {
        if (_timeLimitCoroutine != null) _timeLimitCoroutine = null;

        _timeLimitCoroutine = StartCoroutine(TimeLimit_Over_Coroutine());
    }
    private IEnumerator TimeLimit_Over_Coroutine()
    {
        Clock_Timer timer = _controller.timer;

        while (timer.timeRunning == true) yield return null;

        UnInteract();

        // add unsatisfied icon or something ?

        // clear food
        FoodData_Controller icon = _controller.foodIcon;
        icon.Clear_Food();
        icon.Clear_State();

        // hunger level to 0
        Character_Data data = _controller.characterData;
        data.Update_Hunger(-data.hungerLevel);

        // return roam area to current location
        SpriteRenderer currentLocation = _controller.mainController.currentLocation.roamArea;
        NPC_Movement move = _controller.movement;
        move.Stop_FreeRoam();
        move.Free_Roam(currentLocation, 0f);
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

            foodIcon.Assign_Food(randomFood);

            // set random state
            List<StateBox_Sprite> allStates = foodIcon.stateBoxController.stateBoxSprites;

            for (int i = 0; i < allStates.Count; i++)
            {
                bool setState = Random.value > 0.5f;

                if (setState == false) continue;

                foodIcon.Update_State(allStates[i].type, Random.Range(1, allStates[i].boxSprites.Count + 1));
            }
        }
    }

    // Serve Food Order
    private void Serve_FoodOrder()
    {
        if (_foodOrderServed == true) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false) return;
        if (_controller.detection.player == null) return;

        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;
        FoodData playerFoodData = playerFoodIcon.currentFoodData;

        _controller.actionBubble.Toggle(false);

        // check if order food is correct
        if (playerFoodData.foodScrObj != foodIcon.currentFoodData.foodScrObj)
        {
            UnInteract();
            return;
        }

        // stop time limit
        StopCoroutine(_timeLimitCoroutine);
        _controller.timer.Stop_Time();

        // save food score
        _foodScore = Calculated_FoodScore();

        playerFoodIcon.Clear_Food();
        playerFoodIcon.Clear_State();

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
        Food_ScrObj servedFood = _controller.foodIcon.currentFoodData.foodScrObj;

        // stop free roam
        movement.Stop_FreeRoam();
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
        movement.Free_Roam(movement.currentRoamArea, Random.Range(roamDelayTime.x, roamDelayTime.y));

        // turn on collider
        detection.BoxCollider_Toggle(true);

        // activate coin sprite
        _goldCoinSR.color = Color.white;
    }

    // Food Served 
    private int Calculated_FoodScore()
    {
        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;
        FoodData playerFoodData = playerFoodIcon.currentFoodData;

        int defaultScore = playerFoodData.foodScrObj.price;

        // time left
        defaultScore += _controller.timer.Current_TimeBlock_Amount();

        // state match
        defaultScore += _controller.foodIcon.StateData_MatchCount(playerFoodData.stateData);

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

        // update data
        _controller.foodIcon.Clear_Food();

        // leave
        NPC_Movement move = _controller.movement;
        SpriteRenderer currentLocation = _controller.mainController.currentLocation.roamArea;

        move.Update_RoamArea(currentLocation);
        move.Leave(Random.Range(roamDelayTime.x, roamDelayTime.y));
    }

    /*
    // Leave Area
    private IEnumerator Leave_Location_Coroutine(float delayTime)
    {
        // get components
        Detection_Controller detection = _controller.detection;
        NPC_Movement movement = _controller.movement;

        // stop free roam
        movement.Stop_FreeRoam();

        // collider toggle off
        detection.BoxCollider_Toggle(false);

        // wait until time pass
        yield return new WaitForSeconds(delayTime);

        // set target position to outer camera
        float randDirection = Random.Range(-1, 1);
        float posX;

        if (randDirection >= 0)
        {
            randDirection = 1f;
            posX = 2f;
        }
        else
        {
            randDirection = -1f;
            posX = -2f;
        }

        Vector2 targetPos = _controller.mainController.OuterCamera_Position(randDirection, 0f, posX, -3f);
        movement.Assign_TargetPosition(targetPos);

        // if npc reaches target position
        while (movement.At_TargetPosition() == false)
        {
            yield return null;
        }

        // spawn another new customer
        _controller.mainController.currentLocation.Spawn_NPCs(1);

        // untrack this npc
        _controller.mainController.UnTrack_CurrentCharacter(gameObject);

        // remove this npc
        Destroy(gameObject);
    }
    private void Leave_Location(float delaytime)
    {
        StartCoroutine(Leave_Location_Coroutine(delaytime));
    }
    */
}