using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroceryNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private IInteractable_Controller _interactable;

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

    private HashSet<FoodData> _unlockDatas = new();


    [Header("")]
    [SerializeField][Range(0, 100)] private int _questCoolTime;
    [SerializeField][Range(0, 100)] private int _questCount;

    private int _currentQuestCool;
    private int _currentQuestCount;

    private bool _questComplete;


    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        Load_CurrentFoodStocks();
        Load_PlaceableStocks();

        _questBar.Toggle_BarColor(true);
        Toggle_QuestBar();

        // untrack
        Main_Controller.instance.UnTrack_CurrentCharacter(gameObject);

        // food box toggle
        _foodBox.color = Color.clear;

        // free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        // action subscription
        Main_Controller.instance.worldMap.OnNewLocation += Restock_Instant;

        GlobalTime_Controller.instance.OnDayTime += Restock;
        GlobalTime_Controller.instance.OnTimeTik += Restock_EmptyStocks;

        GlobalTime_Controller.instance.OnTimeTik += Collect_FoodBundles;
        GlobalTime_Controller.instance.OnTimeTik += Set_QuestFood;
        GlobalTime_Controller.instance.OnTimeTik += Set_Discount;

        _npcController.movement.TargetPosition_UpdateEvent += FoodBox_DirectionUpdate;

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

        GlobalTime_Controller.instance.OnDayTime -= Restock;
        GlobalTime_Controller.instance.OnTimeTik -= Restock_EmptyStocks;

        GlobalTime_Controller.instance.OnTimeTik -= Collect_FoodBundles;
        GlobalTime_Controller.instance.OnTimeTik -= Set_QuestFood;
        GlobalTime_Controller.instance.OnTimeTik -= Set_Discount;

        _npcController.movement.TargetPosition_UpdateEvent -= FoodBox_DirectionUpdate;

        _interactable.OnInteract -= Cancel_Action;
        _interactable.OnInteract -= Interact_FacePlayer;

        _detection.EnterEvent -= Toggle_QuestBar;
        _detection.ExitEvent -= Toggle_QuestBar;

        _interactable.OnHoldInteract -= Complete_Quest;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GroceryNPC/_unlockDatas", _unlockDatas);

        ES3.Save("GroceryNPC/_currentQuestCool", _currentQuestCool);
        ES3.Save("GroceryNPC/_currentQuestCount", _currentQuestCount);

        ES3.Save("GroceryNPC/_questComplete", _questComplete);

        FoodData_Controller questIcon = _npcController.foodIcon;
        FoodData questFoodData = new(questIcon.currentData);

        ES3.Save("GroceryNPC/questData", questFoodData);

        Save_CurrentFoodStocks();
        Save_PlaceableStocks();
    }

    public void Load_Data()
    {
        _unlockDatas = ES3.Load("GroceryNPC/_unlockDatas", _unlockDatas);

        // load starting bundle
        for (int i = 0; i < _startingBundles.Length; i++)
        {
            if (UnlockData(_startingBundles[i]) != null) continue;
            Unlock(_startingBundles[i]);
        }

        foreach (FoodData unlockData in _unlockDatas)
        {
            Unlock_New(unlockData.foodScrObj);
        }

        _currentQuestCool = ES3.Load("GroceryNPC/_currentQuestCool", _currentQuestCool);
        _currentQuestCount = ES3.Load("GroceryNPC/_currentQuestCount", _currentQuestCount);

        _questComplete = ES3.Load("GroceryNPC/_questComplete", _questComplete);

        FoodData questFoodData = null;
        questFoodData = new(ES3.Load("GroceryNPC/questData", questFoodData));

        if (_questComplete)
        {
            FoodData_Controller questIcon = _npcController.foodIcon;

            questIcon.Set_CurrentData(new(questFoodData.foodScrObj));
            questIcon.Show_Icon();

            return;
        }

        if (questFoodData == null) return;

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


    // Quest
    public void Toggle_QuestBar()
    {
        if (!_npcController.foodIcon.hasFood || _detection.player == null || _actionTimer.animationRunning)
        {
            _questBarObject.SetActive(false);
            return;
        }

        _questBar.Load_Custom(_questCount, _currentQuestCount);
        _questBar.Toggle(true);

        _questBarObject.SetActive(true);
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

        _questComplete = false;

        questIcon.Set_CurrentData(new(food));
        questIcon.Show_Icon(0.5f);
    }

    /// <summary>
    /// Sets a random quest food that was not previously set
    /// </summary>
    private void Set_QuestFood()
    {
        FoodData_Controller questIcon = _npcController.foodIcon;

        if (questIcon.hasFood && !_questComplete) return;
        if (_currentQuestCount >= _questCount) return;

        if (_currentQuestCool < _questCoolTime)
        {
            _currentQuestCool++;
            return;
        }

        if (_actionCoroutine != null) return;

        List<FoodData> unlockedDatas = new(_unlockDatas);

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
        
        if (_currentQuestCount >= _questCount)
        {
            dialog.Update_Dialog(2);
            return;
        }

        if (_questComplete) return;

        FoodData_Controller playerIcon = _detection.player.foodIcon;
        FoodData_Controller questIcon = _npcController.foodIcon;

        if (questIcon.Is_SameFood(playerIcon.currentData.foodScrObj) == false)
        {
            dialog.Update_Dialog(0);
            return;
        }

        _questComplete = true;
        _currentQuestCool = 0;

        questIcon.Show_Icon();

        // quest count update
        _currentQuestCount++;
        _currentQuestCount = Mathf.Clamp(_currentQuestCount, 0, _questCount);

        Toggle_QuestBar();

        // remove player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        Set_Discount();
        
        if (_currentQuestCount < _questCount)
        {
            DialogBox updateBox = dialog.Update_Dialog(1);
            updateBox.data.Set_SmartInfo("questCount", _currentQuestCount + "/" + _questCount);
            
            return;
        }
        
        dialog.Update_Dialog(2);
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


    // Data Control
    private void Unlock(Food_ScrObj food)
    {
        if (Main_Controller.instance.dataController.Is_RawFood(food)) return;

        FoodData unlockData = UnlockData(food);

        if (unlockData == null)
        {
            _unlockDatas.Add(new(food));
            return;
        }

        int currentAmount = unlockData.currentAmount;
        int setAmount = Mathf.Clamp(currentAmount + 1, 0, food.unlockAmount);

        unlockData.Set_Amount(setAmount);
    }

    private void Unlock_New(Food_ScrObj food)
    {
        if (!Data_Unlocked(food)) return;

        foreach (Food_ScrObj unlockFood in food.Unlocks())
        {
            if (UnlockData(unlockFood) != null) continue;
            Unlock(unlockFood);
        }
    }


    private FoodData UnlockData(Food_ScrObj food)
    {
        foreach (FoodData data in _unlockDatas)
        {
            if (food != data.foodScrObj) continue;
            return data;
        }
        return null;
    }

    /// <returns>
    /// Unlock count (currentAmount) highest to lowest values to datas
    /// </returns>
    private List<FoodData> UnlockDatas()
    {
        List<FoodData> unlockDatas = new(_unlockDatas);

        for (int i = 0; i < unlockDatas.Count - 1; i++)
        {
            int maxIndex = i;

            // compare and arrange most to least unlock count datas
            for (int j = i + 1; j < unlockDatas.Count; j++)
            {
                if (unlockDatas[j].currentAmount <= unlockDatas[maxIndex].currentAmount) continue;

                maxIndex = j;
            }

            if (maxIndex == i) continue;

            FoodData temp = unlockDatas[i];

            unlockDatas[i] = unlockDatas[maxIndex];
            unlockDatas[maxIndex] = temp;
        }

        return unlockDatas;
    }

    private FoodData UnlockingData()
    {
        List<FoodData> unlockedDatas = UnlockDatas();
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


    private int Unlock_Count(Food_ScrObj food)
    {
        FoodData data = UnlockData(food);

        if (data == null) return 0;
        return data.currentAmount;
    }

    private bool Data_Unlocked(Food_ScrObj food)
    {
        return Unlock_Count(food) >= food.unlockAmount;
    }


    private HashSet<Food_ScrObj> UnlockedFood_Ingredients()
    {
        HashSet<Food_ScrObj> ingredients = new();

        foreach (FoodData data in _unlockDatas)
        {
            List<Food_ScrObj> dataIngredients = data.foodScrObj.Ingredients();

            foreach (Food_ScrObj ingredient in dataIngredients)
            {
                if (ingredient.price <= 0) continue;
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
            if (i > _placeableStocks.Length - 1) break;

            _placeableStocks[i].Load_Data(placedDatas[i], completeDatas[i]);
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

        // loops through empty placeable stocks
        for (int i = 0; i < completedStocks.Count; i++)
        {
            movement.Assign_TargetPosition(completedStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            List<FoodData> placedDatas = completedStocks[i].foodIcon.AllDatas();

            // loops through placed foods
            foreach (FoodData data in placedDatas)
            {
                Food_ScrObj dataFood = data.foodScrObj;

                Unlock(dataFood);
                Unlock_New(dataFood);
            }

            completedStocks[i].Reset_Data();

            // update all unlock previews for _placeableStocks
            foreach (PlaceableStock stock in _placeableStocks)
            {
                Toggle_UnlockingFood(stock);
            }
        }

        Cancel_Action();
        yield break;
    }


    private void Toggle_UnlockingFood(PlaceableStock placeStock)
    {
        if (placeStock.foodIcon.hasFood) return;

        FoodData_Controller previewIcon = placeStock.previewIcon;
        FoodData progressData = UnlockingData();

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