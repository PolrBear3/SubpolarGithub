using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    private Detection_Controller _detection;
    [SerializeField] private FoodData_Controller _foodIcon;

    [Header("Start Data")]
    [SerializeField] private Food_ScrObj _food;
    [SerializeField] private int _amount;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    private void Start()
    {
        Assign_FoodData();
    }

    // IInteractable
    public void Interact()
    {
        Give_Food();
    }

    // Set Food Data at Start
    private void Assign_FoodData()
    {
        _foodIcon.Assign_Food(_food);
        _foodIcon.Update_Amount(_amount);
    }

    // Give Food to Player
    private void Give_Food()
    {
        FoodData_Controller player = _detection.player.foodIcon;

        /*
        bool isSameFood = player.currentFoodData.foodScrObj == _foodIcon.currentFoodData.foodScrObj;
        bool isSameState = _foodIcon.Same_StateData(player.currentFoodData.stateData);
        */

        if (player.hasFood == false)
        {
            _foodIcon.Update_Amount(-1);
            player.Assign_Food(_food);
        }
    }
}