using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Serialization;

public class FoodStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Space(20)]
    [SerializeField] private DialogTrigger _dialog;

    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private CoinLauncher _launcher;
    
    [Space(20)]
    [SerializeField] private Sprite[] _sprites;

    [Space(20)]
    [SerializeField] private SpriteRenderer _tagSR;
    [SerializeField] private Sprite[] _tagSprites;

    [Space(20)]
    [Range(0, 999)][SerializeField] private int _defaultUnlockPrice;
    public int defaultUnlockPrice => _defaultUnlockPrice;
    
    [Range(0, 999)][SerializeField] private int _bonusUnlockPrice;
    public int bonusUnlockPrice => _bonusUnlockPrice;
    
    [Range(0, 100)][SerializeField] private int _discountPercentage;

    [Space(60)]
    [SerializeField] private VideoGuide_Trigger _guideTrigger;


    private FoodStock_Data _data;
    public FoodStock_Data data => _data;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        if (_data != null) return;
        _data = new FoodStock_Data(new StockData(false));
    }

    private void Start()
    {
        Update_Sprite();
        Update_TagSprite();
        Update_Bubble();
        
        Toggle_AmountBar();

        // subscriptions
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.detection.ExitEvent += Toggle_AmountBar;
 
        _interactable.OnInteract += Toggle_Dialog;
        _interactable.OnInteract += Toggle_Price;

        _interactable.OnInteract += Toggle_AmountBar;
        _interactable.OnUnInteract += Toggle_AmountBar;

        _interactable.OnAction1 += Purchase_Stock;
        _interactable.OnAction1 += Purchase_Single;
        _interactable.OnAction2 += Purchase_All;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;
 
        _interactable.OnInteract -= Toggle_Dialog;
        _interactable.OnInteract -= Toggle_Price;

        _interactable.OnInteract -= Toggle_AmountBar;
        _interactable.OnUnInteract -= Toggle_AmountBar;

        _interactable.OnAction1 -= Purchase_Stock;
        _interactable.OnAction1 -= Purchase_Single;
        _interactable.OnAction2 -= Purchase_All;
    }


    // Data
    public void Set_Data(FoodStock_Data data)
    {
        if (data == null) return;
        _data = data;
    }
    
    public void Set_FoodData(FoodData data)
    {
        if (data == null) return;
        
        _data.stockedFoodDatas.Clear();
        _data.Set_StockedFoodData(data);

        _foodIcon.Set_CurrentData(data);
        _foodIcon.Show_Icon();
        
        if (_foodIcon.hasFood == false) return;

        Update_TagSprite();
    }


    // Amount Data
    public int Current_Amount()
    {
        if (_foodIcon.hasFood == false) return 0;
        return _foodIcon.currentData.currentAmount;
    }

    public void Set_Amount(int setValue)
    {
        if (_foodIcon.hasFood == false) return;

        _foodIcon.currentData.Set_Amount(setValue);
        _data.Recent_StockedFood().Set_Amount(setValue);

        Toggle_AmountBar();
        Update_TagSprite();
    }

    public void Update_Amount(int updateValue)
    {
        if (_foodIcon.hasFood == false) return;

        _foodIcon.currentData.Update_Amount(updateValue);
        _data.Recent_StockedFood().Update_Amount(updateValue);

        Toggle_AmountBar();
        Update_TagSprite();
    }


    // Toggle Control
    public void Toggle_Unlock(bool toggle)
    {
        _data.stockData.Toggle_UnLock(toggle);

        Update_Sprite();
        Update_TagSprite();
        Update_Bubble();
    }

    public void Toggle_Discount(bool toggle)
    {
        _data.stockData.Toggle_Discount(toggle);

        Update_TagSprite();
    }
    

    private void Toggle_AmountBar()
    {
        bool playerDetected = _interactable.detection.player != null;
        bool bubbleOn = _interactable.bubble.bubbleOn;

        _foodIcon.Toggle_AmountBar(playerDetected && !bubbleOn);
    }


    private void Toggle_Price()
    {
        if (_data.stockData.unlocked == false) return;
        
        if (_data.stockPurchaseData.purchased == false)
        {
            int unlockPrice = _data.stockPurchaseData == null ? _defaultUnlockPrice : _data.stockPurchaseData.price;
            GoldSystem.instance.Indicate_TriggerData(new(_dialog.defaultData.icon, -unlockPrice));
            return;
        }

        if (_foodIcon.hasFood == false) return;

        Food_ScrObj currentFood = _foodIcon.currentData.foodScrObj;
        GoldSystem.instance.Indicate_TriggerData(new(currentFood.sprite, -StockedFood_Price()));
    }

    private void Toggle_Dialog()
    {
        if (_data.Stock_Purchased() == false) return;
        
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_foodIcon.currentData == null)
        {
            // no food currenlty stocked!
            dialog.Update_Dialog(0);

            interactable.UnInteract();
            return;
        }

        if (_foodIcon.currentData.currentAmount > 0) return;

        // not enough food amount currenlty stocked
        dialog.Update_Dialog(1);

        interactable.UnInteract();
    }


    // Sprite Control
    private void Update_Bubble()
    {
        Action_Bubble bubble = _interactable.bubble;

        if (_data.stockData.unlocked == false)
        {
            bubble.Empty_Bubble();
            bubble.Set_IndicatorToggleDatas(null);
            return;
        }

        if (_data.Stock_Purchased() == false)
        {
            Sprite unlockSprite = bubble.bubbleDatas[0].iconSprite;

            bubble.Set_Bubble(unlockSprite, null);
            bubble.Toggle_Height(true);

            bubble.Set_IndicatorToggleDatas(new() { bubble.bubbleDatas[0] });
            return;
        }

        bubble.Set_Bubble(bubble.bubbleDatas[1].iconSprite, bubble.bubbleDatas[2].iconSprite);
        bubble.Toggle_Height(false);
        
        bubble.Set_IndicatorToggleDatas(new() { bubble.bubbleDatas[1], bubble.bubbleDatas[2] });
    }

    private void Update_Sprite()
    {
        if (_data.stockData.unlocked == false)
        {
            _sr.sprite = _sprites[0];
            return;
        }

        if (_data.Stock_Purchased() == false)
        {
            _sr.sprite = _sprites[1];
            return;
        }

        _sr.sprite = _sprites[2];
    }

    private void Update_TagSprite()
    {
        if (_data.Stock_Purchased() == false)
        {
            _tagSR.color = Color.clear;
            return;
        }
        _tagSR.color = Color.white;
        
        if (_data.stockData.isDiscount)
        {
            _tagSR.sprite = _tagSprites[2];
            return;
        }

        if (_foodIcon.hasFood == false || _foodIcon.currentData.currentAmount <= 0)
        {
            _tagSR.sprite = _tagSprites[0];
            return;
        }

        _tagSR.sprite = _tagSprites[1];
    }

    
    // Interaction
    private void Purchase_Stock()
    {
        if (_data.stockData.unlocked == false) return;
        
        PurchaseData stockPurchaseData = _data.stockPurchaseData;
        if (stockPurchaseData.purchased) return;

        int unlockPrice = stockPurchaseData == null ? _defaultUnlockPrice : stockPurchaseData.price;
        
        if (GoldSystem.instance.Update_CurrentAmount(-unlockPrice) == false)
        {
            _interactable.UnInteract();
            return;
        }

        stockPurchaseData.Toggle_PurchaseState(true);
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[1], transform.position);

        Update_Sprite();
        Update_TagSprite();
        Update_Bubble();
        
        _interactable.UnInteract();
    }
    
    
    private int StockedFood_Price()
    {
        StockData stockData = _data.stockData;
        
        if (stockData.unlocked == false || _foodIcon.hasFood == false) return 0;

        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;
        int price = stockedFood.price;

        if (stockData.isDiscount && price > 0)
        {
            float discountValue = 1f - (_discountPercentage / 100f);
            price = Mathf.FloorToInt(price * discountValue);
        }

        return price;
    }
    
    private bool Purchase(int purchaseAmount)
    {
        if (_data.stockData.unlocked == false) return false;
        
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_foodIcon.hasFood == false)
        {
            dialog.Update_Dialog(0);
            return false;
        }

        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;
        int stockedAmount = _foodIcon.currentData.currentAmount;

        if (stockedAmount <= 0 || stockedAmount < purchaseAmount)
        {
            dialog.Update_Dialog(1);
            return false;
        }

        Main_Controller main = Main_Controller.instance;

        // add food
        FoodMenu_Controller foodMenu = main.currentVehicle.menu.foodMenu;
        int leftOverAmount = foodMenu.Add_FoodItem(stockedFood, purchaseAmount);

        if (leftOverAmount > 0)
        {
            // set to recent amount
            foodMenu.Remove_FoodItem(stockedFood, purchaseAmount - leftOverAmount);

            // Not enough space in food storage!
            dialog.Update_Dialog(2);

            _interactable.UnInteract();
            return false;
        }

        // check nugget amount
        if (GoldSystem.instance.Update_CurrentAmount(-(StockedFood_Price() * purchaseAmount)) == false)
        {
            _interactable.UnInteract();
            return false;
        }

        //
        Update_Amount(-purchaseAmount);
        Update_TagSprite();

        // add purchased food to archive
        ArchiveMenu_Controller archiveMenu = main.currentVehicle.menu.archiveMenu;
        archiveMenu.Archive_Food(stockedFood);

        // deactivate discount on empty amount
        if (Current_Amount() > 0) return true;

        Toggle_Discount(false);
        return true;
    }

    private void Purchase_Single()
    {
        if (Purchase(1) == false) return;

        // coin launch animation
        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;
        Transform player = _interactable.detection.player.transform;

        _launcher.Parabola_CoinLaunch(stockedFood.sprite, player.position);
    }

    private void Purchase_All()
    {
        if (_data.stockPurchaseData.purchased == false) return;
        if (Purchase(_foodIcon.currentData.currentAmount) == false) return;

        // coin launch animation
        Transform player = _interactable.detection.player.transform;
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], player.position);
    }
}