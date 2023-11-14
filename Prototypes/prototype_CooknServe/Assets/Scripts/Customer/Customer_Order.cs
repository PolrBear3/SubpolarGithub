using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Order : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _orderFood;
    [SerializeField] private Icon_Controller _currentFoodIcon;

    [SerializeField] private int _servePoint;

    [Header("Order Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Icon_Controller _orderIcon;

    private bool _menuOn;
    private bool _isOrdered;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    // IInteractable
    public void Interact()
    {
        Menu_Activate(!_menuOn);

        if (!_menuOn) return;

        Player_Interaction player = _playerController.playerInteraction;

        if (!player.Is_Closest_Interactable(gameObject)) return;
        if (_isOrdered) return;

        _isOrdered = true;
        Set_Order();
    }

    // Player Input
    private void OnOption1()
    {
        if (_playerController == null) return;

        Menu_Activate(!_menuOn);
        Serve_Order();
    }
    private void OnOption2()
    {
        if (_playerController == null) return;

        Menu_Activate(!_menuOn);
        Serve_Order();
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

        Menu_Activate(false);
    }

    // Custom
    private void Menu_Activate(bool activate)
    {
        _menuOn = activate;
        _menu.SetActive(activate);
    }

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
        _orderIcon.Assign(_orderFood.foodScrObj.sprite);
    }
    private void Serve_Order()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (player.currentFood == null) return;

        Food_ScrObj playerFood = player.currentFood.foodScrObj;

        if (playerFood != _orderFood.foodScrObj) return;

        _servePoint++;

        Cancel_Order();
        player.Empty_CurrentFood();
    }
    private void Cancel_Order()
    {
        _orderFood = null;
        _isOrdered = false;
    }
}
