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


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _amountBar.Set_Amount(ES3.Load("ScrapStack/currentAmount", _amountBar.currentAmount));
        _amountBar.Load();
        _amountBar.Transparent_Toggle(true);

        Update_CurrentSprite();

        // subscriptions
        _interactable.detection.EnterEvent += Toggle_AmountBar;
        _interactable.InteractEvent += Toggle_AmountBar;
        _interactable.UnInteractEvent += Toggle_AmountBar;

        _interactable.OnAction1Event += Stack_ExistingScrap;
        _interactable.OnAction2Event += Stack_BookmarkedScrap;
    }

    private void OnDestroy()
    {
        // subscriptions
        _interactable.detection.EnterEvent -= Toggle_AmountBar;
        _interactable.InteractEvent -= Toggle_AmountBar;
        _interactable.UnInteractEvent -= Toggle_AmountBar;

        _interactable.OnAction1Event -= Stack_ExistingScrap;
        _interactable.OnAction2Event -= Stack_BookmarkedScrap;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("ScrapStack/currentAmount", _amountBar.currentAmount);
    }

    public void Load_Data()
    {

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
            _amountBar.Transparent_Toggle(true);
            return;
        }

        _amountBar.Transparent_Toggle(_interactable.bubble.bubbleOn);
    }


    // Stack
    private void Stack_ExistingScrap()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_amountBar.Is_MaxAmount())
        {
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;

        ItemSlots_Controller slots = menu.controller.slotsController;
        List<ItemSlot_Data> scrapDatas = slots.Station_SlotDatas(menu.currentDatas, _scrap);

        if (scrapDatas.Count <= 0)
        {
            dialog.Update_Dialog(1);
            return;
        }

        scrapDatas[scrapDatas.Count - 1].Empty_Item();

        _amountBar.Update_Amount(1);
        _amountBar.Load();

        CoinLauncher playerLauncher = _interactable.detection.player.coinLauncher;
        playerLauncher.Parabola_CoinLaunch(_scrap.miniSprite, transform.position);

        Update_CurrentSprite();
    }

    private void Stack_BookmarkedScrap()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_amountBar.Is_MaxAmount())
        {
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;

        ItemSlots_Controller slots = menu.controller.slotsController;
        List<ItemSlot_Data> scrapDatas = slots.Station_SlotDatas(menu.currentDatas, _scrap, true);

        if (scrapDatas.Count <= 0)
        {
            dialog.Update_Dialog(2);
            return;
        }

        scrapDatas[scrapDatas.Count - 1].Empty_Item();

        _amountBar.Update_Amount(1);
        _amountBar.Load();

        CoinLauncher playerLauncher = _interactable.detection.player.coinLauncher;
        playerLauncher.Parabola_CoinLaunch(_scrap.miniSprite, transform.position);

        Update_CurrentSprite();
    }
}