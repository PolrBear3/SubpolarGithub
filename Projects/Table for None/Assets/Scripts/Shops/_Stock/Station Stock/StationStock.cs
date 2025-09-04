using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationStock : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Space(20)]
    [SerializeField] private DialogTrigger _dialog;

    [SerializeField] private ActionBubble_Interactable _interactable;
    [SerializeField] private CoinLauncher _launcher;

    [Space(20)]
    [SerializeField] private Sprite _emptyStation;

    [Space(20)]
    [SerializeField] private SpriteRenderer _statusSign;
    [SerializeField] private Sprite[] _signSprites;

    [Space(20)]
    [Range(0, 100)][SerializeField] private int _discountPercentage;
    
    [Space(40)] 
    [SerializeField] private TutorialQuest_ScrObj _stationPurchaseQuest;


    private StationStock_Data _data;
    public StationStock_Data data => _data;
    

    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        if (_data != null) return;
        _data = new(false);
    }

    private void Start()
    {
        Restock(_data.stationData);
        Toggle_Discount(_data.discountData.isDiscount);

        // subscriptions
        _interactable.OnInteract += Toggle_Price;

        _interactable.OnAction1 += Purchase;
        _interactable.OnAction2 += Toggle_Discount;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteract -= Toggle_Price;

        _interactable.OnAction1 -= Purchase;
        _interactable.OnAction2 -= Toggle_Discount;
    }
    
    
    // Data
    public void Set_Data(StationStock_Data data)
    {
        if (_data == null)
        {
            _data = new(false);
            return;
        }
        _data = data;
    }
    

    // Functions
    private int Price()
    {
        if (_data.Station_Sold()) return 0;
        
        int price = _data.stationData.amount;
        if (price <= 0) return 0;
        
        if (_data.discountData.isDiscount && price > 0)
        {
            float discountValue = 1f - (_discountPercentage / 100f);
            price = Mathf.RoundToInt(price * discountValue);
        }

        return price;
    }

    private void Toggle_Price()
    {
        if (_data.Station_Sold()) return;

        Sprite stationicon = _data.stationData.stationScrObj.dialogIcon;
        GoldSystem_TriggerData triggerData = new(stationicon, -Price());
        
        GoldSystem.instance.Indicate_TriggerData(triggerData);
    }


    private void Purchase()
    {
        if (_data.Station_Sold())
        {
            _dialog.Update_Dialog(1);
            return;
        }

        StationMenu_Controller stationMenu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = stationMenu.controller.slotsController;
        
        bool slotFull = slotsController.Empty_SlotData(stationMenu.ItemSlot_Datas()) == null;

        if (slotFull)
        {
            // Not Enough space in vehicle storage!
            _dialog.Update_Dialog(0);
            return;
        }
        
        Station_ScrObj currentStation = _data.stationData.stationScrObj;

        // pay calculation
        if (GoldSystem.instance.Update_CurrentAmount(-Price()) == false) return;

        // add to vehicle
        stationMenu.Add_StationItem(currentStation, 1);

        // station coin launch animation
        _launcher.Parabola_CoinLaunch(currentStation.dialogIcon, _interactable.detection.player.transform.position);

        // tutorial quest
        TutorialQuest_Controller.instance.Complete_Quest(_stationPurchaseQuest, 1);
        
        Update_toSold();
        Toggle_Discount(false);
    }

    public void Update_toSold()
    {
        // set data
        _data = new(false);

        // set sprite
        _sr.sprite = _emptyStation;
        _sr.sortingLayerName = "Background";
        _sr.sortingOrder = 1;

        _interactable.bubble.Toggle_Height(true);

        if (_data.discountData.isDiscount) return;
        _statusSign.sprite = _signSprites[2];
    }


    private void Toggle_Discount()
    {
        if (_data.Station_Sold() == false)
        {
            _dialog.Update_Dialog(2);
            return;
        }

        StockData discountData = _data.discountData;
        _data.discountData.Toggle_Discount(!discountData.isDiscount);

        if (discountData.isDiscount)
        {
            _statusSign.sprite = _signSprites[1];

            _dialog.Update_Dialog(3);
            return;
        }
        _dialog.Update_Dialog(4);

        if (_data.Station_Sold())
        {
            _statusSign.sprite = _signSprites[2];
            return;
        }
        _statusSign.sprite = _signSprites[0];
    }
    public void Toggle_Discount(bool toggleOn)
    {
        StockData discountData = _data.discountData;
        discountData.Toggle_Discount(toggleOn);

        if (discountData.isDiscount)
        {
            _statusSign.sprite = _signSprites[1];
            return;
        }

        _statusSign.sprite = _data.Station_Sold() ? _signSprites[2] : _signSprites[0];
    }


    public void Restock(StationData restockData)
    {
        if (restockData == null || restockData.stationScrObj == null) return;
        
        // set data
        _data.Set_StationData(new(restockData));

        // set sprite
        _sr.sprite = _data.stationData.stationScrObj.sprite;
        _sr.sortingLayerName = "Prefabs";
        _sr.sortingOrder = 0;

        _interactable.bubble.Toggle_Height(false);

        // tag sprite
        if (_data.discountData.isDiscount) return;
        _statusSign.sprite = _signSprites[0];
    }
}
