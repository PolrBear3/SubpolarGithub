using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop_Controller : MonoBehaviour, IInteractable, ISaveLoadable
{
    private PlayerInput _playerInput;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private Action_Bubble _bubble;
    public Action_Bubble bubble => _bubble;



    public delegate void InputSystem_Event();

    public event InputSystem_Event Action1_Event;

    public event InputSystem_Event Interact_Event;
    public event InputSystem_Event UnInteract_Event;



    [Header("")]
    [SerializeField] private GameObject _menuPanel;

    [Header("")]
    [SerializeField] private Vector2 _claimPositionRange;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();

        _playerInput = gameObject.GetComponent<PlayerInput>();
        _detection = gameObject.GetComponent<Detection_Controller>();

        Claim_Position();
    }

    private void Start()
    {
        Menu_Toggle(false);
        UnInteract();
    }



    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
    }



    // InputSystem
    private void OnAction1()
    {
        Menu_Toggle(true);
        UnInteract();

        Action1_Event?.Invoke();
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (_menuPanel == null) return;
        if (!_menuPanel.TryGetComponent(out ISaveLoadable saveLoad)) return;

        saveLoad.Save_Data();
    }

    public void Load_Data()
    {
        if (_menuPanel == null) return;
        if (!_menuPanel.TryGetComponent(out ISaveLoadable saveLoad)) return;

        saveLoad.Load_Data();
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

        Interact_Event?.Invoke();
    }

    public void UnInteract()
    {
        _playerInput.enabled = false;
        _bubble.Toggle(false);

        UnInteract_Event?.Invoke();
    }



    // Position Claim
    private void Claim_Position()
    {
        int claimRepeatNum = (int)_claimPositionRange.x * 2 + 1;
        float positionXNum = transform.position.x - _claimPositionRange.x;

        for (int i = 0; i < claimRepeatNum; i++)
        {
            _mainController.Claim_Position(new(positionXNum, transform.position.y));
            positionXNum++;
        }
    }



    /// <summary>
    /// Includes all playerInput activations
    /// </summary>
    public void Menu_Toggle(bool toggleOn)
    {
        if (_menuPanel == null) return;

        // menu panel activation
        _menuPanel.SetActive(toggleOn);

        // player movement input
        if (_detection.player == null) return;
        _detection.player.Player_Input().enabled = !toggleOn;
    }
}