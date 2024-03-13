using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Controller : MonoBehaviour
{
    private PlayerInput _playerInput;
    private SpriteRenderer _spriteRenderer;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    private Station_Movement _movement;
    public Station_Movement movement => _movement;



    [SerializeField] private Station_ScrObj _stationScrObj;
    public Station_ScrObj stationScrObj => _stationScrObj;

    private Animator _stationAnimator;


    public delegate void Action_Event();

    public event Action_Event Interact_Event;
    public event Action_Event Action1_Event;
    public event Action_Event Action2_Event;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(this);

        _playerInput = gameObject.GetComponent<PlayerInput>();
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _spriteRenderer = sr; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
        if (gameObject.TryGetComponent(out Station_Movement movement)) { _movement = movement; }
        if (gameObject.TryGetComponent(out Animator stationAnimator)) { _stationAnimator = stationAnimator; }
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



    // Green Red Color Triggers
    public void TransparentBlink_Toggle(bool toggleOn)
    {
        if (toggleOn == true)
        {
            _stationAnimator.enabled = true;
            _stationAnimator.Play("Station_transaprencyBlink");
            return;
        }

        _stationAnimator.enabled = false;
        _spriteRenderer.color = Color.white;
    }
    public void Restriction_Toggle(bool toggleOn)
    {
        if (toggleOn == true)
        {
            _stationAnimator.enabled = false;
            _spriteRenderer.color = _mainController.dataController.restrictionColor;
            return;
        }

        _stationAnimator.enabled = true;
        _stationAnimator.Play("Station_transaprencyBlink");
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



    // Main Station Controls
    public void PlayerInput_Activation(bool isEnabled)
    {
        _playerInput.enabled = isEnabled;
    }

    public void Destroy_Station()
    {
        Vector2 snapPosition = Main_Controller.SnapPosition(transform.position);
        _mainController.UnClaim_Position(snapPosition);

        _mainController.UnTrack_CurrentStation(this);
        Destroy(gameObject);
    }
}