using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Interaction : MonoBehaviour, IInteractable
{
    private NPC_Controller _controller;

    [SerializeField] private GameObject _stateBoxes;
    [SerializeField] private SpriteRenderer _eatAnimationSR;
    [SerializeField] private Sprite _coinSprite;

    private bool _foodOrderServed;
    private int _foodScore;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_controller.detection.Has_Player() == false)
        {
            _controller.actionBubble.Toggle_Off();
            StateBox_Toggle();
        }
    }

    // IInteractable
    public void Interact()
    {
        Interact_FacePlayer();

        Assign_FoodOrder();

        ActionBubble_Toggle();
        StateBox_Toggle();

        Collect_Coin();
    }

    // InputSystem
    private void OnAction1()
    {
        Serve_FoodOrder();

        ActionBubble_Toggle();
        StateBox_Toggle();
    }

    // Stop and Face Player if Interacted
    private void Interact_FacePlayer()
    {
        if (_controller.actionBubble.bubbleOn == true) return;

        _controller.movement.Stop_FreeRoam();
        _controller.movement.Free_Roam(4f);

        _controller.animationController.Flip_Sprite(_controller.detection.player.gameObject);
    }

    // Update Action Bubble
    private void ActionBubble_Toggle()
    {
        if (_foodOrderServed == true) return;

        _controller.actionBubble.Update_Bubble(_controller.foodIcon.currentFoodData.foodScrObj.sprite, null);
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

    // Set Random Food Order and State
    private void Assign_FoodOrder()
    {
        if (_foodOrderServed == true) return;

        Data_Controller data = _controller.mainController.dataController;
        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false)
        {
            // set random food
            foodIcon.Assign_Food(data.CookedFood(Random.Range(0, data.cookedFoods.Count)));

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

        FoodData_Controller playerFoodIcon = _controller.detection.player.foodIcon;
        FoodData playerFoodData = playerFoodIcon.currentFoodData;

        _controller.actionBubble.Toggle_Off();

        // check if order food is correct
        if (playerFoodData.foodScrObj != foodIcon.currentFoodData.foodScrObj) return;

        // save food score
        _foodScore = Calculated_FoodScore();

        playerFoodIcon.Clear_Food();
        playerFoodIcon.Clear_State();

        _foodOrderServed = true;

        // food eat animation
        Eat_Animation();
    }

    // Eat Food Animation after Food Order is Served
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
        yield return new WaitForSeconds(1f);

        // eat food sprite
        _eatAnimationSR.sprite = servedFood.eatSprite;

        // wait
        yield return new WaitForSeconds(1f);

        // no sprite
        _eatAnimationSR.sprite = null;

        // wait
        yield return new WaitForSeconds(0.5f);

        // activate free roam
        movement.Free_Roam(3f);
        // turn on collider
        detection.BoxCollider_Toggle(true);
        // coin sprite
        _eatAnimationSR.sprite = _coinSprite;
    }
    private void Eat_Animation()
    {
        StartCoroutine(Eat_Animation_Coroutine());
    }

    // Get Served Food Order Score
    private int Calculated_FoodScore()
    {
        int foodScore = 1;
        FoodData playerFoodData = _controller.detection.player.foodIcon.currentFoodData;

        if (_controller.foodIcon.Same_StateData(playerFoodData.stateData)) foodScore++;

        return foodScore;
    }

    // Interaction Finish
    private void Collect_Coin()
    {
        if (_foodOrderServed == false) return;

        _controller.mainController.currentCoin += _foodScore;

        _foodOrderServed = false;
        _foodScore = 0;

        _eatAnimationSR.sprite = null;
        _controller.foodIcon.Clear_Food();

        Leave_Location(3f);
    }

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
}
