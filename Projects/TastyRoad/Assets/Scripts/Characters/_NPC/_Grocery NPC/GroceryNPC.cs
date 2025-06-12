using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroceryNPC : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private NPC_Controller _npcController;
    
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private IInteractable_Controller _interactable;

    [Space(20)]
    [SerializeField] private SpriteRenderer _foodBox;
    [SerializeField] private Clock_Timer _actionTimer;

    [Space(20)]
    [SerializeField] private GameObject _questBarObject;
    [SerializeField] private AmountBar _questBar;

    [Space(20)]
    [SerializeField] private SubLocation _currentSubLocation;

    [Space(20)]
    [SerializeField] private FoodStock[] _foodStocks;
    [SerializeField] private PlaceableStock[] _placeableStocks;
    
    [Space(20)]
    [SerializeField] private Food_ScrObj[] _startingBundles;

    [Space(20)]
    [SerializeField][Range(0, 1)] private float _actionSpeed;

    [Space(10)]
    [SerializeField][Range(0, 100)] private int _questCoolTime;
    [SerializeField][Range(0, 100)] private int _questCount;

    [Space(60)] 
    [SerializeField] private VideoGuide_Trigger _guideTrigger;

    
    private GroceryNPC_Data _data;
    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        Load_CurrentFoodStocks();
        UpdateFoodStock_UnlockPrices();
        
        Load_PlaceableStocks();

        _npcController.foodIcon.Show_Icon(0.5f);
        
        _questBar.Toggle_BarColor(true);
        Toggle_QuestBar();

        Update_BundleQuest();

        // untrack
        Main_Controller.instance.UnTrack_CurrentCharacter(gameObject);

        // food box toggle
        _foodBox.color = Color.clear;

        // free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        // action subscription
        Main_Controller.instance.worldMap.OnNewLocation += Restock_Instant;

        globaltime globaltime = globaltime.instance;
        
        globaltime.OnTimeTik += Collect_FoodBundles;
        globaltime.OnTimeTik += Set_Discount;
        
        globaltime.OnDayTime += Restock;
        globaltime.OnTimeTik += Restock_EmptyStocks;

        _npcController.movement.TargetPosition_UpdateEvent += FoodBox_DirectionUpdate;

        _interactable.OnInteract += _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.OnInteract += Cancel_Action;
        _interactable.OnInteract += Interact_FacePlayer;

        _detection.EnterEvent += Toggle_QuestBar;
        _detection.ExitEvent += Toggle_QuestBar;

        _interactable.OnHoldInteract += Complete_Quest;
    }

    private void OnDestroy()
    {
        // action subscription
        Main_Controller.instance.worldMap.OnNewLocation -= Restock_Instant;
        
        globaltime globaltime = globaltime.instance;
        
        globaltime.OnTimeTik -= Collect_FoodBundles;
        globaltime.OnTimeTik -= Set_Discount;
        
        globaltime.OnDayTime -= Restock;
        globaltime.OnTimeTik -= Restock_EmptyStocks;

        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        _interactable.OnInteract -= _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.OnInteract -= Cancel_Action;
        _interactable.OnInteract -= Interact_FacePlayer;

        _detection.EnterEvent -= Toggle_QuestBar;
        _detection.ExitEvent -= Toggle_QuestBar;

        _interactable.OnHoldInteract -= Complete_Quest;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        FoodData_Controller questIcon = _npcController.foodIcon;
        FoodData questFoodData = new(questIcon.currentData);

        ES3.Save("GroceryNPC/GroceryNPC_Data", _data);
        ES3.Save("GroceryNPC/questData", questFoodData);

        Save_CurrentFoodStocks();
        Save_PlaceableStocks();
    }

    public void Load_Data()
    {
        // load starting bundle
        HashSet<FoodData> startingBundleDatas = new();

        foreach (Food_ScrObj food in _startingBundles)
        {
            startingBundleDatas.Add(new(food));
        }
        _data = ES3.Load("GroceryNPC/GroceryNPC_Data", new GroceryNPC_Data(startingBundleDatas));

        foreach (FoodData data in _data.unlockDatas)
        {
            _data.UnlockNew_FoodData(data.foodScrObj);
        }

        FoodData questFoodData = null;
        questFoodData = new(ES3.Load("GroceryNPC/questData", questFoodData));

        if (questFoodData.foodScrObj == null)
        {
            Set_QuestFood();
            return;
        }
        Set_QuestFood(questFoodData.foodScrObj);
    }


    // Basics
    private void Interact_FacePlayer()
    {
        // facing to player direction
        _npcController.basicAnim.Flip_Sprite(_detection.player.gameObject);

        NPC_Movement movement = _npcController.movement;

        movement.Stop_FreeRoam();
        movement.Free_Roam(_currentSubLocation.roamArea, movement.intervalTime);
    }

    private void FoodBox_DirectionUpdate()
    {
        NPC_Movement move = _npcController.movement;

        // left
        if (move.Move_Direction() == -1) _foodBox.flipX = true;

        // right
        else _foodBox.flipX = false;
    }


    private void Start_Action()
    {
        _actionTimer.Toggle_RunAnimation(true);
        _interactable.Toggle_Lock(true);

        _foodBox.color = Color.white;

        Toggle_QuestBar();

        NPC_Movement movement = _npcController.movement;
        
        movement.Stop_FreeRoam();
        movement.Set_MoveSpeed(movement.defaultMoveSpeed + 3);
    }

    private void Cancel_Action()
    {
        if (_actionCoroutine != null)
        {
            StopCoroutine(_actionCoroutine);
            _actionCoroutine = null;
        }

        _actionTimer.Toggle_RunAnimation(false);
        _interactable.Toggle_Lock(false);

        _foodBox.color = Color.clear;

        Toggle_QuestBar();

        // return to free roam
        NPC_Movement move = _npcController.movement;
        
        move.Stop_FreeRoam();
        move.Set_MoveSpeed(move.defaultMoveSpeed);
        
        move.Free_Roam(_currentSubLocation.roamArea, move.Random_IntervalTime());
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
            
            // food stock subscriptions
            _foodStocks[i].OnUnlock += UpdateFoodStock_UnlockPrices;
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

    private bool Food_Stocked(Food_ScrObj searchFood)
    {
        for (int i = 0; i < _foodStocks.Length; i++)
        {
            FoodData_Controller stockIcon = _foodStocks[i].foodIcon;

            if (!stockIcon.hasFood) continue;
            if (!stockIcon.Is_SameFood(searchFood)) continue;

            return true;
        }
        return false;
    }


    private void UpdateFoodStock_UnlockPrices()
    {
        int unlockCount = 0;

        for (int i = 0; i < _foodStocks.Length; i++)
        {
            StockData stockData = _foodStocks[i].stockData;
           
            if (stockData != null && stockData.unlocked)
            {
                unlockCount++;
                continue;
            }
        }

        foreach (FoodStock foodStock in _foodStocks)
        {
            int calculatedPrice = foodStock.defaultUnlockPrice + foodStock.bonusUnlockPrice * unlockCount;
            foodStock.Set_UnlockPrice(calculatedPrice);
        }
    }


    // Quest
    public void Toggle_QuestBar()
    {
        if (_data.questComplete || _detection.player == null || _actionTimer.animationRunning)
        {
            _questBarObject.SetActive(false);
            return;
        }

        _questBarObject.SetActive(true);
        
        _questBar.Load_Custom(_questCount, _data.questCompleteCount);
        _questBar.Toggle(true);
    }


    private void Set_QuestFood(Food_ScrObj food)
    {
        FoodData_Controller questIcon = _npcController.foodIcon;

        questIcon.Update_AllDatas(null);

        if (food == null)
        {
            questIcon.Hide_Icon();
            return;
        }

        questIcon.Set_CurrentData(new(food));
        questIcon.Show_Icon(0.5f);
    }

    /// <summary>
    /// Sets a random quest food that was not previously set
    /// </summary>
    private void Set_QuestFood()
    {
        if (_actionCoroutine != null) return;
        if (_data.questComplete) return;

        FoodData_Controller questIcon = _npcController.foodIcon;
        List<FoodData> unlockedDatas = new(_data.unlockDatas);

        for (int i = 0; i < unlockedDatas.Count; i++)
        {
            if (unlockedDatas.Count <= 1) break;
            if (!questIcon.Is_SameFood(unlockedDatas[i].foodScrObj)) continue;

            unlockedDatas.RemoveAt(i);
            break;
        }

        int randIndex = Random.Range(0, unlockedDatas.Count);
        Food_ScrObj questFood = unlockedDatas[randIndex].foodScrObj;

        Set_QuestFood(questFood);
    }


    private void Complete_Quest()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        
        if (_data.questComplete)
        {
            dialog.Update_Dialog(2);
            return;
        }

        FoodData_Controller playerIcon = _detection.player.foodIcon;
        FoodData_Controller questIcon = _npcController.foodIcon;

        if (playerIcon.hasFood == false || questIcon.Is_SameFood(playerIcon.currentData.foodScrObj) == false)
        {
            dialog.Update_Dialog(0);
            return;
        }

        _data.Update_QuestCompleteCount(1);
        _data.Toggle_QuestCompleteState(_data.questCompleteCount >= _questCount);
        
        Toggle_QuestBar();

        // remove player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        if (_data.questComplete)
        {
            dialog.Update_Dialog(2);
            return;
        }
        
        Set_QuestFood();
        
        DialogBox updateBox = dialog.Update_Dialog(1);
        updateBox.data.Set_SmartInfo("questCount", _data.questCompleteCount + "/" + _questCount);
        Main_Controller.instance.dialogSystem.RefreshCurrent_DialogInfo();
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
        if (_data.questComplete == false) return;
        if (DiscountAvailable_Stocks().Count <= 0) return;

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
        
        // reset data
        _data.Toggle_QuestCompleteState(false);
        _data.Update_QuestCompleteCount(-_questCount);
        
        Set_QuestFood();

        Cancel_Action();
        yield break;
    }


    // Data
    private FoodData UnlockProgress_FoodData()
    {
        List<FoodData> unlockedDatas = _data.Unlocked_FoodDatas();
        if (unlockedDatas.Count <= 0) return null;

        for (int i = 0; i < unlockedDatas.Count; i++)
        {
            Food_ScrObj dataFood = unlockedDatas[i].foodScrObj;

            if (unlockedDatas[i].currentAmount >= dataFood.unlockAmount) continue;
            if (UnlockingFood_Toggled(dataFood)) continue;

            return unlockedDatas[i];
        }

        for (int i = 0; i < unlockedDatas.Count; i++)
        {
            Food_ScrObj dataFood = unlockedDatas[i].foodScrObj;

            if (unlockedDatas[i].currentAmount >= dataFood.unlockAmount) continue;

            return unlockedDatas[i];
        }

        return null;
    }

    private HashSet<Food_ScrObj> UnlockedFood_Ingredients()
    {
        HashSet<Food_ScrObj> ingredients = new();

        foreach (FoodData data in _data.unlockDatas)
        {
            List<Food_ScrObj> dataIngredients = data.foodScrObj.Ingredients();

            foreach (Food_ScrObj ingredient in dataIngredients)
            {
                ingredients.Add(ingredient);
            }
        }

        return ingredients;
    }

    private Food_ScrObj UnlockedFood_Ingredient()
    {
        List<Food_ScrObj> ingredients = new(UnlockedFood_Ingredients());
        
        if (ingredients.Count <= 0) return null;
        
        while (ingredients.Count > 0)
        {
            int randIndex = Random.Range(0, ingredients.Count);
            Food_ScrObj randFood = ingredients[randIndex];

            ingredients.RemoveAt(randIndex);

            if (Food_Stocked(randFood)) continue;
            return randFood;
        }

        ingredients = new(UnlockedFood_Ingredients());
        return ingredients[Random.Range(0, ingredients.Count)];
    }


    // Restock
    private void Restock_Instant()
    {
        if (UnlockedFood_Ingredients().Count <= 0) return;
        
        for (int i = 0; i < _foodStocks.Length; i++)
        {
            if (_foodStocks[i].stockData.unlocked == false) continue;
            if (_foodStocks[i].stockData.isDiscount) continue;

            // clear data
            _foodStocks[i].foodIcon.Set_CurrentData(null);

            Food_ScrObj restockFood = UnlockedFood_Ingredient();

            // restock
            _foodStocks[i].Set_FoodData(new(restockFood));
            _foodStocks[i].Update_Amount(_foodStocks[i].foodIcon.maxAmount - 1);
            _foodStocks[i].Toggle_Discount(false);
        }
    }

    /// <summary>
    /// Restock all stocks to new items
    /// </summary>
    public void Restock()
    {
        if (_actionCoroutine != null) return;
        if (UnlockedFood_Ingredients().Count <= 0) return;

        _actionCoroutine = StartCoroutine(Restock_Coroutine());
    }
    private IEnumerator Restock_Coroutine()
    {
        Start_Action();

        List<FoodStock> stocks = FoodStocks_byDistance();
        
        NPC_Movement movement = _npcController.movement;
        movement.Set_MoveSpeed(movement.defaultMoveSpeed + 2);

        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].stockData.unlocked == false) continue;
            if (stocks[i].stockData.isDiscount) continue;

            // move to stock 
            movement.Assign_TargetPosition(stocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // restock
            stocks[i].Set_FoodData(new(UnlockedFood_Ingredient(), stocks[i].foodIcon.maxAmount));

            yield return new WaitForSeconds(_actionSpeed);
        }

        Cancel_Action();
        yield break;
    }

    /// <summary>
    /// Restock unlocked empty stocks
    /// </summary>
    private void Restock_EmptyStocks()
    {
        if (_actionCoroutine != null) return;
        if (UnlockedFood_Ingredients().Count <= 0) return;

        _actionCoroutine = StartCoroutine(Restock_EmptyStock_Coroutine());
    }
    private IEnumerator Restock_EmptyStock_Coroutine()
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

            // restock
            stocks[i].Set_FoodData(new(UnlockedFood_Ingredient(), stocks[i].foodIcon.maxAmount));

            yield return new WaitForSeconds(_actionSpeed);
        }

        Cancel_Action();
        yield break;
    }


    // Placeable Stocks Control
    private void Save_PlaceableStocks()
    {
        Dictionary<List<FoodData>, StockData> placeableStockDatas = new();
        List<PurchaseData> purchaseDatas = new();

        foreach (PlaceableStock stock in _placeableStocks)
        {
            placeableStockDatas.Add(stock.foodIcon.AllDatas(), stock.data);
            purchaseDatas.Add(stock.purchaseData);
        }

        ES3.Save("GroceryNPC/placeableStockDatas", placeableStockDatas);
        ES3.Save("GroceryNPC/PlaceableStock/PurchaseData", purchaseDatas);
    }

    private void Load_PlaceableStocks()
    {
        Dictionary<List<FoodData>, StockData> placeableStockDatas = new();
        placeableStockDatas = new(ES3.Load("GroceryNPC/placeableStockDatas", placeableStockDatas));

        List<List<FoodData>> placedDatas = new(placeableStockDatas.Keys);
        List<StockData> stockDatas = new(placeableStockDatas.Values);
        
        List<PurchaseData> purchaseDatas = ES3.Load("GroceryNPC/PlaceableStock/PurchaseData", new List<PurchaseData>());

        for (int i = 0; i < placeableStockDatas.Count; i++)
        {
            if (i > _placeableStocks.Length - 1) break;
            
            _placeableStocks[i].Load_Data(placedDatas[i], stockDatas[i]);
            _placeableStocks[i].Set_PurchaseData(purchaseDatas[i]);
        }

        foreach (PlaceableStock stock in _placeableStocks)
        {
            Toggle_UnlockingFood(stock);
        }
    }


    private List<PlaceableStock> Complete_Stocks()
    {
        List<PlaceableStock> stocks = new();

        for (int i = 0; i < _placeableStocks.Length; i++)
        {
            if (_placeableStocks[i].data.isComplete == false) continue;
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

        // loops through empty placeable stocks
        for (int i = 0; i < completedStocks.Count; i++)
        {
            movement.Assign_TargetPosition(completedStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            List<FoodData> placedDatas = completedStocks[i].foodIcon.AllDatas();
            int paymentPrice = 0;
            
            // loops through placed foods
            foreach (FoodData data in placedDatas)
            {
                Food_ScrObj placedFood = data.foodScrObj;
                
                if (placedFood.Ingredients().Count <= 0) continue;
                _data.Unlock_FoodData(placedFood);

                if (_data.FoodData_UnlockMaxed(placedFood)) continue;
                paymentPrice += data.foodScrObj.price;
            }

            completedStocks[i].Set_PurchaseData(new(paymentPrice));
            completedStocks[i].Toggle_PurchaseState(true);
            
            completedStocks[i].Reset_Data();

            // update all unlock previews for _placeableStocks
            foreach (PlaceableStock stock in _placeableStocks)
            {
                Toggle_UnlockingFood(stock);
            }

            Update_BundleQuest();
        }

        Cancel_Action();
        yield break;
    }

    private void Update_BundleQuest()
    {
        TutorialQuest_Controller questController = TutorialQuest_Controller.instance;
        TutorialQuest bundleQuest = questController.CurrentQuest("CompleteAllBundles");
        
        int currentMaxCount = _data.MaxUnlocked_FoodDatas().Count;
        int completeCount = bundleQuest.currentCompleteCount;

        questController.Complete_Quest(bundleQuest, currentMaxCount - completeCount);
    }


    private void Toggle_UnlockingFood(PlaceableStock placeStock)
    {
        if (placeStock.foodIcon.hasFood) return;

        FoodData_Controller previewIcon = placeStock.previewIcon;
        FoodData progressData = UnlockProgress_FoodData();

        previewIcon.Update_AllDatas(null);

        if (progressData == null)
        {
            previewIcon.Hide_Icon();
            return;
        }

        previewIcon.Set_CurrentData(progressData);
        previewIcon.Show_Icon(0.5f);
        previewIcon.amountBar.Load_Custom(progressData.foodScrObj.unlockAmount, progressData.currentAmount);
    }

    private bool UnlockingFood_Toggled(Food_ScrObj checkFood)
    {
        for (int i = 0; i < _placeableStocks.Length; i++)
        {
            if (_placeableStocks[i].previewIcon.Is_SameFood(checkFood)) return true;
        }
        return false;
    }
}