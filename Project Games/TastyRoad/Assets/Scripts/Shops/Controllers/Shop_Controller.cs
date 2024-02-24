using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop_Controller : MonoBehaviour, IInteractable
{
    private PlayerInput _playerInput;

    private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private Action_Bubble _bubble;
    public Action_Bubble bubble => _bubble;

    [Header("")]
    [SerializeField] private Sprite _interactIcon;
    [SerializeField] private GameObject _menuPanel;



    // UnityEngine
    private void Awake()
    {
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _detection = gameObject.GetComponent<Detection_Controller>();
    }

    private void Start()
    {
        Menu_Toggle(false);
    }



    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _bubble.Toggle_Off();
    }



    // InputSystem
    private void OnAction1()
    {
        _bubble.Toggle_Off();

        Menu_Toggle(true);
    }

    private void OnExit()
    {
        _bubble.Update_Bubble(_interactIcon, null);

        Menu_Toggle(false);
    }



    // IInteractable
    public void Interact()
    {
        _bubble.Update_Bubble(_interactIcon, null);
    }

    public void UnInteract()
    {
        _bubble.Toggle_Off();
    }



    /// <summary>
    /// Includes all playerInput activations
    /// </summary>
    public void Menu_Toggle(bool toggleOn)
    {
        _menuPanel.SetActive(toggleOn);

        // shop player input
        _playerInput.enabled = !toggleOn;

        // player movement input
        if (_detection.player == null) return;
        _detection.player.Player_Input().enabled = !toggleOn;
    }
}
