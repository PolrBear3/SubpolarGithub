using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Station_Controller _stationController;
    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;
    [SerializeField] private Action_Bubble _actionBubble;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Station_Controller controller)) { _stationController = controller; }
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }

    // InputSystem
    private void OnAction1()
    {
        if (_stationController.movement.enabled == true) return;
        Swap_Food();
    }

    private void OnAction2()
    {
        if (_stationController.movement.enabled == true) return;
        Merge_Food();
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_stationController.movement.enabled == true) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _stationController.PlayerInput_Toggle(false);
        _actionBubble.Toggle_Off();
    }

    // IInteractable
    public void Interact()
    {
        // if cooked food is not available, swap only
        if (CookedFood() == null)
        {
            Swap_Food();
            return;
        }

        _stationController.PlayerInput_Toggle(!_actionBubble.bubbleOn);
        _actionBubble.Update_Bubble(_foodIcon.currentFoodData.foodScrObj, CookedFood());
    }

    public void UnInteract()
    {
        if (_stationController.movement.enabled == true) return;

        _stationController.PlayerInput_Toggle(false);
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

        Food_ScrObj cookedFood = _stationController.mainController.dataController.CookedFood(ingredients);

        return cookedFood;
    }

    // Swap Current and Player Food
    private void Swap_Food()
    {
        _actionBubble.Toggle_Off();

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
        _actionBubble.Toggle_Off();

        Main_Controller main = _stationController.mainController;
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        // get cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(playerIcon.currentFoodData);
        ingredients.Add(_foodIcon.currentFoodData);

        Food_ScrObj cookedFood = main.dataController.CookedFood(ingredients);

        // add cookedFood to archive
        main.AddFood_toArhive(cookedFood);

        // change table food to cooked food
        _foodIcon.Assign_Food(cookedFood);
        _foodIcon.Clear_State();

        // empty player food
        playerIcon.Clear_Food();
        playerIcon.Clear_State();
    }
}