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
    [Range(0, 1)] [SerializeField] private float _actionSpeed;

    [Range(0, 10)] [SerializeField] private int _discountStockCount;

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

        GlobalTime_Controller.TimeTik_Update += Refill_Amount;
        GlobalTime_Controller.TimeTik_Update += Set_Discount;
    }

    private void OnDestroy()
    {
        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        GlobalTime_Controller.TimeTik_Update -= Refill_Amount;
        GlobalTime_Controller.TimeTik_Update -= Set_Discount;
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
    private List<FoodStock> Discount_FoodStocks()
    {
        List<FoodStock> discountStocks = new();

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].isDiscount == false) continue;
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
            // if food stock current amount is not max amount
            if (_foodStocks[i].currentAmount >= _foodStocks[i].maxAmount) continue;

            // move to food stock
            movement.Stop_FreeRoam();
            movement.Assign_TargetPosition(_foodStocks[i].transform.position);

            // wait until arrival
            while (movement.At_TargetPosition() == false) yield return null;

            // gradually amount increase
            int refillAmount = _foodStocks[i].maxAmount - _foodStocks[i].currentAmount;

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

    private void Restock()
    {
        if (_actionCoroutine != null) return;

        // food box transparency
        _foodBox.color = Color.white;

        _actionCoroutine = StartCoroutine(Restock_Coroutine());
    }
    private IEnumerator Restock_Coroutine()
    {
        NPC_Movement movement = _npcController.movement;

        SortFoodStocks_byDistance();

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            // if currently interacting
            if (_foodStocks[i].interactable.bubble.bubbleOn) continue;

            // 50% activation
            if (Random.value < 0.5f) continue;

            // action bubble interaction control 
            _foodStocks[i].interactable.LockInteract(true);
            _foodStocks[i].interactable.LockUnInteract(true);

            // move to food stock
            movement.Stop_FreeRoam();
            movement.Assign_TargetPosition(_foodStocks[i].transform.position);

            // wait until arrival
            while (movement.At_TargetPosition() == false) yield return null;

            // gradually amount decrease
            int deceraseAmount = _foodStocks[i].currentAmount;

            for (int j = 0; j < deceraseAmount; j++)
            {
                _foodStocks[i].Update_Amount(-1);
                yield return new WaitForSeconds(_actionSpeed);
            }

            _foodStocks[i].Update_Data();

            yield return new WaitForSeconds(_actionSpeed);

            // action bubble interaction control
            _foodStocks[i].interactable.LockInteract(false);
            _foodStocks[i].interactable.LockUnInteract(false);

            _foodStocks[i].interactable.UnInteract();
        }

        // food box transparency
        _foodBox.color = Color.clear;

        // return to free roam
        movement.Free_Roam(_currentSubLocation.roamArea, _actionSpeed);
    }


    private void Set_Discount()
    {
        if (_actionCoroutine != null) return;
        if (_discountStockCount <= 0) return;

        // check if there is a non discount food stock
        if (Discount_FoodStocks().Count >= _foodStocks.Length) return;

        _actionCoroutine = StartCoroutine(Set_Discount_Coroutine());
    }
    private IEnumerator Set_Discount_Coroutine()
    {
        List<FoodStock> currentStocks = new();

        foreach (var stock in _foodStocks)
        {
            currentStocks.Add(stock);
        }

        List<FoodStock> targetStocks = new();

        do
        {
            // get random food stocks
            FoodStock targetStock = currentStocks[Random.Range(0, currentStocks.Count)];

            if (targetStock.isDiscount == true)
            {
                currentStocks.Remove(targetStock);
            }
            else
            {
                targetStocks.Add(targetStock);
            }
        }
        while (targetStocks.Count < _discountStockCount);

        SortFoodStocks_byDistance(targetStocks);

        NPC_Movement movement = _npcController.movement;
        List<FoodStock> previousStocks = Discount_FoodStocks();

        // set target stocks to discount
        for (int i = 0; i < targetStocks.Count; i++)
        {
            // if currently interacting
            if (targetStocks[i].interactable.bubble.bubbleOn) continue;

            // move to food stock
            movement.Stop_FreeRoam();
            movement.Assign_TargetPosition(targetStocks[i].transform.position);

            // wait until arrival
            while (movement.At_TargetPosition() == false) yield return null;

            targetStocks[i].Update_Discount(true);

            // remove discount from previous discount food stock
            if (previousStocks.Count <= 0) continue;

            int randIndex = Random.Range(0, previousStocks.Count);

            previousStocks[randIndex].Update_Discount(false);
            previousStocks.RemoveAt(randIndex);
        }

        // return to free roam
        movement.Free_Roam(_currentSubLocation.roamArea, _actionSpeed);

        // reset action
        _actionCoroutine = null;
        yield break;
    }
}
