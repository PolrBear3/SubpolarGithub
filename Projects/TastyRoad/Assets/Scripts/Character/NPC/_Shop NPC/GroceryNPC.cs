using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;

    [Header("")]
    [SerializeField] private GameObject _questBarObject;
    [SerializeField] private AmountBar _questBar;

    [Header("")]
    [SerializeField] private SpriteRenderer _foodBox;

    [Header("")]
    [SerializeField] private SubLocation _currentSubLocation;

    [Header("")]
    [SerializeField] private FoodStock[] _foodStocks;
    [SerializeField] private PlaceableStock[] _placeableStocks;


    [Header("")]
    [SerializeField][Range(0, 1)] private float _actionSpeed;

    [Header("")]
    [SerializeField] private Food_ScrObj[] _startingBundles;
    private List<FoodData> _archivedBundles = new();
    private Food_ScrObj _questFood;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _restockCount;
    private int _currentRestockCount;

    [SerializeField][Range(0, 100)] private int _restockBonus;

    [Header("")]
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

        _questBar.Toggle_BarColor(true);
        Update_QuestBar();

        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // food box toggle
        _foodBox.color = Color.clear;

        // free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent += FoodBox_DirectionUpdate;

        WorldMap_Controller.NewLocation_Event += Restock_Full;

        GlobalTime_Controller.TimeTik_Update += Set_QuestFood;
        GlobalTime_Controller.TimeTik_Update += Set_Discount;
        GlobalTime_Controller.TimeTik_Update += Restock;
        GlobalTime_Controller.TimeTik_Update += Collect_FoodBundles;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent += Cancel_Action;
        interact.InteractEvent += Interact_FacePlayer;

        interact.InteractEvent += Update_QuestBar;
        interact.UnInteractEvent += Update_QuestBar;

        interact.OnAction1Event += Toggle_RestockMode;
        interact.OnAction2Event += Complete_Quest;
    }

    private void OnDestroy()
    {
        // action subscription
        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        WorldMap_Controller.NewLocation_Event -= Restock_Full;

        GlobalTime_Controller.TimeTik_Update -= Set_QuestFood;
        GlobalTime_Controller.TimeTik_Update -= Set_Discount;
        GlobalTime_Controller.TimeTik_Update -= Restock;
        GlobalTime_Controller.TimeTik_Update -= Collect_FoodBundles;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.InteractEvent -= Cancel_Action;
        interact.InteractEvent -= Interact_FacePlayer;

        interact.InteractEvent -= Update_QuestBar;
        interact.UnInteractEvent -= Update_QuestBar;

        interact.OnAction1Event -= Toggle_RestockMode;
        interact.OnAction2Event -= Complete_Quest;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GroceryNPC/_archivedBundles", _archivedBundles);
        ES3.Save("GroceryNPC/_isNewRestock", _isNewRestock);

        ES3.Save("GroceryNPC/_currentRestockCount", _currentRestockCount);
        ES3.Save("GroceryNPC/_currentQuestCount", _currentQuestCount);

        Save_CurrentFoodStocks();
        Save_PlaceableStocks();
    }

    public void Load_Data()
    {
        // new
        if (ES3.KeyExists("GroceryNPC/_archivedBundles") == false)
        {
            foreach (Food_ScrObj food in _startingBundles)
            {
                Archive_toBundles(food);
            }

            return;
        }

        // load
        _archivedBundles = ES3.Load("GroceryNPC/_archivedBundles", _archivedBundles);
        _isNewRestock = ES3.Load("GroceryNPC/_isNewRestock", _isNewRestock);

        _currentRestockCount = ES3.Load("GroceryNPC/_currentRestockCount", _currentRestockCount);
        _currentQuestCount = ES3.Load("GroceryNPC/_currentQuestCount", _currentQuestCount);

        Load_CurrentFoodStocks();
        Load_PlaceableStocks();
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


    private void Save_PlaceableStocks()
    {
        Dictionary<List<FoodData>, bool> placeableStockData = new();

        foreach (PlaceableStock stock in _placeableStocks)
        {
            placeableStockData.Add(stock.placedFoods, stock.isComplete);
        }

        ES3.Save("GroceryNPC/placeableStockData", placeableStockData);
    }

    private void Load_PlaceableStocks()
    {
        Dictionary<List<FoodData>, bool> placeableStockData = new();
        placeableStockData = new(ES3.Load("GroceryNPC/placeableStockData", placeableStockData));

        List<List<FoodData>> placedDatas = new(placeableStockData.Keys);
        List<bool> completeDatas = new(placeableStockData.Values);

        for (int i = 0; i < placeableStockData.Count; i++)
        {
            if (i > _placeableStocks.Length - 1) return;

            _placeableStocks[i].Load_Data(placedDatas[i], completeDatas[i]);
        }
    }


    // Data Control
    private bool Is_Archived(Food_ScrObj food)
    {
        for (int i = 0; i < _archivedBundles.Count; i++)
        {
            if (_archivedBundles[i].foodScrObj != food) continue;
            return true;
        }
        return false;
    }


    private void Archive_toBundles(Food_ScrObj food)
    {
        if (Is_Archived(food))
        {
            // increase amount of data
            Archived_BundleData(food).Update_Amount(1);
            return;
        }

        FoodData archiveData = new(food);
        _archivedBundles.Add(archiveData);
    }




    private FoodData Archived_BundleData(Food_ScrObj food)
    {
        for (int i = 0; i < _archivedBundles.Count; i++)
        {
            if (_archivedBundles[i].foodScrObj != food) continue;
            return _archivedBundles[i];
        }

        return null;
    }

    private List<Food_ScrObj> ArchivedBundles_Ingredients()
    {
        List<Food_ScrObj> ingredients = new();

        for (int i = 0; i < _archivedBundles.Count; i++)
        {
            // Raw Food
            if (_archivedBundles[i].foodScrObj.ingredients.Count <= 0)
            {
                ingredients.Add(_archivedBundles[i].foodScrObj);

                _archivedBundles.RemoveAt(i);
                continue;
            }

            // Cooked Food
            for (int j = 0; j < _archivedBundles[i].foodScrObj.ingredients.Count; j++)
            {
                ingredients.Add(_archivedBundles[i].foodScrObj.ingredients[j].foodScrObj);
            }

            int currentAmount = _archivedBundles[i].currentAmount;
            int calculatedAmount = currentAmount - 1;

            _archivedBundles[i].Set_Amount(Mathf.Clamp(calculatedAmount, 1, currentAmount));
        }

        return ingredients;
    }


    // Basics
    private void Interact_FacePlayer()
    {
        // facing to player direction
        _npcController.basicAnim.Flip_Sprite(_npcController.interactable.detection.player.gameObject);

        NPC_Movement movement = _npcController.movement;

        movement.Stop_FreeRoam();
        movement.Free_Roam(_currentSubLocation.roamArea, Random.Range(movement.intervalTimeRange.x, movement.intervalTimeRange.y));
    }


    private void Start_Action()
    {
        _npcController.interactable.LockInteract(true);
        _foodBox.color = Color.white;

        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();
    }

    private void Cancel_Action()
    {
        if (_actionCoroutine != null)
        {
            StopCoroutine(_actionCoroutine);
            _actionCoroutine = null;
        }

        _npcController.interactable.LockInteract(false);

        // food box transparency
        _foodBox.color = Color.clear;

        // return to free roam
        NPC_Movement move = _npcController.movement;
        move.Free_Roam(_currentSubLocation.roamArea, move.Random_IntervalTime());
    }


    // Quest
    public void Update_QuestBar()
    {
        Action_Bubble bubble = _npcController.interactable.bubble;
        _questBarObject.SetActive(!bubble.bubbleOn);

        if (bubble.bubbleOn) return;
        _questBar.Load_Custom(_questCount, _currentQuestCount);
    }


    private void Set_QuestFood()
    {
        if (_questFood != null) return;
        if (_currentQuestCount >= _questCount) return;

        // get random cooked food from _archivedCooks
        _questFood = _archivedBundles[Random.Range(0, _archivedBundles.Count)].foodScrObj;

        ActionBubble_Interactable interactable = _npcController.interactable;
        Action_Bubble bubble = interactable.bubble;

        bubble.Set_Bubble(_questFood, _questFood);
        Update_RestockBubble();

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

        Action_Bubble bubble = _npcController.interactable.bubble;
        bubble.Empty_Bubble();
        Update_RestockBubble();
    }


    // Restock
    private void Toggle_RestockMode()
    {
        _isNewRestock = !_isNewRestock;
        Update_RestockBubble();
    }

    private void Update_RestockBubble()
    {
        Action_Bubble bubble = _npcController.interactable.bubble;

        if (_isNewRestock)
        {
            bubble.Set_Bubble(bubble.setSprites[1], null);
            return;
        }

        bubble.Set_Bubble(bubble.setSprites[0], null);
    }


    private void Restock_Full()
    {
        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].stockData.unlocked == false) continue;

            // clear data
            _foodStocks[i].foodIcon.Set_CurrentData(null);

            Food_ScrObj newFood = ArchivedBundles_Ingredients()[Random.Range(0, ArchivedBundles_Ingredients().Count)];

            // set new food and update to full amount
            _foodStocks[i].Set_FoodData(new(newFood));
            _foodStocks[i].Update_Amount(_foodStocks[i].maxAmount - 1);
        }
    }

    public void Restock()
    {
        if (_currentRestockCount < _restockCount)
        {
            _currentRestockCount++;
            return;
        }

        if (_actionCoroutine != null) return;

        _currentRestockCount = 0;
        _actionCoroutine = StartCoroutine(Restock_Coroutine());
    }
    private IEnumerator Restock_Coroutine()
    {
        Start_Action();

        List<FoodStock> stocks = FoodStocks_byDistance();
        NPC_Movement movement = _npcController.movement;

        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].stockData.unlocked == false) continue;

            int currentAmount = stocks[i].Current_Amount();

            // new restock
            if (_isNewRestock)
            {
                movement.Assign_TargetPosition(stocks[i].transform.position);
                while (movement.At_TargetPosition() == false) yield return null;

                Food_ScrObj newFood = ArchivedBundles_Ingredients()[Random.Range(0, ArchivedBundles_Ingredients().Count)];

                stocks[i].Set_FoodData(new(newFood));
                stocks[i].Update_Amount(-(currentAmount + 1));

                yield return new WaitForSeconds(_actionSpeed);
                continue;
            }

            // amount restock
            if (stocks[i].foodIcon.hasFood == false) continue;

            int restockAmount = stocks[i].maxAmount - currentAmount;
            if (restockAmount <= 0) continue;

            movement.Assign_TargetPosition(stocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            for (int j = 0; j < restockAmount; j++)
            {
                stocks[i].Update_Amount(1);
                yield return new WaitForSeconds(_actionSpeed);
            }
        }

        Cancel_Action();
        yield break;
    }


    // Other
    private void Set_Discount()
    {
        if (_actionCoroutine != null) return;
        if (_currentQuestCount < _questCount) return;

        _actionCoroutine = StartCoroutine(Set_Discount_Coroutine());
    }
    private IEnumerator Set_Discount_Coroutine()
    {
        Start_Action();

        List<FoodStock> sortedStocks = new();
        NPC_Movement movement = _npcController.movement;

        // sort stocks according to conditions
        for (int i = 0; i < FoodStocks_byDistance().Count; i++)
        {
            if (FoodStocks_byDistance()[i].stockData.unlocked == false) continue;
            if (FoodStocks_byDistance()[i].foodIcon.hasFood == false) continue;
            if (FoodStocks_byDistance()[i].stockData.isDiscount) continue;

            sortedStocks.Add(FoodStocks_byDistance()[i]);
        }

        // cancel action if there are no available stocks
        if (sortedStocks.Count <= 0)
        {
            Cancel_Action();
            yield break;
        }

        FoodStock randStock = sortedStocks[Random.Range(0, sortedStocks.Count)];

        movement.Assign_TargetPosition(randStock.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        randStock.Toggle_Discount(true);
        _currentQuestCount = 0;

        Cancel_Action();
        yield break;
    }


    private List<PlaceableStock> Complete_Stocks()
    {
        List<PlaceableStock> stocks = new();

        for (int i = 0; i < _placeableStocks.Length; i++)
        {
            if (_placeableStocks[i].isComplete == false) continue;
            stocks.Add(_placeableStocks[i]);
        }

        return stocks;
    }

    public void Collect_FoodBundles()
    {
        if (_actionCoroutine != null) return;
        if (Complete_Stocks().Count <= 0) return;

        _actionCoroutine = StartCoroutine(Collect_FoodBundles_Coroutine());
    }
    private IEnumerator Collect_FoodBundles_Coroutine()
    {
        Start_Action();

        NPC_Movement movement = _npcController.movement;

        for (int i = 0; i < Complete_Stocks().Count; i++)
        {
            movement.Assign_TargetPosition(Complete_Stocks()[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            List<FoodData> placedDatas = Complete_Stocks()[i].placedFoods;

            for (int j = 0; j < placedDatas.Count; j++)
            {
                Archive_toBundles(placedDatas[j].foodScrObj);
            }

            Complete_Stocks()[i].Reset_Data();

            _currentRestockCount += _restockBonus;
        }

        Cancel_Action();
        yield break;
    }
}