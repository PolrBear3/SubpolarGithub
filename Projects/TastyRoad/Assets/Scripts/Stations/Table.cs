using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    [SerializeField] private Action_Bubble _actionBubble;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Station_Controller controller)) { _stationController = controller; }
    }



    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
    }



    // IInteractable
    public void Interact()
    {
        // if cooked food is not available, swap only
        if (CookedFood() == null)
        {
            Basic_SwapFood();
            return;
        }

        _actionBubble.Update_Bubble(_stationController.Food_Icon().currentFoodData.foodScrObj, CookedFood());

        _stationController.PlayerInput_Activation(true);
        _stationController.Action1_Event += Basic_SwapFood;
        _stationController.Action2_Event += Merge_Food;
    }

    public void UnInteract()
    {
        if (_actionBubble != null) _actionBubble.Toggle(false);

        _stationController.PlayerInput_Activation(false);
        _stationController.Action1_Event -= Basic_SwapFood;
        _stationController.Action2_Event -= Merge_Food;
    }



    // Get Available Cooked Food Merged with Table and Player Food
    private Food_ScrObj CookedFood()
    {
        FoodData_Controller icon = _stationController.Food_Icon();

        if (_stationController.detection.player.foodIcon.currentFoodData.foodScrObj == null || icon.currentFoodData.foodScrObj == null) return null;

        FoodData playerFoodData = _stationController.detection.player.foodIcon.currentFoodData;

        // search for cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(playerFoodData);
        ingredients.Add(icon.currentFoodData);

        Food_ScrObj cookedFood = _stationController.mainController.dataController.CookedFood(ingredients);

        return cookedFood;
    }

    // Swap Current and Player Food
    public void Basic_SwapFood()
    {
        FoodData_Controller icon = _stationController.Food_Icon();
        FoodData_Controller playerIcon = _stationController.detection.player.foodIcon;

        Food_ScrObj ovenFood = icon.currentFoodData.foodScrObj;
        List<FoodState_Data> tableStateDatas = new(icon.currentFoodData.stateData);

        Food_ScrObj playerFood = playerIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> playerStateData = new(playerIcon.currentFoodData.stateData);

        icon.Assign_State(playerStateData);
        icon.Assign_Food(playerFood);

        playerIcon.Assign_Food(ovenFood);
        playerIcon.Assign_State(tableStateDatas);

        // reset action
        UnInteract();
    }

    // Merge Current and Player Food
    private void Merge_Food()
    {
        Main_Controller main = _stationController.mainController;

        FoodData_Controller icon = _stationController.Food_Icon();
        FoodData_Controller playerIcon = _stationController.detection.player.foodIcon;

        // get cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(playerIcon.currentFoodData);
        ingredients.Add(icon.currentFoodData);

        Food_ScrObj cookedFood = main.dataController.CookedFood(ingredients);

        // add cookedFood to archive
        main.AddFood_toArhive(cookedFood);

        // change table food to cooked food
        icon.Assign_Food(cookedFood);
        icon.Clear_State();

        // empty player food
        playerIcon.Clear_Food();
        playerIcon.Clear_State();

        // reset action
        UnInteract();
    }
}