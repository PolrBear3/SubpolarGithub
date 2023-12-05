using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Order_Stand : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private bool _orderOpen;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    // IInteractable
    public void Interact()
    {
        Order_Toggle();
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = playerController;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = null;
    }

    //
    private void Order_Toggle()
    {
        _orderOpen = !_orderOpen;
    }
}
