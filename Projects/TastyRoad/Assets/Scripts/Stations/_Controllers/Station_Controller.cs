using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Controller : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer => _spriteRenderer;


    [Header("")]
    [SerializeField] private Animator _stationAnimator;
    public Animator stationAnmiator => _stationAnimator;

    [SerializeField] private BasicAnimation_Controller _animController;
    public BasicAnimation_Controller animController => _animController;


    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;
    public ActionBubble_Interactable interactable => _interactable;

    [SerializeField] private IInteractable_Controller _iInteractable;
    public IInteractable_Controller iInteractable => _iInteractable;

    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;


    [Header("")]
    [SerializeField] private Station_Movement _movement;
    public Station_Movement movement => _movement;

    [SerializeField] private Station_Maintenance _maintenance;
    public Station_Maintenance maintenance => _maintenance;

    [SerializeField] private Station_ScrObj _stationScrObj;
    public Station_ScrObj stationScrObj => _stationScrObj;


    [Header("")]
    [SerializeField] private ItemDropper _itemDropper;
    public ItemDropper itemDropper => _itemDropper;


    private StationData _data;
    public StationData data => _data;

    private bool _isRoamArea;
    public bool isRoamArea => _isRoamArea;

    public Action OnStationDestroy;


    // UnityEngine
    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Main_Controller.instance.Track_CurrentStation(this);
    }

    private void Start()
    {
        Set_Data();
    }

    private void OnDestroy()
    {
        Destroy_Station();
    }


    // Data Constructor Functions
    private void Set_Data()
    {
        if (_data != null) return;

        _data = new(_stationScrObj, transform.position);
    }

    public void Set_Data(StationData data)
    {
        _data = data;
    }


    // Green Red Color Triggers
    public void TransparentBlink_Toggle(bool toggleOn)
    {
        if (toggleOn == true)
        {
            _stationAnimator.Play("TransparencyBlinker_blink");
            return;
        }

        _stationAnimator.Play("stop");
    }
    public void RestrictionBlink_Toggle(bool toggleOn)
    {
        if (toggleOn == true)
        {
            _stationAnimator.Play("TransparencyBlinker_restrictBlink");
            return;
        }

        _stationAnimator.Play("TransparencyBlinker_blink");
        _spriteRenderer.color = Color.white;
    }


    /// <summary>
    /// Finds Food Icon if it is attached to this station prefab.
    /// </summary>
    public FoodData_Controller Food_Icon()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent(out FoodData_Controller foodIcon)) continue;
            return foodIcon;
        }
        return null;
    }

    /// <summary>
    /// Finds Action Bubble if it is attached to this station prefab.
    /// </summary>
    public Action_Bubble ActionBubble()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent(out Action_Bubble bubble)) continue;
            return bubble;
        }
        return null;
    }


    // Main Station Controls
    public void RoamArea_Toggle(bool toggleOn)
    {
        _isRoamArea = toggleOn;
    }


    public void Destroy_Station()
    {
        OnStationDestroy?.Invoke();
        
        Main_Controller.instance.UnTrack_CurrentStation(this);
        Destroy(gameObject);
    }
}