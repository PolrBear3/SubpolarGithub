using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;
    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;
    [SerializeField] private Action_Bubble _actionBubble;

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_detection.Has_Player() == false)
        {
            _actionBubble.Toggle_Off();
        }
    }

    // IInteractable
    public void Interact()
    {
        Update_ActionBubble();

        // if cooked food is not available, swap only
        if (CookedFood() == null)
        {
            Swap_Food();
        }
    }

    // InputSystem
    private void OnAction1()
    {
        Swap_Food();

        _actionBubble.Toggle_Off();
    }

    private void OnAction2()
    {
        Merge_Food();

        _actionBubble.Toggle_Off();
    }

    // Get Available Cooked Food Merged with Table and Player Food
    private Food_ScrObj CookedFood()
    {
        if (_detection.player.foodIcon.currentFoodData.foodScrObj == null || _foodIcon.currentFoodData.foodScrObj == null) return null;

        FoodData playerFoodData = _detection.player.foodIcon.currentFoodData;

        // search for cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(playerFoodData);
        ingredients.Add(_foodIcon.currentFoodData);

        Food_ScrObj cookedFood = _mainController.dataController.CookedFood(ingredients);

        return cookedFood;
    }

    // Action Bubble Update 
    private void Update_ActionBubble()
    {
        // if player or table does not have food
        if (CookedFood() == null) return;

        _actionBubble.Update_Bubble(_foodIcon.currentFoodData.foodScrObj.sprite, CookedFood().sprite);
    }

    // Swap Current and Player Food
    private void Swap_Food()
    {
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        Food_ScrObj ovenFood = _foodIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> ovenStateData = new(_foodIcon.currentFoodData.stateData);

        Food_ScrObj playerFood = playerIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> playerStateData = new(playerIcon.currentFoodData.stateData);

        _foodIcon.Assign_Food(playerFood);
        _foodIcon.Assign_State(playerStateData);

        playerIcon.Assign_Food(ovenFood);
        playerIcon.Assign_State(ovenStateData);
    }

    // Merge Current and Player Food
    private void Merge_Food()
    {
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        // get cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(playerIcon.currentFoodData);
        ingredients.Add(_foodIcon.currentFoodData);

        Food_ScrObj cookedFood = _mainController.dataController.CookedFood(ingredients);

        // change table food to cooked food
        _foodIcon.Assign_Food(cookedFood);
        _foodIcon.Clear_State();

        // empty player food
        playerIcon.Clear_Food();
        playerIcon.Clear_State();
    }
}