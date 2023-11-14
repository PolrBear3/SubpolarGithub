using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Order : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _orderFood;
    [SerializeField] private Icon_Controller _serveReadyIcon;

    [Header("Options Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Icon_Controller _menuIcon;

    private bool _menuOn;
    private bool _serveReady;

    //
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    //
    public void Interact()
    {
        if (!_playerController.playerInteraction.Is_Closest_Interactable(gameObject)) return;

        if (_orderFood == null)
        {
            Set_Order();
        }
        else
        {
            Serve_Order();
        }
    }

    private void OnOption1()
    {

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
    private void Menu_Activate(bool activate)
    {
        
    }

    //
    private void Set_Order()
    {
        if (_orderFood != null) return;

        Data_Controller data = _gameController.dataController;

        // random merged food
        int randFoodID = Random.Range(0, data.mergedFoods.Count);
        Food_ScrObj randFood = data.Get_MergedFood(randFoodID);

        // random heat level
        int randStateLevel = Random.Range(1, 3);
        FoodState_Data randData = new();
        randData.stateType = FoodState_Type.heated;
        randData.stateLevel = randStateLevel;

        // set order food
        Food orderFood = new();
        orderFood.foodScrObj = randFood;
        orderFood.data.Add(randData);
        _orderFood = orderFood;
    }

    private void Take_Order()
    {

    }

    private void Serve_Order()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (player.currentFood == null) return;

        Food_ScrObj playerFood = player.currentFood.foodScrObj;

        if (playerFood != _orderFood.foodScrObj) return;

        Cancel_Order();
        player.Empty_CurrentFood();
    }

    private void Cancel_Order()
    {
        _orderFood = null;

        _menuIcon.Clear();
        _serveReadyIcon.Clear();
    }
}
