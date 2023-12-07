using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Order_Stand : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    private Game_Controller _gameController;
    private Player_Controller _playerController;

    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private bool _orderOpen;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }
    private void Start()
    {
        _gameController.Connect_Station(gameObject);
    }

    // IInteractable
    public void Interact()
    {
        Order_Toggle();
        Sprite_Toggle();
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
    private void Sprite_Toggle()
    {
        if (_orderOpen) _sr.sprite = activeSprite;
        else _sr.sprite = inactiveSprite;
    }
    private void Order_Toggle()
    {
        _orderOpen = !_orderOpen;
    }
}
