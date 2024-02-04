using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Controller : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private PlayerInput _input;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    private Station_Movement _movement;
    public Station_Movement movement => _movement;

    [SerializeField] private Station_ScrObj _stationScrObj;
    public Station_ScrObj stationScrObj => _stationScrObj;

    public delegate void Action_Event();
    public event Action_Event Interact_Event;

    [Header("")]
    [SerializeField] private Color _indicatorColor;
    [SerializeField] private Color _restrictionColor;
    private Coroutine _colorCoroutine;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(this);

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        if (gameObject.TryGetComponent(out PlayerInput input)) { _input = input; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
        if (gameObject.TryGetComponent(out Station_Movement movement)) { _movement = movement; }

        PlayerInput_Toggle(false);
    }

    // InputSystem
    private void OnInteract()
    {
        Interact_Event?.Invoke();
    }

    // Player Input Toggle
    public void PlayerInput_Toggle(bool toggleOn)
    {
        _input.enabled = toggleOn;
    }

    // Green Red Color Triggers
    public void Indicator_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _spriteRenderer.color = Color.white;
            return;
        }

        _spriteRenderer.color = _indicatorColor;
    }
    public void Restriction_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _spriteRenderer.color = _indicatorColor;
            return;
        }

        _spriteRenderer.color = _restrictionColor;
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

    // Main Station Controls
    public void Destroy_Station()
    {
        _mainController.UnClaim_Position(movement.Current_SnapPosition());

        _mainController.UnTrack_CurrentStation(this);
        Destroy(gameObject);
    }
}