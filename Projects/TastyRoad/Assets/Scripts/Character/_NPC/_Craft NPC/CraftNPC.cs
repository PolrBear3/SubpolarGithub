using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC : MonoBehaviour
{
    private CraftNPC_Controller _controller;
    public CraftNPC_Controller controller => _controller;


    [Header("")]
    [SerializeField] private NPC_Controller _npcController;
    public NPC_Controller npcController => _npcController;


    [Header("")]
    [SerializeField] private Clock_Timer _actionTimer;
    public Clock_Timer actionTimer => _actionTimer;

    [SerializeField] private AmountBar _nuggetBar;
    public AmountBar nuggetBar => _nuggetBar;

    [SerializeField] private AmountBar _giftBar;
    public AmountBar giftBar => _giftBar;


    private Action OnSave;

    private Coroutine _coroutine;
    public Coroutine coroutine;


    // MonoBehaviour
    public void Start()
    {
        // amount bars
        _nuggetBar.Toggle_BarColor(_nuggetBar.Is_MaxAmount());
        _nuggetBar.Load();

        _giftBar.Set_Amount(_npcController.foodIcon.AllDatas().Count);
        _giftBar.Load();

        Toggle_AmountBars();

        // starting movement
        _npcController.movement.Free_Roam(0);

        // subscriptions
        Detection_Controller detection = _npcController.interactable.detection;

        detection.EnterEvent += Toggle_AmountBars;
        detection.ExitEvent += Toggle_AmountBars;

        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.OnHoldIInteract += Pay;
        interactable.OnHoldIInteract += Gift;
    }

    public void OnDestroy()
    {
        // subscriptions
        Detection_Controller detection = _npcController.interactable.detection;

        detection.EnterEvent -= Toggle_AmountBars;
        detection.ExitEvent -= Toggle_AmountBars;

        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.OnHoldIInteract -= Pay;
        interactable.OnHoldIInteract -= Gift;
    }


    // Data
    public Coroutine Set_Coroutine(Coroutine coroutine)
    {
        _coroutine = coroutine;
        return _coroutine;
    }


    // OnSave Action
    public void Subscribe_OnSave(Action action)
    {
        OnSave += action;
    }

    public void Invoke_OnSave()
    {
        OnSave?.Invoke();
    }


    // Toggles
    public void Toggle_AmountBars()
    {
        ActionBubble_Interactable interactable = _npcController.interactable;

        bool playerDetected = interactable.detection.player != null;
        bool bubbleOn = interactable.bubble.bubbleOn;

        GameObject amountBars = _nuggetBar.transform.parent.parent.gameObject;

        if (_coroutine != null || playerDetected == false || bubbleOn)
        {
            amountBars.SetActive(false);
            return;
        }

        amountBars.SetActive(true);

        _nuggetBar.Toggle(true);
        _giftBar.Toggle(true);
    }

    public DialogData Current_PayCount()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        string holdToPay = "Hold <sprite=15> to pay <sprite=56>\n";
        string currentPayCount = _nuggetBar.currentAmount + "/" + _nuggetBar.maxAmount + " <sprite=56> currently payed";

        return new DialogData(dialog.defaultData.icon, holdToPay + currentPayCount);
    }


    // Main Interactions
    private void Pay()
    {
        Food_ScrObj nugget = _npcController.mainController.dataController.goldenNugget;
        FoodData_Controller playerIcon = _npcController.interactable.detection.player.foodIcon;

        if (playerIcon.Is_SameFood(nugget) == false) return;

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        _nuggetBar.Update_Amount(1);
        _nuggetBar.Toggle_BarColor(_nuggetBar.Is_MaxAmount());
        _nuggetBar.Load();

        gameObject.GetComponent<DialogTrigger>().Update_Dialog(Current_PayCount());
    }

    private void Gift()
    {
        FoodData_Controller foodIcon = _npcController.foodIcon;

        if (foodIcon.DataCount_Maxed()) return;

        Food_ScrObj nugget = _npcController.mainController.dataController.goldenNugget;
        FoodData_Controller playerIcon = _npcController.interactable.detection.player.foodIcon;

        if (playerIcon.hasFood == false) return;
        if (playerIcon.Is_SameFood(nugget)) return;

        FoodData playerFood = new(playerIcon.currentData);

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        foodIcon.Set_CurrentData(playerFood);
        _giftBar.Load_Custom(foodIcon.maxDataCount, foodIcon.AllDatas().Count);
    }
}