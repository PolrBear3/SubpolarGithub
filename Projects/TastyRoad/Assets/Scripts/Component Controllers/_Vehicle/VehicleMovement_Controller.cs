using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleMovement_Controller : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private Vehicle_Controller _controller;
    [SerializeField] private ActionBubble_Interactable _interactable;


    [Header("")]
    [SerializeField] private Animator _wheelsAnim;
    [SerializeField] private Sprite _wheelsSprite;


    [Header("")]
    [SerializeField][Range(0, 10)] private float _defaultMoveSpeed;

    [SerializeField][Range(0, 10)] private float _maxMoveSpeed;
    public float maxMoveSpeed => _maxMoveSpeed;


    private bool _onBoard;
    public bool onBoard => _onBoard;

    private float _moveSpeed;
    public float moveSpeed => _moveSpeed;


    private Vector2 _defaultPosition;
    private Vector2 _recentPosition;


    // UnityEngine
    private void Start()
    {
        // set to recent position
        _controller.positionClaimer.UnClaim_CurrentPositions();

        _controller.transform.position = _recentPosition;
        _controller.positionClaimer.Claim_CurrentPositions();

        // set player position
        Player_Controller player = Main_Controller.instance.Player();
        player.transform.position = _controller.driverSeatPoint.position;

        // subscriptions
        WorldMap_Controller.OnNewLocation += Moveto_DefaultPosition;

        _interactable.OnAction1 += Ride;
        _interactable.OnAction2 += _controller.Open_LocationMenu;
    }

    private void OnDestroy()
    {
        // subscriptions
        WorldMap_Controller.OnNewLocation -= Moveto_DefaultPosition;

        _interactable.OnAction1 -= Ride;
        _interactable.OnAction2 -= _controller.Open_LocationMenu;
    }

    private void Update()
    {
        ResrictPosition_Update();
        ExitRestricted_IndicationUpdate();
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("VehicleMovement_Controller/_moveSpeed", _moveSpeed);

        ES3.Save("VehicleMovement_Controller/_defaultPosition", _defaultPosition);
        ES3.Save("VehicleMovement_Controller/_recentPosition", _recentPosition);
    }

    public void Load_Data()
    {
        _moveSpeed = ES3.Load("VehicleMovement_Controller/_moveSpeed", _defaultMoveSpeed);

        if (ES3.KeyExists("VehicleMovement_Controller/_recentPosition") == false)
        {
            _recentPosition = _controller.transform.position;
            _defaultPosition = _controller.transform.position;

            return;
        }

        _defaultPosition = ES3.Load("VehicleMovement_Controller/_defaultPosition", _defaultPosition);
        _recentPosition = ES3.Load("VehicleMovement_Controller/_recentPosition", _recentPosition);
    }


    // Visual Control
    private void Toggle_WheelsAnimation(bool toggle)
    {
        _wheelsAnim.enabled = toggle;

        if (toggle)
        {
            _wheelsAnim.Play("VehicleWheels_movement");
            return;
        }

        SpriteRenderer wheelsRenderer = _wheelsAnim.gameObject.GetComponent<SpriteRenderer>();
        wheelsRenderer.sprite = _wheelsSprite;
    }


    // Movement
    private void Movement_Update()
    {
        if (_onBoard == false) return;

        Vector2 currentDirection = Input_Controller.instance.inputDirection;
        _controller.transform.Translate(_moveSpeed * Time.deltaTime * currentDirection);
    }

    public void Update_MovementSpeed(float updateValue)
    {
        _moveSpeed = Mathf.Clamp(_moveSpeed + updateValue, 0, _maxMoveSpeed);
    }


    private void ResrictPosition_Update()
    {
        if (_onBoard == false) return;

        Location_Controller location = Main_Controller.instance.currentLocation;
        Transform vehicle = _controller.transform;

        if (location.Restricted_Position(vehicle.position) == false) return;

        vehicle.position = location.Redirected_Position(vehicle.position);
    }

    private void ExitRestricted_IndicationUpdate()
    {
        if (_onBoard == false) return;

        if (Exit_Available() == false)
        {
            LeanTween.color(_controller.spritesFile, Color.red, 0.01f);
            return;
        }

        LeanTween.color(_controller.spritesFile, Color.white, 0.01f);
    }


    private void Moveto_DefaultPosition()
    {
        _controller.positionClaimer.UnClaim_CurrentPositions();

        _controller.transform.position = _defaultPosition;
        _recentPosition = _defaultPosition;

        _controller.positionClaimer.Claim_CurrentPositions();
    }


    // Ride Actions
    private void Ride()
    {
        if (_onBoard) return;

        Input_Controller.instance.OnInteract += Exit;
        _interactable.OnAction2 -= _controller.Open_LocationMenu;

        _interactable.LockUnInteract(true);
        _interactable.bubble.Toggle(false);

        _controller.Toggle_TransparencyLock(true);
        _controller.positionClaimer.UnClaim_CurrentPositions();

        Toggle_WheelsAnimation(true);

        Player_Controller player = Main_Controller.instance.Player();

        player.Toggle_Controllers(false);
        player.Toggle_Hide(true);

        _onBoard = true;
    }


    private bool Exit_Available()
    {
        if (_onBoard == false) return false;

        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.currentLocation;

        Custom_PositionClaimer claimer = _controller.positionClaimer;

        for (int i = 0; i < claimer.All_InteractPositions().Count; i++)
        {
            if (location.Restricted_Position(claimer.All_InteractPositions()[i])) return false;

            if (claimer.Is_ClaimPosition(claimer.All_InteractPositions()[i]) == false) continue;
            Vector2 redirectedPos = location.Redirected_SnapPosition(claimer.All_InteractPositions()[i]);

            if (main.Position_Claimed(redirectedPos)) return false;
        }

        return true;
    }

    private void Exit()
    {
        if (Exit_Available() == false) return;

        _interactable.LockUnInteract(false);
        _interactable.UnInteract();

        Input_Controller.instance.OnInteract -= Exit;
        _interactable.OnAction2 += _controller.Open_LocationMenu;

        // set vehicle to snap point
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.currentLocation;

        Transform vehicle = _controller.transform;

        Vector2 targetPos = location.Redirected_SnapPosition(vehicle.position);
        vehicle.transform.position = targetPos;

        _controller.Toggle_TransparencyLock(false);
        _controller.positionClaimer.Claim_CurrentPositions();

        Toggle_WheelsAnimation(false);

        // update player
        Player_Controller player = main.Player();

        player.Toggle_Hide(false);
        player.Toggle_Controllers(true);

        player.transform.position = _controller.driverSeatPoint.position;

        _recentPosition = _controller.transform.position;
        _onBoard = false;
    }
}