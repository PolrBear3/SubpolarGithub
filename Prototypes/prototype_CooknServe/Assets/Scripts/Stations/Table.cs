using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _currentFood;
    [SerializeField] private SpriteRenderer _currentFoodIcon;

    [SerializeField] private bool _optionsOn;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    public void Interact()
    {
        _optionsOn = true;
    }

    private void OnOption1()
    {
        if (!_optionsOn) return;

        if (_playerController.playerInteraction.currentFood != null)
        {
            Swap_Food();
        }
        else
        {
            Give_Food();
        }

        _optionsOn = false;
    }
    private void OnOption2()
    {
        if (!_optionsOn) return;

        // Mix Food

        _optionsOn = false;
    }
    private void OnMovement()
    {
        // Exit Options
        _optionsOn = false;
    }

    //
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
    private void Set_Food()
    {
        Player_Interaction interaction = _playerController.playerInteraction;

        if (_playerController.playerInteraction.currentFood == null) return;

        _currentFood = interaction.currentFood;
        _currentFoodIcon.sprite = _currentFood.foodScrObj.ingameSprite;
        _currentFoodIcon.color = Color.white;

        interaction.Empty_CurrentFood();
    }
    private void Swap_Food()
    {
        // save previous food
        Food previousFood = _currentFood;

        // update this food to player food
        Set_Food();

        // update player food to previous food
        _playerController.playerInteraction.Set_CurrentFood(previousFood);
    }

    private void Give_Food()
    {
        if (_playerController.playerInteraction.currentFood != null) return;

        Player_Interaction interaction = _playerController.playerInteraction;

        interaction.currentFood = _currentFood;
        _currentFood = null;
        _currentFoodIcon.sprite = null;
        _currentFoodIcon.color = Color.clear;

        interaction.Set_CurrentFood(interaction.currentFood);
    }
}
