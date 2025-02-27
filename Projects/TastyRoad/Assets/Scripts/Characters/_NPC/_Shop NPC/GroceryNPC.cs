using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;

    [Header("")]
    [SerializeField] private Clock_Timer _actionTimer;


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

    [Header("")]
    [SerializeField][Range(0, 100)] private int _questCoolTime;
    [SerializeField][Range(0, 100)] private int _questCount;


    private List<FoodData> _archivedBundles = new();
    private List<FoodData> _unlockDatas = new();


    private int _currentQuestCool;
    private int _currentQuestCount;

    private FoodData _questFood;
    private FoodData _previousQuestFood;


    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        _questBar.Toggle_BarColor(true);
        Toggle_QuestBar();

        // untrack
        Main_Controller.instance.UnTrack_CurrentCharacter(gameObject);

        // food box toggle
        _foodBox.color = Color.clear;

        // free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        // action subscription
        WorldMap_Controller.OnNewLocation += Restock_Instant;

        GlobalTime_Controller.instance.OnDayTime += Restock;
        GlobalTime_Controller.instance.OnTimeTik += Restock_Unlocks;

        GlobalTime_Controller.instance.OnTimeTik += Collect_FoodBundles;
        GlobalTime_Controller.instance.OnTimeTik += Set_QuestFood;

        _npcController.movement.TargetPosition_UpdateEvent += FoodBox_DirectionUpdate;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.OnIInteract += Cancel_Action;
        interact.OnIInteract += Interact_FacePlayer;

        interact.detection.EnterEvent += Toggle_QuestBar;
        interact.OnIInteract += Toggle_QuestBar;
        interact.OnUnIInteract += Toggle_QuestBar;

        interact.OnAction1Input += DialogUpdate_UnlockCount;
        interact.OnAction2Input += Complete_Quest;
    }

    private void OnDestroy()
    {
        // action subscription

        WorldMap_Controller.OnNewLocation -= Restock_Instant;

        GlobalTime_Controller.instance.OnDayTime -= Restock;
        GlobalTime_Controller.instance.OnTimeTik -= Restock_Unlocks;

        GlobalTime_Controller.instance.OnTimeTik -= Collect_FoodBundles;
        GlobalTime_Controller.instance.OnTimeTik -= Set_QuestFood;

        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        ActionBubble_Interactable interact = _npcController.interactable;

        interact.OnIInteract -= Cancel_Action;
        interact.OnIInteract -= Interact_FacePlayer;

        interact.detection.EnterEvent -= Toggle_QuestBar;
        interact.OnIInteract -= Toggle_QuestBar;
        interact.OnUnIInteract -= Toggle_QuestBar;

        interact.OnAction1Input -= DialogUpdate_UnlockCount;
        interact.OnAction2Input -= Complete_Quest;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GroceryNPC/_archivedBundles", _archivedBundles);
        ES3.Save("GroceryNPC/_unlockDatas", _unlockDatas);

        ES3.Save("GroceryNPC/_currentQuestCool", _currentQuestCool);
        ES3.Save("GroceryNPC/_currentQuestCount", _currentQuestCount);

        ES3.Save("GroceryNPC/_questFood", _questFood);
        ES3.Save("GroceryNPC/_previousQuestFood", _previousQuestFood);

        Save_CurrentFoodStocks();
        Save_PlaceableStocks();
    }

    public void Load_Data()
    {
        // new game
        if (ES3.KeyExists("GroceryNPC/_archivedBundles") == false)
        {
            foreach (Food_ScrObj food in _startingBundles)
            {
                Archive_toBundles(food);

                if (food.ingredients.Count <= 0) continue;

                // unlock only cooked foods from starting bundles
                FoodData unlockData = new(food);
                unlockData.Set_Amount(0);

                _unlockDatas.Add(unlockData);
            }

            return;
        }

        // load
        _archivedBundles = ES3.Load("GroceryNPC/_archivedBundles", _archivedBundles);
        _unlockDatas = ES3.Load("GroceryNPC/_unlockDatas", _unlockDatas);

        _currentQuestCool = ES3.Load("GroceryNPC/_currentQuestCool", _currentQuestCool);
        _currentQuestCount = ES3.Load("GroceryNPC/_currentQuestCount", _currentQuestCount);

        _questFood = new(ES3.Load("GroceryNPC/_questFood", _questFood));
        _previousQuestFood = new(ES3.Load("GroceryNPC/_previousQuestFood", _previousQuestFood));

        Set_QuestFood(_questFood.foodScrObj);

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
            // check if not enough food stocks are available to load
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
            placeableStockData.Add(stock.foodIcon.AllDatas(), stock.isComplete);
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
    private bool Is_StartingBundle(Food_ScrObj food)
    {
        for (int i = 0; i < _startingBundles.Length; i++)
        {
            if (_startingBundles[i] != food) continue;
            return true;
        }
        return false;
    }

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

    private void Update_BundleData(Food_ScrObj food, int updateValue)
    {
        if (Is_Archived(food) == false) return;

        FoodData updateData = Archived_BundleData(food);
        int currentAmount = updateData.currentAmount;

        if (currentAmount + updateValue <= 0)
        {
            if (Is_StartingBundle(food)) return;

            _archivedBundles.Remove(updateData);
            return;
        }

        updateData.Update_Amount(updateValue);
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

    private FoodData RandomWeighted_BundleData()
    {
        // get total wieght
        int totalWeight = 0;

        foreach (FoodData data in _archivedBundles)
        {
            totalWeight += data.currentAmount;
        }

        // track values
        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // get random food according to weight
        for (int i = 0; i < _archivedBundles.Count; i++)
        {
            cumulativeWeight += _archivedBundles[i].currentAmount;

            if (randValue >= cumulativeWeight) continue;

            return _archivedBundles[i];
        }

        return null;
    }


    // Unlock Datas
    private bool Is_Unlocked(Food_ScrObj food)
    {
        for (int i = 0; i < _unlockDatas.Count; i++)
        {
            if (_unlockDatas[i].foodScrObj != food) continue;
            if (_unlockDatas[i].currentAmount >= food.unlockAmount) return true;
        }
        return false;
    }

    private bool Unlock_inProgress(Food_ScrObj food)
    {
        if (Is_Unlocked(food)) return false;

        for (int i = 0; i < _unlockDatas.Count; i++)
        {
            if (_unlockDatas[i].foodScrObj != food) continue;
            return true;
        }

        return false;
    }

    private int Unlock_Count(Food_ScrObj food)
    {
        if (Is_Unlocked(food) == false) return 0;

        return UnlockData(food).currentAmount;
    }


    private FoodData UnlockData(Food_ScrObj food)
    {
        if (Is_Unlocked(food) == false) return null;

        for (int i = 0; i < _unlockDatas.Count; i++)
        {
            if (food != _unlockDatas[i].foodScrObj) continue;
            return _unlockDatas[i];
        }

        return null;
    }

    private FoodData Add_toUnlockData(Food_ScrObj food)
    {
        if (food.ingredients.Count <= 0) return null;

        for (int i = 0; i < _unlockDatas.Count; i++)
        {
            if (_unlockDatas[i].foodScrObj != food) continue;

            int currentAmount = _unlockDatas[i].currentAmount;
            int setAmount = Mathf.Clamp(currentAmount + 1, 0, food.unlockAmount);

            _unlockDatas[i].Set_Amount(setAmount);

            return _unlockDatas[i];
        }

        FoodData newFood = new(food);
        _unlockDatas.Add(newFood);

        return newFood;
    }


    // Basics
    private void Interact_FacePlayer()
    {
        // facing to player direction
        _npcController.basicAnim.Flip_Sprite(_npcController.interactable.detection.player.gameObject);

        NPC_Movement movement = _npcController.movement;

        movement.Stop_FreeRoam();
        movement.Free_Roam(_currentSubLocation.roamArea, movement.Random_IntervalTime());
    }


    private void Start_Action()
    {
        _actionTimer.Toggle_RunAnimation(true);

        _npcController.interactable.UnInteract();
        _npcController.interactable.LockInteract(true);

        _foodBox.color = Color.white;

        Toggle_QuestBar();

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

        _actionTimer.Toggle_RunAnimation(false);

        _npcController.interactable.LockInteract(false);
        _foodBox.color = Color.clear;

        Toggle_QuestBar();

        // return to free roam
        NPC_Movement move = _npcController.movement;
        move.Free_Roam(_currentSubLocation.roamArea, move.Random_IntervalTime());
    }


    // Quest
    public void Toggle_QuestBar()
    {
        ActionBubble_Interactable interactable = _npcController.interactable;

        if (interactable.detection.player == null || _actionTimer.animationRunning)
        {
            _questBarObject.SetActive(false);
            return;
        }

        Action_Bubble bubble = interactable.bubble;
        _questBarObject.SetActive(!bubble.bubbleOn);

        if (_questBarObject.activeSelf == false) return;

        _questBar.Load_Custom(_questCount, _currentQuestCount);
        _questBar.Toggle(true);
    }


    private void Set_QuestFood(Food_ScrObj food)
    {
        if (food == null)
        {
            _questFood = null;
            return;
        }

        _questFood = new(food);
        _previousQuestFood = new(food);

        ActionBubble_Interactable interactable = _npcController.interactable;
        Action_Bubble bubble = interactable.bubble;

        bubble.Set_Bubble(bubble.leftIcon.sprite, food.sprite);
        interactable.UnInteract();
    }

    /// <summary>
    /// Sets a random quest food that was not previously set
    /// </summary>
    private void Set_QuestFood()
    {
        if (_questFood != null) return;
        if (_currentQuestCount >= _questCount) return;

        if (_currentQuestCool < _questCoolTime)
        {
            _currentQuestCool++;
            return;
        }

        List<FoodData> unlockedDatas = new(_unlockDatas);

        for (int i = 0; i < unlockedDatas.Count; i++)
        {
            if (_previousQuestFood == null) break;
            if (_previousQuestFood.foodScrObj != unlockedDatas[i].foodScrObj) continue;

            unlockedDatas.RemoveAt(i);
            break;
        }

        int randIndex = Random.Range(0, unlockedDatas.Count);
        Food_ScrObj questFood = unlockedDatas[randIndex].foodScrObj;

        Set_QuestFood(questFood);
    }


    private void Complete_Quest()
    {
        if (_questFood == null) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        ActionBubble_Interactable interactable = _npcController.interactable;

        Player_Controller player = interactable.detection.player;
        FoodData_Controller playerIcon = player.foodIcon;

        if (playerIcon.hasFood == false)
        {
            dialog.Update_Dialog(2);
            return;
        }

        if (playerIcon.Is_SameFood(_questFood.foodScrObj) == false)
        {
            dialog.Update_Dialog(3);
            return;
        }

        // quest count update
        _currentQuestCount++;
        _currentQuestCount = Mathf.Clamp(_currentQuestCount, 0, _questCount);
        Toggle_QuestBar();

        // remove player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        // animation
        player.coinLauncher.Parabola_CoinLaunch(_questFood.foodScrObj.sprite, transform.position);

        // refresh
        _currentQuestCool = 0;
        _questFood = null;

        Action_Bubble bubble = _npcController.interactable.bubble;
        bubble.Set_Bubble(bubble.leftIcon.sprite, null);

        dialog.Update_Dialog(4);

        // discount reward
        Set_Discount();
    }


    // Restock
    private void Restock_Instant()
    {
        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].stockData.unlocked == false) continue;
            if (_foodStocks[i].stockData.isDiscount) continue;

            // clear data
            _foodStocks[i].foodIcon.Set_CurrentData(null);

            // get food
            Food_ScrObj bundleFood = RandomWeighted_BundleData().foodScrObj;
            Update_BundleData(bundleFood, -1);

            List<Food_ScrObj> foodIngredients = bundleFood.Ingredients();

            Food_ScrObj restockFood = foodIngredients[Random.Range(0, foodIngredients.Count)];
            Update_BundleData(restockFood, -1);

            // restock
            _foodStocks[i].Set_FoodData(new(restockFood));
            _foodStocks[i].Update_Amount(_foodStocks[i].foodIcon.maxAmount - 1);
            _foodStocks[i].Toggle_Discount(false);
        }
    }

    public void Restock()
    {
        if (_actionCoroutine != null) return;

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
            if (stocks[i].stockData.isDiscount) continue;

            // move to stock 
            movement.Assign_TargetPosition(stocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // get food
            Food_ScrObj bundleFood = RandomWeighted_BundleData().foodScrObj;
            Update_BundleData(bundleFood, -1);

            List<Food_ScrObj> foodIngredients = bundleFood.Ingredients();

            Food_ScrObj restockFood = foodIngredients[Random.Range(0, foodIngredients.Count)];
            Update_BundleData(restockFood, -1);

            // restock
            stocks[i].Set_FoodData(new(restockFood, stocks[i].foodIcon.maxAmount));

            yield return new WaitForSeconds(_actionSpeed);
        }

        Cancel_Action();
        yield break;
    }

    private void Restock_Unlocks()
    {
        if (_actionCoroutine != null) return;

        _actionCoroutine = StartCoroutine(Restock_Unlocks_Coroutine());
    }
    private IEnumerator Restock_Unlocks_Coroutine()
    {
        Start_Action();

        List<FoodStock> stocks = FoodStocks_byDistance();
        NPC_Movement movement = _npcController.movement;

        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].stockData.unlocked == false) continue;
            if (stocks[i].foodIcon.hasFood) continue;

            movement.Assign_TargetPosition(stocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // get food
            Food_ScrObj bundleFood = RandomWeighted_BundleData().foodScrObj;
            Update_BundleData(bundleFood, -1);

            List<Food_ScrObj> foodIngredients = bundleFood.Ingredients();

            Food_ScrObj restockFood = foodIngredients[Random.Range(0, foodIngredients.Count)];
            Update_BundleData(restockFood, -1);

            // restock
            stocks[i].Set_FoodData(new(restockFood, stocks[i].foodIcon.maxAmount));

            yield return new WaitForSeconds(_actionSpeed);
        }

        Cancel_Action();
        yield break;
    }


    // Discount
    private List<FoodStock> DiscountAvailable_Stocks()
    {
        List<FoodStock> sortedStocks = new();

        // sort stocks according to conditions
        for (int i = 0; i < FoodStocks_byDistance().Count; i++)
        {
            if (FoodStocks_byDistance()[i].stockData.unlocked == false) continue;
            if (FoodStocks_byDistance()[i].foodIcon.hasFood == false) continue;
            if (FoodStocks_byDistance()[i].stockData.isDiscount) continue;

            sortedStocks.Add(FoodStocks_byDistance()[i]);
        }

        return sortedStocks;
    }

    private void Set_Discount()
    {
        if (_actionCoroutine != null) return;
        if (_currentQuestCount < _questCount) return;
        if (DiscountAvailable_Stocks().Count <= 0) return;

        _currentQuestCount = 0;
        Toggle_QuestBar();

        _actionCoroutine = StartCoroutine(Set_Discount_Coroutine());
    }
    private IEnumerator Set_Discount_Coroutine()
    {
        Start_Action();
        NPC_Movement movement = _npcController.movement;

        FoodStock randStock = DiscountAvailable_Stocks()[Random.Range(0, DiscountAvailable_Stocks().Count)];

        movement.Assign_TargetPosition(randStock.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        randStock.Toggle_Discount(true);
        randStock.Set_Amount(randStock.foodIcon.maxAmount);

        Cancel_Action();
        yield break;
    }


    // Food Bundle Stocks
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

        List<PlaceableStock> completedStocks = new(Complete_Stocks());

        // loops through placeable stocks
        for (int i = 0; i < completedStocks.Count; i++)
        {
            movement.Assign_TargetPosition(completedStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            List<FoodData> placedDatas = completedStocks[i].foodIcon.AllDatas();

            // loops through placed foods
            for (int j = 0; j < placedDatas.Count; j++)
            {
                Archive_toBundles(placedDatas[j].foodScrObj);
                Add_toUnlockData(placedDatas[j].foodScrObj);

                if (Is_Unlocked(placedDatas[j].foodScrObj) == false) continue;

                // loops through unlocked foods
                for (int k = 0; k < placedDatas[j].foodScrObj.Unlocks().Count; k++)
                {
                    if (Unlock_inProgress(placedDatas[j].foodScrObj.Unlocks()[k])) continue;

                    Add_toUnlockData(placedDatas[j].foodScrObj.Unlocks()[k]).Set_Amount(0);
                }

                // loops through unlocked food ingredients
                foreach (Food_ScrObj newIngredient in placedDatas[j].foodScrObj.Unlocks_Ingredients())
                {
                    Archive_toBundles(newIngredient);
                }
            }

            completedStocks[i].Reset_Data();
        }

        Cancel_Action();
        yield break;
    }


    private void DialogUpdate_UnlockCount()
    {
        FoodData_Controller playerIcon = _npcController.interactable.detection.player.foodIcon;

        if (playerIcon.hasFood == false) return;

        Data_Controller data = Main_Controller.instance.dataController;
        Food_ScrObj playerFood = playerIcon.currentData.foodScrObj;

        if (data.Is_RawFood(playerFood)) return;

        Sprite playerFoodSprite = playerFood.sprite;
        string dialogInfo = Unlock_Count(playerFood) + "/" + playerFood.unlockAmount + " collected from bundle";

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        DialogData dialogData = new(playerFoodSprite, dialogInfo);

        dialog.Update_Dialog(dialogData);
    }
}