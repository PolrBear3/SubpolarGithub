using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC : MonoBehaviour
{
    private CraftNPC_Controller _controller;
    public CraftNPC_Controller controller => _controller;


    [Space(20)]
    [SerializeField] private NPC_Controller _npcController;
    public NPC_Controller npcController => _npcController;

    [SerializeField] private DialogTrigger _dialog;
    public DialogTrigger dialog => _dialog;
    
    [Space(20)]
    [SerializeField] private Sprite _npcIconSprite;
    public Sprite npcIconSprite => _npcIconSprite;

    [SerializeField] private Sprite _paymentIconSprite;

    [Space(10)]
    [SerializeField] private SpriteRenderer _statusIcon;

    [Space(20)]
    [SerializeField][Range(0, 1000)] private int _defalutPrice;
    public int defaultPrice => _defalutPrice;

    [SerializeField][Range(0, 100)] private float _upgradeTimeValue;
    public float upgradeTimeValue => _upgradeTimeValue;

    
    private Sprite _defaultSprite;

    private CraftNPC_Data _data;
    public CraftNPC_Data data => _data;

    private PurchaseData _purchaseData;
    public PurchaseData purchaseData => _purchaseData;
    
    private Action _OnSave;
    
    private Coroutine _coroutine;
    public Coroutine coroutine;


    // MonoBehaviour
    public void Awake()
    {
        _defaultSprite = _statusIcon.sprite;
    }

    public void Start()
    {
        Toggle_PayIcon();

        // starting movement
        _npcController.movement.Free_Roam(Main_Controller.instance.currentLocation.data.roamArea, 0f);

        // subscriptions
        _npcController.interactable.OnHoldInteract += Pay;
    }

    public void OnDestroy()
    {
        // subscriptions
        _npcController.interactable.OnHoldInteract -= Pay;
    }


    // Coroutine Action
    public Coroutine Set_Coroutine(Coroutine coroutine)
    {
        if (_coroutine != null && coroutine == null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;

            return null;
        }

        _coroutine = coroutine;
        return _coroutine;
    }

    public void Toggle_Action(bool toggle)
    {
        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();
        
        _npcController.timer.Toggle_RunAnimation(toggle);
        npcController.interactable.LockInteract(toggle);

        if (toggle)
        {
            movement.Set_MoveSpeed(movement.defaultMoveSpeed + 3);
            
            _statusIcon.gameObject.SetActive(false);
            return;
        }
        
        Toggle_PayIcon();

        movement.Set_MoveSpeed(movement.defaultMoveSpeed);
        _npcController.movement.Free_Roam(Main_Controller.instance.currentLocation.data.roamArea, 0f);
        
        Set_Coroutine(null);
    }


    // OnSave Action
    public void Subscribe_OnSave(Action action)
    {
        _OnSave += action;
    }

    public void Invoke_OnSave()
    {
        _OnSave?.Invoke();
    }


    // Data
    public void Set_Data(CraftNPC_Data data)
    {
        if (data == null)
        {
            _data = new(data);
            return;
        }

        _data = new(data);
    }

    public void Set_PurchaseData(PurchaseData data)
    {
        _purchaseData = data;
    }


    // Main Interactions
    public void Toggle_PayIcon()
    {
        _statusIcon.gameObject.SetActive(true);
        
        if (_purchaseData.purchased == false)
        {
            _statusIcon.sprite = _defaultSprite;
            return;
        }

        _statusIcon.sprite = _paymentIconSprite;
    }

    private void Pay()
    {
        if (_purchaseData.purchased) return;
        if (GoldSystem.instance.Update_CurrentAmount(-_purchaseData.price) == false) return;

        _purchaseData.Toggle_PurchaseState(true);
        Toggle_PayIcon();

        // sfx
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }
}