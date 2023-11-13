using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    [SerializeField] private Icon_Controller _currentFoodIcon;

    [Header ("Options Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Icon_Controller _icon1;
    [SerializeField] private Icon_Controller _icon2;

    private Food _currentFood;
    private bool _optionsOn;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    public void Interact()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (_currentFood == null && player.currentFood == null) return;

        if (!_optionsOn && player.Is_Closest_Interactable(gameObject))
        {
            Options_Update(true);
            Icons_Update();
            return;
        }

        Options_Update(false);
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

        Player_Interaction player = _playerController.playerInteraction;

        if (_currentFood == null && player.currentFood != null)
        {
            Swap_Food();
        }
        else if (_currentFood != null && player.currentFood == null)
        {
            Give_Food();
        }
        else
        {
            Merge_Food();
        }

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

        if (!_optionsOn) return;
        Options_Update(false);
    }

    //
    private void Options_Update(bool optionsOn)
    {
        _optionsOn = optionsOn;
        _menu.SetActive(_optionsOn);
    }
    private void Icons_Update()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (_currentFood == null && player.currentFood == null)
        {
            _icon1.Clear();
            _icon2.Clear();
            return;
        }

        if (_currentFood == null && player.currentFood != null)
        {
            _icon1.Assign(player.currentFood.foodScrObj.ingameSprite);
            _icon2.Assign(player.currentFood.foodScrObj.ingameSprite);
        }
        else if (_currentFood != null && player.currentFood == null)
        {
            _icon1.Assign(_currentFood.foodScrObj.ingameSprite);
            _icon2.Assign(_currentFood.foodScrObj.ingameSprite);
        }
        else
        {
            List<Food_ScrObj> ingredients = new();
            ingredients.Add(player.currentFood.foodScrObj);
            ingredients.Add(_currentFood.foodScrObj);

            Food_ScrObj mergedFood = _gameController.dataController.Get_MergedFood(ingredients);

            _icon1.Assign(_currentFood.foodScrObj.ingameSprite);

            if (mergedFood == null)
            {
                _icon2.Assign(_currentFood.foodScrObj.ingameSprite);
            }
            else
            {
                _icon2.Assign(mergedFood.ingameSprite);
            }
        }
    }

    private void Set_Food()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (player.currentFood == null) return;

        _currentFood = player.currentFood;
        _currentFoodIcon.Assign(_currentFood.foodScrObj.ingameSprite);

        player.Empty_CurrentFood();
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

        Player_Interaction player = _playerController.playerInteraction;

        player.currentFood = _currentFood;
        player.Set_CurrentFood(player.currentFood);

        _currentFood = null;
        _currentFoodIcon.Clear();
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

        if (mergedFood == null)
        {
            Swap_Food();
            return;
        }

        _currentFood.Set_Food(mergedFood);
        _currentFoodIcon.Assign(_currentFood.foodScrObj.ingameSprite);

        player.Empty_CurrentFood();
    }
}
