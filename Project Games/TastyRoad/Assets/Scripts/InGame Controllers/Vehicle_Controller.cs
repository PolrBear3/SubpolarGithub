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

    [Header("Insert Vehicle Panel Prefab")]
    [SerializeField] private VehiclePanel_Controller _panel;
    public VehiclePanel_Controller panel => _panel;

    [Header("")]
    [SerializeField] private Action_Bubble _bubble;

    [SerializeField] private Sprite _panelIcon;
    [SerializeField] private Sprite _driveIcon;

    [Header("")]
    [SerializeField] private Transform _transparencyPoint;

    [SerializeField] private Transform _spawnPoint;
    public Transform spawnPoint => _spawnPoint;

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
        VehiclePanel_Toggle(false);
        _playerInput.enabled = false;

        Claim_Position();
    }

    private void Update()
    {
        Transparency_Update();
    }



    // InputSystem
    private void OnAction1()
    {
        Player_PlayerInput_Toggle(false);

        _bubble.Toggle_Off();
        _playerInput.enabled = false;

        VehiclePanel_Toggle(true);
    }

    /*
    private void OnAction2()
    {
        Player_PlayerInput_Toggle(false);

        // vehicle drive mode function

        _bubble.Toggle_Off();
        _playerInput.enabled = false;
    }
    */



    // IInteractable
    public void Interact()
    {
        _bubble.Update_Bubble(_panelIcon, null);
        _playerInput.enabled = true;
    }

    public void UnInteract()
    {
        _bubble.Toggle_Off();
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        Main_Controller.Change_SpriteAlpha(_vehicleSprite, 1f);

        _bubble.Toggle_Off();
        _playerInput.enabled = false;
    }



    // Position Claim
    private void Claim_Position()
    {
        // this is the size of the vehicle (make list size for future vehicles)
        Vector2 claim1 = new Vector2(transform.position.x + 1, transform.position.y);
        Vector2 claim2 = new Vector2(transform.position.x - 1, transform.position.y);

        mainController.Claim_Position(transform.position);
        mainController.Claim_Position(claim1);
        mainController.Claim_Position(claim2);
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

    // Panel Control
    public void VehiclePanel_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _panel.gameObject.SetActive(false);

            return;
        }

        _panel.gameObject.SetActive(true);
        _panel.Menu_Control(_panel.currentMenuNum);
    }

    /// <summary>
    /// Disables player movement for panel menu control.
    /// </summary>
    public void Player_PlayerInput_Toggle(bool toggleOn)
    {
        if (detection.player.gameObject.TryGetComponent(out PlayerInput playerInput) == false) return;

        if (toggleOn == false)
        {
            playerInput.enabled = false;
            return;
        }

        playerInput.enabled = true;
    }
}
