using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ActionSelector : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;


    [Space(20)]
    [SerializeField] private Custom_PositionClaimer _positionClaimer;
    public Custom_PositionClaimer positionClaimer => _positionClaimer;

    [SerializeField] private Detection_Controller _detection;

    [SerializeField] private IInteractable_Controller _interactable;
    public IInteractable_Controller interactable => _interactable;
    
    [Space(20)]
    [SerializeField] private GameObject _indicatorObject;

    [SerializeField] private SpriteRenderer _indicatorIcon;
    public SpriteRenderer indicatorIcon => _indicatorIcon;


    private bool _isSelecting;

    private int _currentIndex;
    public int currentIndex => _currentIndex;

    private List<ActionSelector_Data> _currentDatas = new();
    public List<ActionSelector_Data> currentDatas => _currentDatas;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Toggle_CurrentAction();
        Update_IndicatorSprite();

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
        if (_detection.player == null || _currentDatas.Count <= 0)
        {
            _isSelecting = false;
            _indicatorObject.SetActive(false);
            
            return;
        }

        _indicatorObject.SetActive(true);
    }

    private void Update_IndicatorSprite()
    {
        if (_currentDatas.Count <= 0) return;

        _indicatorIcon.sprite = _currentDatas[_currentIndex].actionSprite;
    }


    // Control
    public void Reset_Subscriptions()
    {
        _currentDatas.Clear();
        _currentIndex = 0;
    }

    public void Add_ActionData(ActionSelector_Data data)
    {
        _currentDatas.Add(data);

        Update_IndicatorSprite();
    }


    private void Set_NextAction()
    {
        Toggle_CurrentAction();

        if (_isSelecting == false)
        {
            _isSelecting = true;
            return;
        }

        _currentIndex = (_currentIndex + 1) % _currentDatas.Count;
        Update_IndicatorSprite();
    }


    // Invoke
    public void Invoke_Action(int actionIndex)
    {
        _currentDatas[actionIndex].actionEvent?.Invoke();
    }

    public void Invoke_Action()
    {
        Invoke_Action(_currentIndex);
    }
}
