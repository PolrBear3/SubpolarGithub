using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Space(20)] 
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private CoinLauncher _launcher;

    [Space(20)] 
    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private FoodData_Controller _previewIcon;
    public FoodData_Controller previewIcon => _previewIcon;

    [SerializeField] private GameObject _paymentIcon;

    [Space(20)] 
    [SerializeField] private Sprite[] _sprites;
    
    [Space(60)]
    [SerializeField] private VideoGuide_Trigger _guideTrigger;
    [SerializeField] private Ability_ScrObj _goldMagnetAbility;


    private FoodStock_Data _data;
    public FoodStock_Data data => _data;



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        
        if (_data != null) return;
        _data = new FoodStock_Data(new StockData(false));

        if (_data.stockPurchaseData != null) return;
        _data.Set_PurchaseData(new(false));
    }

    private void Start()
    {
        _foodIcon.Show_Icon();
        
        Toggle_AmountBar();
        Toggle_PurchaseState(_data.Stock_Purchased());
        
        Update_BubbleActions();
        Update_forComplete();
        Update_PreviewIcon();

        // subscriptions
        _interactable.OnInteract += _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.detection.ExitEvent += Toggle_AmountBar;

        _interactable.OnInteract += Toggle_AmountBar;
        _interactable.OnUnInteract += Toggle_AmountBar;

        _interactable.OnInteract += Collect_Payment;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteract -= _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.detection.ExitEvent -= Toggle_AmountBar;

        _interactable.OnInteract -= Toggle_AmountBar;
        _interactable.OnUnInteract -= Toggle_AmountBar;
        
        _interactable.OnInteract -= Collect_Payment;

        _interactable.OnAction1 -= Complete;
        _interactable.OnAction1 -= Place;
    }


    // Data Control
    public void Set_Data(FoodStock_Data data)
    {
        if (data == null) return;
        _data = data;

        _foodIcon.Update_AllDatas(_data.stockedFoodDatas);
        _data.Set_StockedFoodData(_foodIcon.AllDatas());
    }
    
    public void Reset_Data()
    {
        _data.stockedFoodDatas.Clear();
        
        _foodIcon.Update_AllDatas(null);
        _foodIcon.Show_Icon();

        _data.stockData.Toggle_Complete(false);
        
        Update_forComplete();
        Update_BubbleActions();
        
        Toggle_AmountBar();
    }


    // Updates and Toggles
    public void Update_forComplete()
    {
        bool bundleComplete = _data.stockData.isComplete;
        
        _foodIcon.Toggle_Height(bundleComplete);
        _foodIcon.Hide_Condition();
        
        _interactable.bubble.Toggle_Height(bundleComplete);
        _interactable.LockInteract(bundleComplete);

        if (bundleComplete == false)
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
        PurchaseData purchaseData = _data.stockPurchaseData;
        purchaseData.Toggle_PurchaseState(toggle);
        
        bool hasPayment = _data.Stock_Purchased() && purchaseData.price > 0;
        _paymentIcon.SetActive(toggle && hasPayment);
    }

    public void Update_PreviewIcon()
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
        _previewIcon.amountBar.Toggle(_previewIcon.hasFood && !foodPlaced && playerDetected && !bubbleOn);
    }

    private void Update_BubbleActions()
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
        if (_data.stockData.isComplete || _foodIcon.DataCount_Maxed()) return false;
        
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

        _data.stockedFoodDatas.Add(new(playerIcon.currentData));
        
        // add data to _placedFoods
        _foodIcon.Set_CurrentData(playerIcon.currentData);
        _foodIcon.Show_Icon();
        _foodIcon.Toggle_SubDataBar(true);

        // empty player food
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        Update_BubbleActions();
    }


    private void Complete()
    {
        if (!foodIcon.hasFood) return;
        
        _data.stockData.Toggle_Complete(true);
        
        Update_forComplete();
        Update_BubbleActions();
        
        Complete_BundleQuest();
        
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(0);
    }

    private void Complete_BundleQuest()
    {
        List<FoodData> foodDatas = _foodIcon.AllDatas();

        foreach (FoodData data in foodDatas)
        {
            TutorialQuest_Controller.instance.Complete_Quest("CompleteBundle" + data.foodScrObj.name, 1);
        }
    }
    
    
    private void Collect_Payment()
    {
        if (_data.Stock_Purchased() == false) return;
        
        Toggle_PurchaseState(false);
        Update_PreviewIcon();

        PurchaseData stockPurchaseData = _data.stockPurchaseData;
        if (stockPurchaseData.price <= 0) return;
        
        int paymentAmount = stockPurchaseData.price;
        GoldSystem.instance.Update_CurrentAmount(paymentAmount);

        Transform player = _interactable.detection.player.transform;
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], player.position);
    }
}
