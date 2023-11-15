using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Order : MonoBehaviour, IInteractable
{
    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _orderFood;

    [Header("Default")]
    [SerializeField] private Icon_Controller _currentFoodIcon;
    [SerializeField] private Sprite _coinSprite;

    [Header("Order Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Icon_Controller _orderIcon;

    private bool _menuOn;
    private bool _orderServed;
    private bool _calculateComplete;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    // IInteractable
    public void Interact()
    {
        Player_Interaction player = _playerController.playerInteraction;

        if (_calculateComplete && player.Is_Closest_Interactable(gameObject))
        {
            Pay_Order();
            Menu_Activate(false);

            return;
        }

        if (_orderServed) return;

        if (!_menuOn && player.Is_Closest_Interactable(gameObject))
        {
            Menu_Activate(true);
            Set_Order();
            return;
        }

        Menu_Activate(false);
    }

    // Player Input
    private void OnOption1()
    {
        if (_playerController == null || _orderServed) return;

        Menu_Activate(!_menuOn);
        Serve_Order();
    }
    private void OnOption2()
    {
        if (_playerController == null || _orderServed) return;

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

    private IEnumerator Serve_Animation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _currentFoodIcon.Assign(_orderFood.foodScrObj.eatSprite);

        yield return new WaitForSeconds(delayTime);
        _currentFoodIcon.Assign(null);

        yield return new WaitForSeconds(delayTime);
        _calculateComplete = true;
        _currentFoodIcon.Assign(_coinSprite);
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
        if (_orderServed) return;

        Player_Interaction player = _playerController.playerInteraction;

        if (player.currentFood == null) return;

        Food_ScrObj playerFood = player.currentFood.foodScrObj;

        if (playerFood != _orderFood.foodScrObj) return;

        _currentFoodIcon.gameObject.SetActive(true);
        _currentFoodIcon.Assign(_orderFood.foodScrObj.sprite);

        _orderServed = true;
        player.Empty_CurrentFood();

        StartCoroutine(Serve_Animation(1));
    }
    private void Pay_Order()
    {
        _playerController.currentCoin++;
        _currentFoodIcon.Clear();

        Reset_Order();
    }
    private void Reset_Order()
    {
        _orderFood = null;
        _orderServed = false;
        _calculateComplete = false;
    }
}
