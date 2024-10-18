using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : ActionBubble_Interactable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _menu;
    public VehicleMenu_Controller menu => _menu;

    [SerializeField] private VehicleMovement_Controller _movement;

    [SerializeField] private Vehicle_Customizer _customizer;
    public Vehicle_Customizer customizer => _customizer;

    [Header("")]
    [SerializeField] private Custom_PositionClaimer _positionClaimer;
    public Custom_PositionClaimer positionClaimer => _positionClaimer;

    [Header("")]
    [SerializeField] private Transform _transparencyPoint;
    private bool _transparencyLocked;

    [Header("")]
    [SerializeField] private SpriteRenderer _interactArea;
    public SpriteRenderer interactArea => _interactArea;

    [Header("")]
    [SerializeField] private Transform _stationSpawnPoint;
    public Transform stationSpawnPoint => _stationSpawnPoint;

    [SerializeField] private Transform _driverSeatPoint;
    public Transform driverSeatPoint => _driverSeatPoint;


    // UnityEngine
    private new void Awake()
    {
        base.Awake();
        mainController.Track_CurrentVehicle(this);
    }

    private new void Start()
    {
        base.Start();

        // subscription
        detection.ExitEvent += Transparency_Update;
        OnAction1Event += Open_VehicleMenu;
    }

    private void OnDestroy()
    {
        // subscription
        detection.ExitEvent += Transparency_Update;
        OnAction1Event -= Open_VehicleMenu;
    }

    private void Update()
    {
        Transparency_Update();
    }


    // Vehicle Sprite Control
    private void Transparency_Update()
    {
        if (_transparencyLocked || detection.player == null)
        {
            LeanTween.alpha(_customizer.gameObject, 1f, 0f);
            return;
        }

        if (detection.player.transform.position.y > _transparencyPoint.position.y)
        {
            LeanTween.alpha(_customizer.gameObject, 0.3f, 0f);
            return;
        }

        LeanTween.alpha(_customizer.gameObject, 1f, 0f);
    }

    public void Toggle_TransparencyLock(bool toggle)
    {
        _transparencyLocked = toggle;

        if (toggle == false) return;
        LeanTween.alpha(_customizer.gameObject, 1f, 0f);
    }


    // Interact Area Control
    public bool Is_InteractArea(Vector2 checkPosition)
    {
        Bounds bounds = _interactArea.bounds;
        return bounds.Contains(checkPosition);
    }

    public Vector2 Redirected_Position(Vector2 currentPosition)
    {
        if (Is_InteractArea(currentPosition)) return currentPosition;

        Bounds bounds = _interactArea.bounds;

        float closestXPos = Mathf.Clamp(currentPosition.x, bounds.min.x, bounds.max.x);
        float closestYPos = Mathf.Clamp(currentPosition.y, bounds.min.y, bounds.max.y);

        return new Vector2(closestXPos, closestYPos);
    }


    // Action Options
    private void Open_VehicleMenu()
    {
        detection.player.Player_Input().enabled = false;

        _menu.VehicleMenu_Toggle(true);
    }
}
