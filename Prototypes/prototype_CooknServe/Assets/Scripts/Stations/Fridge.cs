using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    [SerializeField] private Food_ScrObj _foodScrObj;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    public void Interact()
    {
        Give_Food();
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
    private void Give_Food()
    {
        Food newFood = gameObject.AddComponent<Food>();
        Food_ScrObj searchedFood = _gameController.dataController.Get_Food(_foodScrObj);

        newFood.Set_Food(searchedFood, searchedFood.ingredients);
        newFood.Update_State(FoodState_Type.sliced, 3);

        _playerController.playerInteraction.Set_CurrentFood(newFood);
    }
}