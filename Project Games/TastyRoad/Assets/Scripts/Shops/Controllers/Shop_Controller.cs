using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop_Controller : MonoBehaviour, IInteractable, ISaveLoadable
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

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
        _mainController = FindObjectOfType<Main_Controller>();

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

        UnInteract();
    }



    // InputSystem
    private void OnAction1()
    {
        Menu_Toggle(true);
        _bubble.Toggle_Off();
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (!_menuPanel.TryGetComponent(out ISaveLoadable saveLoad)) return;

        saveLoad.Save_Data();
    }

    public void Load_Data()
    {
        if (!_menuPanel.TryGetComponent(out ISaveLoadable saveLoad)) return;

        saveLoad.Load_Data();
    }



    // IInteractable
    public void Interact()
    {
        if (_bubble.bubbleOn == true)
        {
            UnInteract();
            return;
        }

        _playerInput.enabled = true;
        _bubble.Update_Bubble(_interactIcon, null);
    }

    public void UnInteract()
    {
        _playerInput.enabled = false;
        _bubble.Toggle_Off();
    }



    /// <summary>
    /// Includes all playerInput activations
    /// </summary>
    public void Menu_Toggle(bool toggleOn)
    {
        _menuPanel.SetActive(toggleOn);

        // player input
        _playerInput.enabled = toggleOn;

        // player movement input
        if (_detection.player == null) return;
        _detection.player.Player_Input().enabled = !toggleOn;
    }
}
