using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : MonoBehaviour, IInteractable
{
    private SpriteRenderer _vehicleSprite;
    private PlayerInput _playerInput;

    [HideInInspector] public Main_Controller mainController;
    [HideInInspector] public Detection_Controller detection;

    [SerializeField] private Action_Bubble _bubble;

    [Header("Insert Vehicle Panel Prefab")]
    [SerializeField] private VehicleMenu_Controller _menu;
    public VehicleMenu_Controller menu => _menu;

    [Header("")]
    [SerializeField] private Transform _transparencyPoint;

    [SerializeField] private Transform _stationSpawnPoint;
    public Transform stationSpawnPoint => _stationSpawnPoint;



    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentVehicle(this);

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _vehicleSprite = sr; }
        if (gameObject.TryGetComponent(out PlayerInput input)) { _playerInput = input; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { this.detection = detection; }
    }

    private void Start()
    {
        UnInteract();
    }

    private void Update()
    {
        Transparency_Update();
    }



    // InputSystem
    private void OnAction1()
    {
        detection.player.Player_Input().enabled = false;

        _bubble.Toggle(false);
        _playerInput.enabled = false;

        _menu.VehicleMenu_Toggle(true);
    }



    // IInteractable
    public void Interact()
    {
        if (_bubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        _playerInput.enabled = true;
        _bubble.Toggle(true);
    }

    public void UnInteract()
    {
        _playerInput.enabled = false;
        _bubble.Toggle(false);
    }



    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        Main_Controller.Change_SpriteAlpha(_vehicleSprite, 1f);

        UnInteract();
    }



    // Vehicle Prefab Control
    private void Transparency_Update()
    {
        if (detection.player == null) return;

        if (detection.player.transform.position.y > _transparencyPoint.position.y)
        {
            Main_Controller.Change_SpriteAlpha(_vehicleSprite, 0.3f);
        }
        else
        {
            Main_Controller.Change_SpriteAlpha(_vehicleSprite, 1f);
        }
    }
}
