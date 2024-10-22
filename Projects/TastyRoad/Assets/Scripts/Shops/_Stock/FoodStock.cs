using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class FoodStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Sprite[] _sprites;

    [Header("")]
    [SerializeField] private SpriteRenderer _tagSR;
    [SerializeField] private Sprite[] _tagSprites;

    [Header("")]
    [Range(1, 99)][SerializeField] private int _maxAmount;
    public int maxAmount => _maxAmount;

    [Range(0, 99)][SerializeField] private int _unlockPrice;

    [Range(0, 99)][SerializeField] private int _discountPrice;


    private StockData _stockData;
    public StockData stockData => _stockData;


    // Editor
    [HideInInspector] public Food_ScrObj foodToAdd;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (_stockData == null)
        {
            Set_StockData(new());
            Set_FoodData(new());
        }

        Toggle_Unlock(_stockData.unlocked);
        Toggle_AmountBar();

        Update_Sprite();
        Update_TagSprite();
        Update_Bubble();

        // subscriptions
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.detection.ExitEvent += Toggle_AmountBar;

        _interactable.InteractEvent += Set_Dialog;
        _interactable.InteractEvent += Toggle_AmountBar;
        _interactable.UnInteractEvent += Toggle_AmountBar;

        _interactable.OnAction1Event += Unlock;
        _interactable.OnAction1Event += Purchase_Single;
        _interactable.OnAction2Event += Purchase_All;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;

        _interactable.InteractEvent -= Set_Dialog;
        _interactable.InteractEvent -= Toggle_AmountBar;
        _interactable.UnInteractEvent -= Toggle_AmountBar;

        _interactable.OnAction1Event -= Unlock;
        _interactable.OnAction1Event -= Purchase_Single;
        _interactable.OnAction2Event -= Purchase_All;
    }


    // Data Control
    public void Set_StockData(StockData data)
    {
        _stockData = new();
        _stockData = data;
    }

    public void Set_FoodData(FoodData data)
    {
        _foodIcon.Set_CurrentData(data);
        _foodIcon.Show_Icon();

        if (_foodIcon.hasFood == false) return;
        Update_TagSprite();
    }


    public int Current_Amount()
    {
        if (_foodIcon.hasFood == false) return 0;
        return _foodIcon.currentData.currentAmount;
    }

    public void Update_Amount(int updateAmount)
    {
        if (_foodIcon.hasFood == false) return;

        _foodIcon.currentData.Update_Amount(updateAmount);

        Toggle_AmountBar();
        Update_TagSprite();
    }


    // Toggle Control
    public void Toggle_Unlock(bool toggle)
    {
        _stockData.Toggle_UnLock(toggle);

        Update_Sprite();
        Update_TagSprite();
    }

    public void Toggle_Discount(bool toggle)
    {
        _stockData.Toggle_Discount(toggle);

        Update_TagSprite();
    }


    private void Toggle_AmountBar()
    {
        Player_Controller playerDetection = _interactable.detection.player;
        FoodData foodData = _foodIcon.currentData;

        if (playerDetection == null || foodData == null || foodData.currentAmount <= 0)
        {
            _foodIcon.Toggle_AmountBar(false);
            return;
        }

        _foodIcon.Toggle_BarLock(false);
        _foodIcon.Toggle_AmountBar(!_interactable.bubble.bubbleOn);
    }


    private void Set_Dialog()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        string currentAmountString = "\nyou have " + _interactable.mainController.GoldenNugget_Amount() + " <sprite=0>";

        if (_stockData.unlocked == false)
        {
            // price to unlock + current nugget amount
            string unlockDialog = _unlockPrice + " <sprite=0> to unlock." + currentAmountString;
            dialog.Update_Dialog(new DialogData(dialog.datas[0].icon, unlockDialog));

            return;
        }

        if (_foodIcon.currentData == null)
        {
            // no food currenlty stocked!
            dialog.Update_Dialog(2);

            interactable.UnInteract();
            return;
        }

        if (_foodIcon.currentData.currentAmount <= 0)
        {
            // not enough food amount currenlty stocked
            dialog.Update_Dialog(3);

            interactable.UnInteract();
            return;
        }

        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;

        // calculation
        int price = _foodIcon.currentData.foodScrObj.price;

        if (_stockData.isDiscount && price > 0)
        {
            price -= _discountPrice;
            price = Mathf.Clamp(price, 0, stockedFood.price);
        }

        string priceString = price + " <sprite=0> to purchase." + currentAmountString;

        // price to purchase + current nugget amount
        DialogData data = new(stockedFood.sprite, priceString);
        DialogBox dialogBox = dialog.Update_Dialog(data);

        dialogBox.UpdateIcon_CenterPosition(stockedFood.uiCenterPosition);
    }


    // Sprite Control
    private void Update_Bubble()
    {
        Action_Bubble bubble = _interactable.bubble;

        if (_stockData.unlocked == false)
        {
            bubble.Set_Bubble(bubble.setSprites[0], null);
            bubble.Toggle_Height(true);

            return;
        }
        bubble.Set_Bubble(bubble.setSprites[1], bubble.setSprites[2]);
        bubble.Toggle_Height(false);
    }

    private void Update_Sprite()
    {
        if (_stockData.unlocked == false)
        {
            _sr.sprite = _sprites[0];
            return;
        }

        // raw food & default
        _sr.sprite = _sprites[1];
    }

    private void Update_TagSprite()
    {
        if (_stockData.unlocked == false)
        {
            _tagSR.color = Color.clear;
            return;
        }

        _tagSR.color = Color.white;

        if (_stockData.isDiscount)
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


    // Functions
    private bool Purchase(int purchaseAmount)
    {
        if (_stockData.unlocked == false || _foodIcon.hasFood == false) return false;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;
        int stockedAmount = _foodIcon.currentData.currentAmount;

        if (stockedAmount <= 0) return false;

        // calculation
        int price = stockedFood.price * purchaseAmount;

        if (_stockData.isDiscount && price > 0)
        {
            price -= _discountPrice * purchaseAmount;
            price = Mathf.Clamp(price, 0, stockedFood.price * purchaseAmount);
        }

        // check nugget amount
        if (_interactable.mainController.GoldenNugget_Amount() < price)
        {
            // Not enough golden nuggets to purchase!
            dialog.Update_Dialog(4);

            _interactable.UnInteract();
            return false;
        }


        _interactable.mainController.Remove_GoldenNugget(price);

        // add food
        FoodMenu_Controller foodMenu = _interactable.mainController.currentVehicle.menu.foodMenu;
        int leftOverAmount = foodMenu.Add_FoodItem(stockedFood, purchaseAmount);

        if (leftOverAmount > 0)
        {
            // set to recent amount
            foodMenu.Remove_FoodItem(stockedFood, purchaseAmount - leftOverAmount);

            // return golden nuggets
            _interactable.mainController.Add_GoldenNugget(stockedFood.price);

            // Not enough space in food storage!
            dialog.Update_Dialog(5);

            _interactable.UnInteract();
            return false;
        }

        //
        Update_Amount(-purchaseAmount);
        Update_TagSprite();

        // add purchased food to archive
        ArchiveMenu_Controller archiveMenu = _interactable.mainController.currentVehicle.menu.archiveMenu;
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
        if (Purchase(_foodIcon.currentData.currentAmount) == false) return;

        // coin launch animation
        Transform player = _interactable.detection.player.transform;

        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], player.position);
    }


    private void Unlock()
    {
        if (_stockData.unlocked) return;

        if (_interactable.mainController.GoldenNugget_Amount() < _unlockPrice)
        {
            // Not enough golden nuggets to purchase!
            gameObject.GetComponent<DialogTrigger>().Update_Dialog(1);

            _interactable.UnInteract();
            return;
        }

        Toggle_Unlock(true);

        Update_Bubble();
        _interactable.UnInteract();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(FoodStock))]
public class FoodStock_Inspector : Editor
{
    //
    private SerializedProperty foodToAddProp;

    private void OnEnable()
    {
        foodToAddProp = serializedObject.FindProperty("foodToAdd");
    }


    //
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FoodStock foodStock = (FoodStock)target;
        serializedObject.Update();

        GUILayout.Space(60);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(foodToAddProp, GUIContent.none);
        Food_ScrObj foodToAdd = (Food_ScrObj)foodToAddProp.objectReferenceValue;

        if (GUILayout.Button("Assign Food"))
        {
            foodStock.Toggle_Unlock(true);
            foodStock.Set_FoodData(new(foodToAdd));
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Increase Amount"))
        {
            foodStock.Update_Amount(1);
        }

        if (GUILayout.Button("Decrease Amount"))
        {
            foodStock.Update_Amount(-1);
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif