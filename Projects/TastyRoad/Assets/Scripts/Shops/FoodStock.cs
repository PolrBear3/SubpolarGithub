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
    [Range(1, 98)][SerializeField] private int _maxAmount;
    public int maxAmount => _maxAmount;

    [Range(1, 98)][SerializeField] private int _discountPrice;


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
        Update_Sprite();
        Update_Bubble();

        // subscriptions
        _interactable.InteractEvent += Set_Dialog;
        _interactable.InteractEvent += Toggle_AmountBar;
        _interactable.UnInteractEvent += Toggle_AmountBar;

        _interactable.Action1Event += Unlock;
        _interactable.Action1Event += Purchase_Single;
        _interactable.Action2Event += Purchase_All;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.InteractEvent -= Set_Dialog;
        _interactable.InteractEvent -= Toggle_AmountBar;
        _interactable.UnInteractEvent -= Toggle_AmountBar;

        _interactable.Action1Event -= Unlock;
        _interactable.Action1Event -= Purchase_Single;
        _interactable.Action2Event -= Purchase_All;
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
        _foodIcon.Toggle_AmountBar(!_interactable.bubble.bubbleOn);

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
        FoodData foodData = _foodIcon.currentData;

        if (foodData == null || foodData.currentAmount <= 0)
        {
            _foodIcon.Toggle_AmountBar(false);
        }

        _foodIcon.Toggle_AmountBar(!_interactable.bubble.bubbleOn);
    }


    private void Set_Dialog()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_stockData.unlocked == false)
        {
            // dialog
            return;
        }

        if (_foodIcon.currentData == null || _foodIcon.currentData.currentAmount <= 0)
        {
            // dialog

            interactable.UnInteract();
            return;
        }

        Food_ScrObj currentFood = _foodIcon.currentData.foodScrObj;

        // calculation
        int price = _foodIcon.currentData.foodScrObj.price;

        if (_stockData.isDiscount && price > 0)
        {
            price = _discountPrice;
        }

        string priceString = price + " nuggets to purchase.\nyour current nugget amount is " + _interactable.mainController.GoldenNugget_Amount();

        DialogData data = new(currentFood.sprite, priceString);
        DialogBox dialogBox = dialog.Update_Dialog(data);

        dialogBox.UpdateIcon_CenterPosition(currentFood.uiCenterPosition);
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

        if (_foodIcon.hasFood == false || _foodIcon.currentData.currentAmount <= 0)
        {
            _tagSR.sprite = _tagSprites[2];
            return;
        }

        if (_stockData.isDiscount == false)
        {
            _tagSR.sprite = _tagSprites[0];
            return;
        }

        _tagSR.sprite = _tagSprites[1];
    }


    // Functions
    private void Purchase(int purchaseAmount)
    {
        if (_stockData.unlocked == false || _foodIcon.hasFood == false) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;
        int stockedAmount = _foodIcon.currentData.currentAmount;

        // not enough amount
        if (stockedAmount <= 0)
        {
            _interactable.UnInteract();

            DialogData data = new DialogData(stockedFood.sprite, "Not enough amount available!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(stockedFood.uiCenterPosition);

            return;
        }

        // calculation
        int price = stockedFood.price * purchaseAmount;

        if (_stockData.isDiscount && price > 0)
        {
            price = _discountPrice * purchaseAmount;
        }

        // not enough golden nuggets
        if (_interactable.mainController.GoldenNugget_Amount() < price)
        {
            _interactable.UnInteract();

            Food_ScrObj goldenNugget = _interactable.mainController.dataController.goldenNugget;

            DialogData data = new DialogData(goldenNugget.sprite, "Not enough golden nuggets to purchase!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(goldenNugget.uiCenterPosition);

            return;
        }

        _interactable.mainController.Remove_GoldenNugget(price);

        // not enough slots
        FoodMenu_Controller foodMenu = _interactable.mainController.currentVehicle.menu.foodMenu;
        if (foodMenu.Add_FoodItem(stockedFood, purchaseAmount) > 0)
        {
            _interactable.UnInteract();

            // return golden nuggets
            _interactable.mainController.Add_GoldenNugget(stockedFood.price);

            DialogData data = new DialogData(stockedFood.sprite, "Not enough space in food storage!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(stockedFood.uiCenterPosition);

            return;
        }

        //
        Update_Amount(-purchaseAmount);
        Update_TagSprite();

        // add purchased food to archive
        ArchiveMenu_Controller archiveMenu = _interactable.mainController.currentVehicle.menu.archiveMenu;
        archiveMenu.Archive_Food(stockedFood);

        // coin launch animation
        _launcher.Parabola_CoinLaunch(stockedFood.sprite, _interactable.detection.player.transform.position);
    }

    private void Purchase_Single()
    {
        Purchase(1);
    }

    private void Purchase_All()
    {
        Purchase(_foodIcon.currentData.currentAmount);
    }


    private void Unlock()
    {
        if (_stockData.unlocked) return;

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