using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;

    [Header("")]
    [SerializeField] private SpriteRenderer _foodBox;

    [Header("")]
    [SerializeField] private SubLocation _currentSubLocation;

    [Header("")]
    [SerializeField] private FoodStock[] _foodStocks;


    [Header("")]
    [SerializeField][Range(0, 1)] private float _actionSpeed;

    [SerializeField] private Food_ScrObj[] _startingArchive;
    private List<FoodData> _archivedCooks = new();
    private Food_ScrObj _questFood;

    [SerializeField][Range(0, 100)] private int _questCount;
    private int _currentQuestCount;

    [SerializeField][Range(0, 100)] private int _refreshCount;
    private int _currentRefreshCount;


    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

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

        GlobalTime_Controller.TimeTik_Update += Set_QuestFood;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent += Cancel_Action;
        interact.InteractEvent += Interact_FacePlayer;

        interact.Action1Event += Complete_Quest;
    }

    private void OnDestroy()
    {
        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        GlobalTime_Controller.TimeTik_Update -= Set_QuestFood;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent -= Cancel_Action;
        interact.InteractEvent -= Interact_FacePlayer;

        interact.Action1Event -= Complete_Quest;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GroceryNPC/_archivedCooks", _archivedCooks);
    }

    public void Load_Data()
    {
        // new
        if (ES3.KeyExists("GroceryNPC/_archivedCooks") == false)
        {
            foreach (Food_ScrObj food in _startingArchive)
            {
                Archive_toCooks(food);
            }

            return;
        }

        // load
        _archivedCooks = ES3.Load("GroceryNPC/_archivedCooks", _archivedCooks);
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


    // Data Control
    private bool Is_Archived(Food_ScrObj food)
    {
        for (int i = 0; i < _archivedCooks.Count; i++)
        {
            if (_archivedCooks[i].foodScrObj != food) continue;
            return true;
        }
        return false;
    }

    private void Archive_toCooks(Food_ScrObj food)
    {
        if (Is_Archived(food)) return;

        FoodData archiveData = new(food);
        _archivedCooks.Add(archiveData);
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


    private void Set_QuestFood()
    {
        // get random cooked food from _archivedCooks
        _questFood = _archivedCooks[Random.Range(0, _archivedCooks.Count)].foodScrObj;

        // action bubble update
        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.bubble.Update_Bubble(_questFood, null);
        interactable.UnInteract();
    }

    private void Complete_Quest()
    {
        if (_questFood == null) return;

        ActionBubble_Interactable interactable = _npcController.interactable;
        FoodData_Controller playerIcon = interactable.detection.player.foodIcon;

        if (playerIcon.Is_SameFood(_questFood) == false) return;

        // quest count update
        _currentQuestCount++;
        _currentQuestCount = Mathf.Clamp(_currentQuestCount, 0, _questCount);

        // remove player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();

        // refresh _questFood
        _questFood = null;
        interactable.bubble.Empty_Bubble();
    }


    private void Set_Discount()
    {
        if (_currentQuestCount < _questCount) return;

        _currentQuestCount = 0;
        _actionCoroutine = StartCoroutine(Set_Discount_Coroutine());
    }
    private IEnumerator Set_Discount_Coroutine()
    {
        Cancel_Action();
        yield break;
    }


    private void Collect_FoodBundles()
    {
        _actionCoroutine = StartCoroutine(Collect_FoodBundles_Coroutine());
    }
    private IEnumerator Collect_FoodBundles_Coroutine()
    {
        Cancel_Action();
        yield break;
    }


    private void Clear_RottenStocks()
    {
        _actionCoroutine = StartCoroutine(Clear_RottenStocks_Coroutine());
    }
    private IEnumerator Clear_RottenStocks_Coroutine()
    {
        Cancel_Action();
        yield break;
    }
}
