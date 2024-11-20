using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Controller : MonoBehaviour
{
    private PlayerInput _playerInput;

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer => _spriteRenderer;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;


    [Header("")]
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private Station_Movement _movement;
    public Station_Movement movement => _movement;

    [SerializeField] private Station_Maintenance _maintenance;
    public Station_Maintenance maintenance => _maintenance;

    [SerializeField] private Animator _stationAnimator;
    public Animator stationAnmiator => _stationAnimator;

    [SerializeField] private ItemDropper _itemDropper;
    public ItemDropper itemDropper => _itemDropper;


    [Header("")]
    [SerializeField] private Station_ScrObj _stationScrObj;
    public Station_ScrObj stationScrObj => _stationScrObj;

    private StationData _data;
    public StationData data => _data;

    private bool _isRoamArea;
    public bool isRoamArea => _isRoamArea;


    public delegate void Action_Event();

    public event Action_Event Interact_Event;
    public event Action_Event Action1_Event;
    public event Action_Event Action2_Event;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _playerInput = gameObject.GetComponent<PlayerInput>();

        _mainController.Track_CurrentStation(this);
    }

    private void Start()
    {
        Set_Data();
    }


    // InputSystem
    private void OnInteract()
    {
        Interact_Event?.Invoke();
    }

    private void OnAction1()
    {
        Action1_Event?.Invoke();
    }
    private void OnAction2()
    {
        Action2_Event?.Invoke();
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
            _stationAnimator.enabled = true;
            _stationAnimator.Play("TransparencyBlinker_blink");
            return;
        }

        _stationAnimator.enabled = false;
        _spriteRenderer.color = Color.white;
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
    public void PlayerInput_Activation(bool isEnabled)
    {
        _playerInput.enabled = isEnabled;
    }

    public void RoamArea_Toggle(bool toggleOn)
    {
        _isRoamArea = toggleOn;
    }


    public void Destroy_Station()
    {
        _mainController.UnTrack_CurrentStation(this);
        Destroy(gameObject);
    }
}