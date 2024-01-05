using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }
    private void Start()
    {
        _gameController.Connect_Station(gameObject);
    }

    // IInteractable
    public void Interact()
    {
        Empty_PlayerFood();
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
    private void Empty_PlayerFood()
    {
        if (_playerController == null) return;

        Player_Interaction interaction = _playerController.playerInteraction;
        if (!interaction.Is_Closest_Interactable(gameObject)) return;

        if (interaction.currentFood == null) return;

        interaction.Empty_CurrentFood();
    }
}
