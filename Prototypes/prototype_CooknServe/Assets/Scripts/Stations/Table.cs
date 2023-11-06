using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _currentFood;
    [SerializeField] private SpriteRenderer _currentFoodIcon;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    public void Interact()
    {
        Use_Table();
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
    private void Use_Table()
    {
        if (_currentFood == null) Set_PlayerFood();
        else Give_CurrentFood();
    }

    private void Set_PlayerFood()
    {
        if (_playerController.playerInteraction.currentFood == null) return;

        Player_Interaction interaction = _playerController.playerInteraction;

        _currentFood = interaction.currentFood;
        _currentFoodIcon.sprite = _currentFood.foodScrObj.ingameSprite;
        _currentFoodIcon.color = Color.white;

        interaction.Empty_CurrentFood();
    }
    private void Give_CurrentFood()
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
