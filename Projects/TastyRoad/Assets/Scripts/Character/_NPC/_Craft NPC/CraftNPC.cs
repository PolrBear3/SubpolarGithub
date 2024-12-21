using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private CraftNPC_Controller _controller;
    public CraftNPC_Controller controller => _controller;


    [Header("")]
    [SerializeField] private AnimatorOverrideController _animOverride;

    [Header("")]
    [SerializeField] private UnityEvent OnSetInstance;
    [SerializeField] private UnityEvent OnSaveInstance;


    // MonoBehaviour
    public void Start()
    {
        Toggle_AmountBars();

        NPC_Movement movement = _controller.controller.movement;
        SpriteRenderer roamArea = _controller.controller.mainController.currentLocation.data.roamArea;

        Load_SpawnLocation();
        movement.Free_Roam(roamArea, 1f);

        // subscriptions
        WorldMap_Controller.NewLocation_Event += Set_SpawnLocation;

        Detection_Controller detection = _controller.controller.interactable.detection;

        detection.EnterEvent += Toggle_AmountBars;
        detection.ExitEvent += Toggle_AmountBars;
    }

    private void OnDestroy()
    {
        // subscriptions
        WorldMap_Controller.NewLocation_Event -= Set_SpawnLocation;

        Detection_Controller detection = _controller.controller.interactable.detection;

        detection.EnterEvent -= Toggle_AmountBars;
        detection.ExitEvent -= Toggle_AmountBars;

        // default subscriptions
        ActionBubble_Interactable interactable = _controller.controller.interactable;

        interactable.OnHoldIInteract -= Pay;
        interactable.OnHoldIInteract -= Gift;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        Vector3 savePos = new(NPC_Transform().position.x, NPC_Transform().position.y);
        ES3.Save("CraftNPC/position", savePos);

        Invoke_OnSaveInstance();
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("CraftNPC/position") == false) return;
        NPC_Transform().position = ES3.Load("CraftNPC/position", NPC_Transform().position);
    }


    // Instance Data
    public void SetInstance_CurrentNPC()
    {
        NPC_Controller npc = _controller.controller;

        npc.basicAnim.Set_OverrideController(_animOverride);
        OnSetInstance?.Invoke();

        // default subscriptions
        ActionBubble_Interactable interactable = _controller.controller.interactable;

        interactable.OnHoldIInteract += Pay;
        interactable.OnHoldIInteract += Gift;
    }

    public void Invoke_OnSaveInstance()
    {
        OnSaveInstance?.Invoke();
    }


    // Spawn
    public Transform NPC_Transform()
    {
        return transform.parent.parent;
    }


    private void Set_SpawnLocation()
    {
        Location_Controller location = _controller.controller.mainController.currentLocation;
        NPC_Transform().position = location.OuterLocation_Position(-1);
    }

    private void Load_SpawnLocation()
    {
        if (ES3.KeyExists("CraftNPC/position")) return;
        Set_SpawnLocation();
    }


    // Toggles
    private void Toggle_AmountBars()
    {
        ActionBubble_Interactable interactable = _controller.controller.interactable;

        bool playerDetected = interactable.detection.player != null;
        bool bubbleOn = interactable.bubble.bubbleOn;

        GameObject amountBars = _controller.nuggetBar.transform.parent.parent.gameObject;

        if (playerDetected == false || bubbleOn)
        {
            amountBars.SetActive(false);
            return;
        }

        amountBars.SetActive(true);

        _controller.nuggetBar.Toggle(true);
        _controller.timeBar.Toggle(true);
    }


    // Main Interactions
    private void Pay()
    {
        Food_ScrObj nugget = _controller.controller.mainController.dataController.goldenNugget;
        FoodData_Controller playerIcon = _controller.controller.interactable.detection.player.foodIcon;

        if (playerIcon.Is_SameFood(nugget) == false) return;

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_AmountBar();
        playerIcon.Show_Condition();

        _controller.nuggetBar.Update_Amount(1);
        _controller.nuggetBar.Load();
    }

    private void Gift()
    {
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
    }
}
