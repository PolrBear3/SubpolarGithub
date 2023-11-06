using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    public void Interact()
    {
        Give_Apple();
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
    private void Give_Apple()
    {
        Food newApple = gameObject.AddComponent<Food>();
        newApple.Set_Food(_gameController.dataController.Get_Food(0));
        newApple.Update_State(FoodState_Type.sliced, 3);

        _playerController.playerInteraction.Set_CurrentFood(newApple);
    }
}