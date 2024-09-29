using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryNPC : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;

    [Header("")]
    [SerializeField] private SpriteRenderer _foodBox;

    [Header("")]
    [SerializeField] private SubLocation _currentSubLocation;

    [Header("")]
    [SerializeField] private FoodStock[] _foodStocks;
    [SerializeField] private Transform _storagePoint;

    [Header("")]
    [Range(0, 1)][SerializeField] private float _actionSpeed;

    [Range(0, 24)][SerializeField] private int _refillCoolTime;
    private int _refillTimeCount;

    [Range(0, 10)][SerializeField] private int _discountStockCount;

    [Range(0, 24)][SerializeField] private int _discountCoolTime;
    private int _discountTimeCount;

    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Start()
    {
        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // food box toggle
        _foodBox.color = Color.clear;

        // free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent += FoodBox_DirectionUpdate;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent += Cancel_Action;
        interact.InteractEvent += Interact_FacePlayer;
    }

    private void OnDestroy()
    {
        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent -= Cancel_Action;
        interact.InteractEvent -= Interact_FacePlayer;
    }


    // Food Box Control
    private void FoodBox_DirectionUpdate()
    {
        NPC_Movement move = _npcController.movement;

        // left
        if (move.Move_Direction() == -1) _foodBox.flipX = true;

        // right
        else _foodBox.flipX = false;
    }


    // Food Stocks Control
    private bool RefillFoodStock_Available()
    {
        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].foodIcon.currentData.currentAmount < _foodStocks[i].maxAmount) return true;
        }
        return false;
    }

    private List<FoodStock> Discount_FoodStocks()
    {
        List<FoodStock> discountStocks = new();

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].data.isDiscount == false) continue;
            discountStocks.Add(_foodStocks[i]);
        }

        return discountStocks;
    }

    private void SortFoodStocks_byDistance()
    {
        System.Array.Sort(_foodStocks, (a, b) =>
        {
            float distanceA = Vector3.Distance(a.transform.position, transform.position);
            float distanceB = Vector3.Distance(b.transform.position, transform.position);
            return distanceA.CompareTo(distanceB);
        });
    }
    private void SortFoodStocks_byDistance(List<FoodStock> targetFoodStocks)
    {
        targetFoodStocks.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(transform.position, a.transform.position);
            float distanceB = Vector3.Distance(transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });
    }


    // Actions
    private void Interact_FacePlayer()
    {
        // facing to player direction
        _npcController.basicAnim.Flip_Sprite(_npcController.interactable.detection.player.gameObject);

        NPC_Movement movement = _npcController.movement;

        movement.Stop_FreeRoam();
        movement.Free_Roam(_currentSubLocation.roamArea, Random.Range(movement.intervalTimeRange.x, movement.intervalTimeRange.y));
    }

    private void Cancel_Action()
    {
        if (_actionCoroutine == null) return;

        StopCoroutine(_actionCoroutine);
        _actionCoroutine = null;

        // food box transparency
        _foodBox.color = Color.clear;

        // return to free roam
        NPC_Movement move = _npcController.movement;
        move.Free_Roam(_currentSubLocation.roamArea, move.Random_IntervalTime());
    }


    private void Refill_Amount()
    {
        if (_actionCoroutine != null) return;
        if (RefillFoodStock_Available() == false) return;

        // cool time
        if (_refillTimeCount < _refillCoolTime)
        {
            _refillTimeCount++;
            return;
        }
        _refillTimeCount = 0;

        // food box transparency
        _foodBox.color = Color.white;

        _actionCoroutine = StartCoroutine(Refill_Amount_Coroutine());
    }
    private IEnumerator Refill_Amount_Coroutine()
    {
        NPC_Movement movement = _npcController.movement;

        SortFoodStocks_byDistance();

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            int stockAmount = _foodStocks[i].foodIcon.currentData.currentAmount;

            // if food stock current amount is not max amount
            if (stockAmount >= _foodStocks[i].maxAmount) continue;

            // move to food stock
            movement.Stop_FreeRoam();
            movement.Assign_TargetPosition(_foodStocks[i].transform.position);

            // wait until arrival
            while (movement.At_TargetPosition() == false) yield return null;

            // gradually amount increase
            int refillAmount = _foodStocks[i].maxAmount - stockAmount;

            for (int j = 0; j < refillAmount; j++)
            {
                _foodStocks[i].Update_Amount(1);
                yield return new WaitForSeconds(_actionSpeed);
            }
        }

        // food box transparency
        _foodBox.color = Color.clear;

        // return to free roam
        movement.Free_Roam(_currentSubLocation.roamArea, _actionSpeed);
    }
}
