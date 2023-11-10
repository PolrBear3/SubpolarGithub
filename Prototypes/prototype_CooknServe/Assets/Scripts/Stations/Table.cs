using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    [SerializeField] private SpriteRenderer _currentFoodIcon;
    [SerializeField] private GameObject _optionsMenu;

    private Food _currentFood;
    private bool _optionsOn;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    public void Interact()
    {
        if (!_optionsOn)
        {
            Options_Update(true);
        }
        else
        {
            Options_Update(false);
        }
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

        Options_Update(false);
    }
    private void OnOption2()
    {
        if (!_optionsOn) return;

        Merge_Food();

        Options_Update(false);
    }
    private void OnMovement()
    {
        if (!_optionsOn) return;

        Options_Update(false);
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
        Options_Update(false);
    }

    //
    private void Options_Update(bool optionsOn)
    {
        _optionsOn = optionsOn;
        _optionsMenu.SetActive(_optionsOn);

        // option box icon update
    }

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

    private void Merge_Food()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (player.currentFood == null || _currentFood == null)
        {
            Give_Food();
            return;
        }

        if (player.currentFood.foodScrObj == _currentFood.foodScrObj)
        {
            Swap_Food();
            return;
        }

        List<Food_ScrObj> ingredients = new();
        ingredients.Add(player.currentFood.foodScrObj);
        ingredients.Add(_currentFood.foodScrObj);

        Food_ScrObj mergedFood = _gameController.dataController.Get_MergedFood(ingredients);
        _currentFood.Set_Food(mergedFood);

        _currentFoodIcon.sprite = _currentFood.foodScrObj.ingameSprite;
        _currentFoodIcon.color = Color.white;

        player.Empty_CurrentFood();
    }
}
