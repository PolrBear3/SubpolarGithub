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
    public NPC_Controller main => _npcController;


    [Header("")]
    [SerializeField] private DialogTrigger _dialog;

    [SerializeField] private Clock_Timer _actionTimer;
    public Clock_Timer actionTimer => _actionTimer;


    [Header("")]
    [SerializeField] private Sprite _npcIconSprite;
    public Sprite npcIconSprite => _npcIconSprite;

    [SerializeField] private SpriteRenderer _statusIcon;
    private Sprite _defaultSprite;


    [Header("")]
    [SerializeField][Range(0, 1000)] private int _defalutPrice;
    public int defaultPrice => _defalutPrice;

    [SerializeField][Range(0, 100)] private float _upgradeTimeValue;
    public float upgradeTimeValue => _upgradeTimeValue;


    private CraftNPC_Data _data;
    public CraftNPC_Data data => _data;


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
        _npcController.movement.Free_Roam(0);

        // subscriptions
        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.OnHoldIInteract += Pay;
    }

    public void OnDestroy()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _npcController.interactable;

        interactable.OnHoldIInteract -= Pay;
    }


    // Coroutine Action
    public Coroutine Set_Coroutine(Coroutine coroutine)
    {
        _coroutine = coroutine;
        return _coroutine;
    }

    public void Toggle_Coroutine(bool toggle)
    {
        actionTimer.Toggle_RunAnimation(toggle);
        main.interactable.LockInteract(toggle);

        if (toggle)
        {
            _statusIcon.gameObject.SetActive(false);
            return;
        }

        Toggle_PayIcon();

        _npcController.movement.Free_Roam(0);
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
            _data = new(false);
            return;
        }

        _data = new(data);
    }


    // Main Interactions
    public void Toggle_PayIcon()
    {
        if (_data.payed == false)
        {
            _statusIcon.gameObject.SetActive(false);
            return;
        }

        _statusIcon.sprite = _defaultSprite;
        _statusIcon.gameObject.SetActive(true);
    }

    private void Pay()
    {
        if (_data == null || _data.payed) return;
        if (GoldSystem.instance.Update_CurrentAmount(_data.price) == false) return;

        _data = new(true);
        Toggle_PayIcon();

        // sfx
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }
}