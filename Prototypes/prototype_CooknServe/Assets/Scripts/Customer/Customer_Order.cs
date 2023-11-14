using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Order : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _orderFood;
    [SerializeField] private Icon_Controller _serveIcon;

    [Header("Options Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Icon_Controller _menuIcon;

    private bool _menuOn;
    private bool _orderTaken;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    // IInteractable
    public void Interact()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (!player.Is_Closest_Interactable(gameObject) || _menuOn)
        {
            Menu_Activate(false);
            return;
        }

        Menu_Activate(true);

        if (_orderFood != null) return;

        Set_Order();
    }

    // Player Input
    private void OnOption1()
    {
        if (_playerController == null || !_menuOn) return;

        Menu_Activate(false);

        if (!_orderTaken) Take_Order();
        else Serve_Order();
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
        _menuIcon.Assign(_orderFood.foodScrObj.sprite);
    }
    private void Take_Order()
    {
        _orderTaken = true;

        _serveIcon.gameObject.SetActive(true);
        _serveIcon.Assign(_orderFood.foodScrObj.emptySprite);
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
        _orderTaken = false;
        _orderFood = null;

        _menuIcon.Clear();
        _serveIcon.Clear();
    }
}
