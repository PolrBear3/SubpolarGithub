using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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


    private StockData _data;
    public StockData data => _data;


    // Editor
    [HideInInspector] public Food_ScrObj foodToAdd;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // set these to loaded data //
        _data = new();
        Set_Data(null);
        //

        Sprite_Update();
        Toggle_Unlock(_data.unlocked);

        _interactable.InteractEvent += Set_Dialog;
        _interactable.Action1Event += Purchase;
    }

    private void OnDestroy()
    {
        _interactable.InteractEvent -= Set_Dialog;
        _interactable.Action1Event -= Purchase;
    }


    // Data Control
    public void Set_Data(Food_ScrObj setFood)
    {
        _foodIcon.Set_CurrentData(new(setFood));

        _foodIcon.currentData.Set_Amount(0);
        _foodIcon.Show_AmountBar();

        if (_foodIcon.currentData.foodScrObj == null) return;

        _foodIcon.Show_Icon();

        // action bubble update
        _interactable.bubble.Update_Bubble(_foodIcon.currentData.foodScrObj, null);
    }

    public void Update_Amount(int updateAmount)
    {
        _foodIcon.currentData.Update_Amount(updateAmount);
        _foodIcon.Show_AmountBar();

        TagSprite_Update();
    }


    public void Toggle_Unlock(bool toggle)
    {
        _data.Toggle_UnLock(toggle);

        Sprite_Update();
        TagSprite_Update();

        _interactable.LockInteract(!toggle);
    }

    public void Toggle_Discount(bool toggle)
    {
        _data.Toggle_Discount(toggle);

        TagSprite_Update();
    }


    private void Set_Dialog()
    {
        Food_ScrObj currentFood = _foodIcon.currentData.foodScrObj;

        // calculation
        int price = _foodIcon.currentData.foodScrObj.price;

        if (_data.isDiscount && price > 0)
        {
            price = _discountPrice;
        }

        string dialog = price + " nuggets to purchase.\nyour current nugget amount is " + _interactable.mainController.GoldenNugget_Amount();

        DialogData data = new DialogData(currentFood.sprite, dialog);
        DialogBox dialogBox = gameObject.GetComponent<DialogTrigger>().Update_Dialog(data);

        dialogBox.UpdateIcon_CenterPosition(currentFood.uiCenterPosition);
    }


    // Sprite Control
    private void Sprite_Update()
    {
        if (_data.unlocked == false)
        {
            _sr.sprite = _sprites[0];
            return;
        }

        // cooked food
        if (_interactable.mainController.dataController.Is_RawFood(_foodIcon.currentData.foodScrObj) == false)
        {
            _sr.sprite = _sprites[2];
            return;
        }

        // raw food & default
        _sr.sprite = _sprites[1];
    }

    private void TagSprite_Update()
    {
        if (_data.unlocked == false)
        {
            _tagSR.color = Color.clear;
            return;
        }

        _tagSR.color = Color.white;

        if (_foodIcon.currentData.currentAmount <= 0)
        {
            _tagSR.sprite = _tagSprites[2];
            return;
        }

        if (_data.isDiscount == false)
        {
            _tagSR.sprite = _tagSprites[0];
            return;
        }

        _tagSR.sprite = _tagSprites[1];
    }


    // Functions
    private void Purchase()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        Food_ScrObj stockedFood = _foodIcon.currentData.foodScrObj;

        // not enough amount
        if (_foodIcon.currentData.currentAmount <= 0)
        {
            _interactable.UnInteract();

            DialogData data = new DialogData(stockedFood.sprite, "Not enough amount available!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(stockedFood.uiCenterPosition);

            return;
        }

        // calculation
        int price = stockedFood.price;

        if (_data.isDiscount && price > 0)
        {
            price = _discountPrice;
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
        if (foodMenu.Add_FoodItem(stockedFood, 1) > 0)
        {
            _interactable.UnInteract();

            // return golden nuggets
            _interactable.mainController.Add_GoldenNugget(stockedFood.price);

            DialogData data = new DialogData(stockedFood.sprite, "Not enough space in food storage!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(stockedFood.uiCenterPosition);

            return;
        }

        //
        Update_Amount(-1);
        TagSprite_Update();

        // add purchased food to archive
        ArchiveMenu_Controller archiveMenu = _interactable.mainController.currentVehicle.menu.archiveMenu;
        archiveMenu.Archive_Food(stockedFood);

        // coin launch animation
        _launcher.Parabola_CoinLaunch(stockedFood.sprite, _interactable.detection.player.transform.position);
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
            foodStock.Set_Data(foodToAdd);
            foodStock.Toggle_Unlock(true);
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