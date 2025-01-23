using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Custom_PositionClaimer _positionClaimer;
    public Custom_PositionClaimer positionClaimer => _positionClaimer;

    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private IInteractable_Controller _interactable;

    [Header("")]
    [SerializeField] private GameObject _indicatorObject;

    [SerializeField] private SpriteRenderer _indicatorIcon;
    public SpriteRenderer indicatorIcon => _indicatorIcon;

    [Header("")]
    [SerializeField] private Sprite[] _indicatorSprites;


    private Action OnAction;
    public Action OnActionToggle;

    private int _subscriptionCount;

    private int _currentIndex;
    public int currentIndex => _currentIndex;

    private bool _isSelecting;


    // MonoBehaviour
    private void Start()
    {
        Toggle_CurrentAction();

        // subscriptions
        _detection.ExitEvent += Toggle_CurrentAction;
        _interactable.OnInteract += Set_NextAction;
    }

    private void OnDestroy()
    {
        // subscriptions
        _detection.ExitEvent -= Toggle_CurrentAction;
        _interactable.OnInteract -= Set_NextAction;
    }


    // Indication
    private void Toggle_CurrentAction()
    {
        if (_detection.player == null || _subscriptionCount <= 0 || _indicatorSprites.Length <= 0)
        {
            _isSelecting = false;

            _indicatorObject.SetActive(false);
            return;
        }

        Update_IndicatorSprite();
        _indicatorObject.SetActive(true);

        OnActionToggle?.Invoke();
    }

    private void Update_IndicatorSprite()
    {
        if (_subscriptionCount <= 0 || _indicatorSprites.Length <= 0) return;

        int spriteNum = Mathf.Clamp(_currentIndex, 0, _indicatorSprites.Length - 1);
        _indicatorIcon.sprite = _indicatorSprites[spriteNum];
    }


    // Control
    public void Reset_Subscriptions()
    {
        OnAction = null;

        _subscriptionCount = 0;
        _currentIndex = 0;
    }

    public void Subscribe_Action(Action subscribeAction)
    {
        OnAction += subscribeAction;

        _subscriptionCount++;
    }


    private void Set_NextAction()
    {
        Toggle_CurrentAction();

        if (_isSelecting == false)
        {
            _isSelecting = true;
            return;
        }

        _currentIndex = (_currentIndex + 1) % _subscriptionCount;
        Update_IndicatorSprite();
    }


    // Invoke
    public void Invoke_Action(int actionIndex)
    {
        if (OnAction == null) return;

        Delegate[] allActions = OnAction.GetInvocationList();

        if (actionIndex < 0 || actionIndex >= allActions.Length) return;

        allActions[actionIndex].DynamicInvoke();
    }

    public void Invoke_Action()
    {
        Invoke_Action(_currentIndex);
    }
}
