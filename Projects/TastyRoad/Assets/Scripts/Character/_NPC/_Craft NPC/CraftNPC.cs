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
    [SerializeField] private AmountBar _nuggetBar;
    public AmountBar nuggetBar => _nuggetBar;

    [SerializeField] private AmountBar _giftBar;
    public AmountBar giftBar => _giftBar;


    private Action OnSave;


    // MonoBehaviour
    public void Start()
    {
        // amount bars
        _nuggetBar.Load();
        _giftBar.Load();

        Toggle_AmountBars();

        // starting movement
        SpriteRenderer roamArea = _npcController.mainController.currentLocation.data.roamArea;
        _npcController.movement.Free_Roam(roamArea, 0);

        // subscriptions
        Detection_Controller detection = _npcController.interactable.detection;

        detection.EnterEvent += Toggle_AmountBars;
        detection.ExitEvent += Toggle_AmountBars;

        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.OnIInteract += Face_Player;
        interactable.OnHoldIInteract += Face_Player;
    }

    public void OnDestroy()
    {
        // subscriptions
        Detection_Controller detection = _npcController.interactable.detection;

        detection.EnterEvent -= Toggle_AmountBars;
        detection.ExitEvent -= Toggle_AmountBars;

        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.OnIInteract -= Face_Player;
        interactable.OnHoldIInteract -= Face_Player;
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
    private void Toggle_AmountBars()
    {
        ActionBubble_Interactable interactable = _npcController.interactable;

        bool playerDetected = interactable.detection.player != null;
        bool bubbleOn = interactable.bubble.bubbleOn;

        GameObject amountBars = _nuggetBar.transform.parent.parent.gameObject;

        if (playerDetected == false || bubbleOn)
        {
            amountBars.SetActive(false);
            return;
        }

        amountBars.SetActive(true);

        _nuggetBar.Toggle(true);
        _giftBar.Toggle(true);
    }


    // Main Interactions
    private void Face_Player()
    {
        GameObject player = _npcController.interactable.detection.player.gameObject;

        _npcController.basicAnim.Flip_Sprite(player);

        NPC_Movement movement = _npcController.movement;
        SpriteRenderer roamArea = _npcController.mainController.currentLocation.data.roamArea;

        movement.Stop_FreeRoam();
        movement.Free_Roam(roamArea, 1f);
    }


    private void Pay()
    {
        /*
        Food_ScrObj nugget = _controller.controller.mainController.dataController.goldenNugget;
        FoodData_Controller playerIcon = _controller.controller.interactable.detection.player.foodIcon;

        if (playerIcon.Is_SameFood(nugget) == false) return;

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_AmountBar();
        playerIcon.Show_Condition();

        _controller.nuggetBar.Update_Amount(1);
        _controller.nuggetBar.Load();
        */
    }

    private void Gift()
    {
        /*
        Food_ScrObj nugget = _controller.controller.mainController.dataController.goldenNugget;
        FoodData_Controller playerIcon = _controller.controller.interactable.detection.player.foodIcon;

        if (playerIcon.hasFood == false) return;
        if (playerIcon.Is_SameFood(nugget)) return;

        FoodData playerFood = new(playerIcon.currentData);

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_AmountBar();
        playerIcon.Show_Condition();

        _controller.controller.foodIcon.Set_CurrentData(playerFood);

        _controller.timeBar.Update_Amount(1);
        _controller.timeBar.Load();
        */
    }
}
