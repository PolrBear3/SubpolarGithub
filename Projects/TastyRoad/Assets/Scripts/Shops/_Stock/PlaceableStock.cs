using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private FoodData_Controller _previewIcon;
    public FoodData_Controller previewIcon => _previewIcon;

    [SerializeField] private GameObject _paymentIcon;

    [Header("")]
    [SerializeField] private Sprite[] _sprites;

    [Header("")]
    [SerializeField][Range(1, 98)] private int _maxAmount;


    private StockData _data;
    public StockData data => _data;
    
    private PurchaseData _purchaseData;
    public PurchaseData purchaseData => _purchaseData;
    
    
    [Space(60)]
    [SerializeField] private VideoGuide_Trigger _guideTrigger;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Update_forComplete();
        Update_PreviewIcon();

        // subscriptions
        _interactable.OnInteract += _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.detection.ExitEvent += Toggle_AmountBar;

        _interactable.detection.EnterEvent += Collect_Payment;
        _interactable.detection.ExitEvent += Collect_Payment;

        _interactable.OnInteract += Toggle_AmountBar;
        _interactable.OnUnInteract += Toggle_AmountBar;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteract -= _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;

        _interactable.detection.EnterEvent -= Collect_Payment;
        _interactable.detection.ExitEvent -= Collect_Payment;

        _interactable.OnInteract -= Toggle_AmountBar;
        _interactable.OnUnInteract -= Toggle_AmountBar;

        _interactable.OnAction1 -= Complete;
        _interactable.OnAction1 -= Place;
    }


    // Data Control
    private void Load_Data()
    {
        Toggle_PurchaseState(_purchaseData != null && _purchaseData.purchased);
        
        if (_data != null) return;
        _data = new(null);

        Update_Bubble();

        if (_purchaseData != null) return;
        Set_PurchaseData(new(0));
    }
    public void Load_Data(List<FoodData> placedData, StockData stockData)
    {
        if (placedData == null || placedData.Count <= 0) return;

        _foodIcon.Update_AllDatas(placedData);
        _data = new(stockData);
        
        Update_Bubble();
    }
    
    public void Reset_Data()
    {
        _foodIcon.Update_AllDatas(null);
        _foodIcon.Show_Icon();

        _data.Toggle_Complete(false);
        Update_forComplete();

        _interactable.LockInteract(false);

        Update_Bubble();
        Toggle_AmountBar();
    }
    
    public void Set_PurchaseData(PurchaseData purchaseData)
    {
        if (purchaseData == null)
        {
            _purchaseData = new(0);
            return;
        }
        
        _purchaseData = purchaseData;
    }


    // Updates and Toggles
    public void Update_forComplete()
    {
        Load_Data();

        _foodIcon.Toggle_Height(_data.isComplete);
        _foodIcon.Hide_Condition();
        
        _interactable.bubble.Toggle_Height(_data.isComplete);
        _interactable.LockInteract(_data.isComplete);

        if (_data.isComplete == false)
        {
            _sr.sprite = _sprites[0];
            _foodIcon.ShowIcon_LockToggle(false);

            _foodIcon.Toggle_Height(false);

            return;
        }

        _sr.sprite = _sprites[1];
        _foodIcon.Hide_Icon();
    }
    
    
    public void Toggle_PurchaseState(bool toggle)
    {
        bool hasPayment = _purchaseData != null && _purchaseData.price > 0;
        _paymentIcon.SetActive(toggle && hasPayment);
        
        if (_purchaseData == null) return;
        _purchaseData.Toggle_PurchaseState(toggle);
    }

    private void Update_PreviewIcon()
    {
        if (_paymentIcon.activeSelf || _previewIcon.hasFood == false)
        {
            _previewIcon.Hide_Icon();
            return;
        }
        previewIcon.Show_Icon(0.5f);
    }


    private void Toggle_AmountBar()
    {
        bool playerDetected = _interactable.detection.player != null;
        bool bubbleOn = _interactable.bubble.bubbleOn;

        bool foodPlaced = _foodIcon.hasFood;
        
        _foodIcon.Toggle_SubDataBar(foodPlaced && playerDetected && !bubbleOn);
        _previewIcon.amountBar.Toggle(!foodPlaced && playerDetected && !bubbleOn);
    }

    private void Update_Bubble()
    {
        Action_Bubble bubble = _interactable.bubble;
        bubble.Empty_Bubble();
        
        _interactable.OnAction1 = null;
        _interactable.OnAction2 = null;

        if (_foodIcon.hasFood == false)
        {
            bubble.Set_Bubble(bubble.setSprites[0], null);

            _interactable.OnAction1 += Place;
            return;
        }

        if (_foodIcon.DataCount_Maxed())
        {
            bubble.Set_Bubble(bubble.setSprites[1], null);

            _interactable.OnAction1 += Complete;
            return;
        }

        bubble.Set_Bubble(bubble.setSprites[0], bubble.setSprites[1]);

        _interactable.OnAction1 += Place;
        _interactable.OnAction2 += Complete;
    }


    // Functions
    private bool Place_Available()
    {
        if (_data.isComplete || _foodIcon.DataCount_Maxed()) return false;
        
        Player_Controller player = _interactable.detection.player;
        if (player == null) return false;

        FoodData_Controller playerIcon = player.foodIcon;
        if (playerIcon.hasFood == false) return false;

        return true;
    }

    private void Place()
    {
        if (Place_Available() == false) return;
        if (!_foodIcon.hasFood) _foodIcon.Update_AllDatas(null);

        _previewIcon.Update_AllDatas(null);
        _previewIcon.Show_Icon();

        FoodData_Controller playerIcon = _interactable.detection.player.foodIcon;

        // add data to _placedFoods
        _foodIcon.Set_CurrentData(playerIcon.currentData);
        _foodIcon.Show_Icon();
        _foodIcon.Toggle_SubDataBar(true);

        // empty player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        Update_Bubble();
    }


    private void Complete()
    {
        if (!foodIcon.hasFood) return;

        _data.Toggle_Complete(true);
        Update_forComplete();
        
        Update_Bubble();
        
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(0);
    }
    
    private void Collect_Payment()
    {
        if (_purchaseData.purchased == false) return;
        
        Toggle_PurchaseState(false);
        Update_PreviewIcon();

        if (_purchaseData.price <= 0) return;
        
        int paymentAmount = _purchaseData.price;
        GoldSystem.instance.Update_CurrentAmount(paymentAmount);

        Transform player = _interactable.detection.player.transform;
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], player.position);
    }
}
