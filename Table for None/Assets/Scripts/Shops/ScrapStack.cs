using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapStack : MonoBehaviour, ISaveLoadable
{
    private SpriteRenderer _sr;

    [SerializeField] private ActionBubble_Interactable _interactable;

    [SerializeField] private AmountBar _amountBar;
    public AmountBar amountBar => _amountBar;

    [Header("")]
    [SerializeField] private Station_ScrObj _scrap;

    [Header("")]
    [SerializeField] private Sprite[] _scrapSprites;
    
    
    [Space(60)]
    [SerializeField] private VideoGuide_Trigger _guideTrigger;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        Load_Data();
    }

    private void Start()
    {
        Toggle_AmountBar();
        Update_CurrentSprite();

        // subscriptions
        _interactable.OnInteract += _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.OnInteract += Toggle_AmountBar;
        _interactable.OnUnInteract += Toggle_AmountBar;

        _interactable.OnAction1 += Stack;
        _interactable.OnAction2 += Retrieve;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.OnInteract -= _guideTrigger.Trigger_CurrentGuide;
        
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.OnInteract -= Toggle_AmountBar;
        _interactable.OnUnInteract -= Toggle_AmountBar;

        _interactable.OnAction1 -= Stack;
        _interactable.OnAction2 -= Retrieve;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("ScrapStack/AmountBar/_currentAmount", _amountBar.currentAmount);
    }

    public void Load_Data()
    {
        _amountBar.Set_Amount(ES3.Load("ScrapStack/AmountBar/_currentAmount", _amountBar.currentAmount));
    }


    // Visual Control
    public void Update_CurrentSprite()
    {
        int spriteNum = Mathf.Clamp(_amountBar.currentAmount, 0, _scrapSprites.Length - 1);
        _sr.sprite = _scrapSprites[spriteNum];
    }

    public void Toggle_AmountBar()
    {
        if (_interactable.detection.player == null)
        {
            _amountBar.Toggle(false);
            return;
        }

        _amountBar.Toggle_BarColor(_amountBar.Is_MaxAmount());
        _amountBar.Load();

        _amountBar.Toggle(!_interactable.bubble.bubbleOn);
    }


    // Interactions
    private void Stack()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_amountBar.Is_MaxAmount())
        {
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = menu.controller.slotsController;

        int vehicleAmount = slotsController.StationAmount(menu.ItemSlot_Datas(), _scrap);

        if (vehicleAmount <= 0)
        {
            dialog.Update_Dialog(1);
            return;
        }

        int stackAmount = 0;
        
        for (int i = 0; i < vehicleAmount; i++)
        {
            menu.Remove_StationItem(_scrap);
            _amountBar.Update_Amount(1);

            stackAmount++;

            if (_amountBar.Is_MaxAmount()) break;
        }

        Toggle_AmountBar();
        Update_CurrentSprite();
        
        TutorialQuest_Controller.instance.Complete_Quest("ScrapStack", stackAmount);
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }

    private void Retrieve()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_amountBar.currentAmount <= 0)
        {
            dialog.Update_Dialog(2);
            return;
        }

        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = menu.controller.slotsController;

        if (slotsController.Empty_SlotData(menu.ItemSlot_Datas()) == null)
        {
            dialog.Update_Dialog(3);
            return;
        }

        int retrieveAmount = _amountBar.currentAmount;
        int retrievedAmount = 0;

        for (int i = 0; i < retrieveAmount; i++)
        {
            _amountBar.Update_Amount(-1);
            menu.Add_StationItem(_scrap, 1);
            
            retrievedAmount++;

            if (slotsController.Empty_SlotData(menu.ItemSlot_Datas()) == null) break;
        }

        Toggle_AmountBar();
        Update_CurrentSprite();
        
        TutorialQuest_Controller.instance.Complete_Quest("ScrapStack", -retrievedAmount);
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }
}