using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    [SerializeField][Range(0, 100)] private int _restockCount;
    private int _currentRestockCount;

    [SerializeField][Range(0, 100)] private int _questCount;
    private int _currentQuestCount;


    private bool _isNewRestock;

    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        Update_RestockBubble();

        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // food box toggle
        _foodBox.color = Color.clear;

        // free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent += FoodBox_DirectionUpdate;

        GlobalTime_Controller.TimeTik_Update += Set_QuestFood;
        GlobalTime_Controller.TimeTik_Update += Set_Discount;
        GlobalTime_Controller.TimeTik_Update += Restock;
        GlobalTime_Controller.TimeTik_Update += Collect_FoodBundles;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent += Cancel_Action;
        interact.InteractEvent += Interact_FacePlayer;

        interact.Action1Event += Toggle_RestockMode;
        interact.Action2Event += Complete_Quest;
    }

    private void OnDestroy()
    {
        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        GlobalTime_Controller.TimeTik_Update -= Set_QuestFood;
        GlobalTime_Controller.TimeTik_Update -= Set_Discount;
        GlobalTime_Controller.TimeTik_Update -= Restock;
        GlobalTime_Controller.TimeTik_Update -= Collect_FoodBundles;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent -= Cancel_Action;
        interact.InteractEvent -= Interact_FacePlayer;

        interact.Action1Event -= Toggle_RestockMode;
        interact.Action2Event -= Complete_Quest;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GroceryNPC/_archivedCooks", _archivedCooks);

        Save_CurrentFoodStocks();
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

        Load_CurrentFoodStocks();
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
    private void Save_CurrentFoodStocks()
    {
        Dictionary<StockData, FoodData> foodStockDatas = new();

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            foodStockDatas.Add(_foodStocks[i].stockData, _foodStocks[i].foodIcon.currentData);
        }

        ES3.Save("GroceryNPC/foodStockDatas", foodStockDatas);
    }

    private void Load_CurrentFoodStocks()
    {
        Dictionary<StockData, FoodData> foodStockDatas = new();
        foodStockDatas = new(ES3.Load("GroceryNPC/foodStockDatas", foodStockDatas));

        List<StockData> stockDatas = new(foodStockDatas.Keys);
        List<FoodData> foodDatas = new(foodStockDatas.Values);

        for (int i = 0; i < foodStockDatas.Count; i++)
        {
            if (i > _foodStocks.Length - 1) return;

            _foodStocks[i].Set_StockData(stockDatas[i]);
            _foodStocks[i].Set_FoodData(foodDatas[i]);
        }
    }


    private bool Restock_Available()
    {
        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].foodIcon.currentData.currentAmount < _foodStocks[i].maxAmount) return true;
        }
        return false;
    }


    private List<FoodStock> FoodStocks_byDistance()
    {
        List<FoodStock> foodStocks = new();

        foreach (FoodStock stock in _foodStocks)
        {
            foodStocks.Add(stock);
        }

        foodStocks.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(transform.position, a.transform.position);
            float distanceB = Vector3.Distance(transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        return foodStocks;
    }

    private List<FoodStock> Discount_FoodStocks()
    {
        List<FoodStock> discountStocks = new();

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].stockData.isDiscount == false) continue;
            discountStocks.Add(_foodStocks[i]);
        }

        return discountStocks;
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


    private List<Food_ScrObj> ArchivedCooks_Ingredients()
    {
        List<Food_ScrObj> ingredients = new();

        for (int i = 0; i < _archivedCooks.Count; i++)
        {
            for (int j = 0; j < _archivedCooks[i].foodScrObj.ingredients.Count; j++)
            {
                Food_ScrObj foodToAdd = _archivedCooks[i].foodScrObj.ingredients[j].foodScrObj;
                if (ingredients.Contains(foodToAdd)) continue;

                ingredients.Add(foodToAdd);
            }
        }

        return ingredients;
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
        if (_questFood != null) return;

        // get random cooked food from _archivedCooks
        _questFood = _archivedCooks[Random.Range(0, _archivedCooks.Count)].foodScrObj;

        Action_Bubble bubble = _npcController.interactable.bubble;
        bubble.Set_Bubble(_questFood, _questFood);
        Update_RestockBubble();
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

        Action_Bubble bubble = _npcController.interactable.bubble;
        bubble.Empty_Bubble();
        Update_RestockBubble();
    }


    private void Update_RestockBubble()
    {
        Action_Bubble bubble = _npcController.interactable.bubble;

        if (_isNewRestock)
        {
            bubble.Set_Bubble(bubble.setSprites[1], bubble.rightIcon.sprite);
            return;
        }

        bubble.Set_Bubble(bubble.setSprites[2], bubble.rightIcon.sprite);
    }

    private void Toggle_RestockMode()
    {
        _isNewRestock = !_isNewRestock;
        Update_RestockBubble();
    }

    public void Restock()
    {
        /*
        if (_currentRestockCount < _restockCount)
        {
            _currentRestockCount++;
            return;
        }
        */

        if (_actionCoroutine != null) return;

        _currentRestockCount = 0;
        _actionCoroutine = StartCoroutine(Restock_Coroutine());
    }
    private IEnumerator Restock_Coroutine()
    {
        for (int i = 0; i < FoodStocks_byDistance().Count; i++)
        {
            if (FoodStocks_byDistance()[i].stockData.unlocked == false) continue;

            int maxAmount = FoodStocks_byDistance()[i].maxAmount;
            int currentAmount = FoodStocks_byDistance()[i].Current_Amount();

            // move to target food stock
            NPC_Movement movement = _npcController.movement;
            movement.Stop_FreeRoam();

            movement.Assign_TargetPosition(FoodStocks_byDistance()[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // new restock
            if (_isNewRestock)
            {
                Food_ScrObj newFood = ArchivedCooks_Ingredients()[Random.Range(0, ArchivedCooks_Ingredients().Count)];

                FoodStocks_byDistance()[i].Set_FoodData(new(newFood));
                FoodStocks_byDistance()[i].Update_Amount(-currentAmount);

                continue;
            }

            // amount restock
            if (FoodStocks_byDistance()[i].foodIcon.hasFood == false) continue;
            if (currentAmount >= maxAmount) continue;

            int restockAmount = maxAmount - currentAmount;

            for (int j = 0; j < restockAmount; j++)
            {
                FoodStocks_byDistance()[i].Update_Amount(1);
                yield return new WaitForSeconds(_actionSpeed);
            }
        }

        Cancel_Action();
        yield break;
    }


    private void Set_Discount()
    {
        if (_actionCoroutine != null) return;
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
        if (_actionCoroutine != null) return;

        _actionCoroutine = StartCoroutine(Collect_FoodBundles_Coroutine());
    }
    private IEnumerator Collect_FoodBundles_Coroutine()
    {
        Cancel_Action();
        yield break;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(GroceryNPC))]
public class EditorButton : Editor
{
    //
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GroceryNPC npc = (GroceryNPC)target;

        GUILayout.Space(60);

        if (GUILayout.Button("Activate"))
        {
            npc.Restock();
        }
    }
}
#endif