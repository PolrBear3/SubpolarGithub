using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionBubble_Interactable : MonoBehaviour, IInteractable
{
    [Header("")]
    [SerializeField] private PlayerInput _input;

    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;

    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField] private Action_Bubble _bubble;
    public Action_Bubble bubble => _bubble;

    private bool _interactLocked;

    public delegate void Event();

    public event Event InteractEvent;
    public event Event Action1Event;
    public event Event Action2Event;


    // MonoBehaviour
    public void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    public void Start()
    {
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
        Action1Event?.Invoke();

        UnInteract();
    }

    private void OnAction2()
    {
        if (Action2Event == null) return;
        Action2Event?.Invoke();

        UnInteract();
    }


    // IInteractable
    public void Interact()
    {
        if (_interactLocked) return;

        // bubble empty
        if (_bubble == null) return;

        // bubble off
        if (_bubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        // bubble on
        _input.enabled = true;
        _bubble.Toggle(true);

        //
        InteractEvent?.Invoke();
    }

    public void UnInteract()
    {
        if (_bubble == null) return;

        // bubble off
        _input.enabled = false;
        _bubble.Toggle(false);
    }


    //
    public void InputToggle(bool toggleOn)
    {
        _input.enabled = toggleOn;
    }

    public void LockInteract(bool toggleLock)
    {
        _interactLocked = toggleLock;
    }
}
