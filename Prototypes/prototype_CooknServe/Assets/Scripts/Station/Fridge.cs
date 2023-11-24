using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    [SerializeField] private Food_ScrObj _foodScrObj;
    [SerializeField] private List<FoodState_Data> _stateDatas = new();

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    //
    public void Interact()
    {
        if (!_playerController.playerInteraction.Is_Closest_Interactable(gameObject)) return;
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
        Food newFood = new();
        Food_ScrObj searchedFood = _gameController.dataController.Get_Food(_foodScrObj);

        newFood.Set_Food(searchedFood);

        for (int i = 0; i < _stateDatas.Count; i++)
        {
            newFood.Update_State(_stateDatas[i].stateType, _stateDatas[i].stateLevel);
        }

        _playerController.playerInteraction.Set_CurrentFood(newFood);
    }
}