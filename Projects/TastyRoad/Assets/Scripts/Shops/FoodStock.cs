using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private AmountBar _amountBar;
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Sprite[] _sprites;

    [Header("")]
    [SerializeField] private SpriteRenderer _tagSR;
    [SerializeField] private Sprite[] _tagSprites;

    [Header("")]
    [Range(1, 98)] [SerializeField] private int _maxAmount;
    public int maxAmount => _maxAmount;

    [Range(1, 98)] [SerializeField] private int _discountPrice;

    private int _currentAmount;
    public int currentAmount => _currentAmount;

    private Food_ScrObj _currentFood;

    private bool _isDiscount;
    public bool isDiscount => _isDiscount;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Set_Data();

        Sprite_Update();
        TagSprite_Update();

        _interactable.LockUnInteract(true);

        _interactable.InteractEvent += Set_Dialog;
        _interactable.Action1Event += Purchase;
    }

    private void OnDestroy()
    {
        _interactable.InteractEvent -= Set_Dialog;
        _interactable.Action1Event -= Purchase;
    }


    // Set
    public void Set_Data()
    {
        _currentFood = _interactable.mainController.dataController.Food();

        // set amount
        Update_Amount(_maxAmount);

        // action bubble update
        _interactable.bubble.Update_Bubble(_currentFood, null);
    }


    public void Update_Data()
    {
        Data_Controller data = _interactable.mainController.dataController;

        if (data.Is_RawFood(_currentFood))
        {
            _currentFood = data.RawFood();
        }
        else
        {
            _currentFood = data.CookedFood();
        }

        // set amount
        Update_Amount(_maxAmount);

        // action bubble update
        _interactable.bubble.Update_Bubble(_currentFood, null);
    }

    public void Update_Discount(bool isDiscount)
    {
        _isDiscount = isDiscount;

        TagSprite_Update();
    }


    private void Set_Dialog()
    {
        // calculation
        int price = _currentFood.price;

        if (_isDiscount && price > 0)
        {
            price = _discountPrice;
        }

        string dialog = price + " nuggets to purchase.\nyour current nugget amount is " + _interactable.mainController.GoldenNugget_Amount();

        DialogData data = new DialogData(_currentFood.sprite, dialog);
        DialogBox dialogBox = gameObject.GetComponent<DialogTrigger>().Update_Dialog(data);

        dialogBox.UpdateIcon_CenterPosition(_currentFood.uiCenterPosition);
    }


    // Sprite Control
    private void Sprite_Update()
    {
        // cooked food
        if (_interactable.mainController.dataController.Is_RawFood(_currentFood) == false)
        {
            _sr.sprite = _sprites[1];
            return;
        }

        // raw food & default
        _sr.sprite = _sprites[0];
    }

    private void TagSprite_Update()
    {
        if (currentAmount <= 0)
        {
            _tagSR.sprite = _tagSprites[0];
            return;
        }

        if (_isDiscount == false)
        {
            _tagSR.sprite = _tagSprites[1];
            return;
        }

        _tagSR.sprite = _tagSprites[2];
    }


    // Amount Bar
    public void Update_Amount(int updateAmount)
    {
        _currentAmount += updateAmount;
        _amountBar.Load(_currentAmount);

        TagSprite_Update();
    }


    // Functions
    private void Purchase()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // not enough amount
        if (_currentAmount <= 0)
        {
            _interactable.UnInteract();

            DialogData data = new DialogData(_currentFood.sprite, "Not enough amount available!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(_currentFood.uiCenterPosition);

            return;
        }

        // calculation
        int price = _currentFood.price;

        if (_isDiscount && price > 0)
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
        if (foodMenu.Add_FoodItem(_currentFood, 1) > 0)
        {
            _interactable.UnInteract();

            // return golden nuggets
            _interactable.mainController.Add_GoldenNugget(_currentFood.price);

            DialogData data = new DialogData(_currentFood.sprite, "Not enough space in food storage!");
            dialog.Update_Dialog(data).UpdateIcon_CenterPosition(_currentFood.uiCenterPosition);

            return;
        }

        //
        Update_Amount(-1);
        TagSprite_Update();

        // archive menu unlocks
        ArchiveMenu_Controller archiveMenu = _interactable.mainController.currentVehicle.menu.archiveMenu;

        archiveMenu.Archive_Food(_currentFood);
        // archiveMenu.UnLock_BookMark(_currentFood);

        // coin launch animation
        _launcher.Parabola_CoinLaunch(_currentFood.sprite, _interactable.detection.player.transform.position);
    }
}
