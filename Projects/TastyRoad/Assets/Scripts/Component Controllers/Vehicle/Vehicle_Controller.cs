using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : MonoBehaviour, IInteractable
{
    private PlayerInput _playerInput;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [Header("")]
    [SerializeField] private Action_Bubble _bubble;

    [SerializeField] private Vehicle_Customizer _customizer;

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
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
        mainController.Track_CurrentVehicle(this);

        if (gameObject.TryGetComponent(out PlayerInput input)) { _playerInput = input; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
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

    private void OnAction2()
    {
        _bubble.Toggle(false);
        _playerInput.enabled = false;

        mainController.worldMap.Map_Toggle(true);
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

        LeanTween.alpha(_customizer.gameObject, 1f, 0f);

        UnInteract();
    }



    // Vehicle Sprite Control
    private void Transparency_Update()
    {
        if (detection.player == null) return;

        if (detection.player.transform.position.y > _transparencyPoint.position.y)
        {
            LeanTween.alpha(_customizer.gameObject, 0.3f, 0f);
        }
        else
        {
            LeanTween.alpha(_customizer.gameObject, 1f, 0f);
        }
    }
}
