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
        Basic_SwapFood();

        // if cooked food is not available, swap only
        /*
        if (CookedFood() == null)
        {
            Basic_SwapFood();
            return;
        }
        */

        // _actionBubble.Update_Bubble(_stationController.Food_Icon().currentData.foodScrObj, CookedFood());

        /*
        _stationController.PlayerInput_Activation(true);
        _stationController.Action1_Event += Basic_SwapFood;
        _stationController.Action2_Event += Merge_Food;
        */
    }

    public void UnInteract()
    {
        if (_actionBubble != null) _actionBubble.Toggle(false);

        _stationController.PlayerInput_Activation(false);
        _stationController.Action1_Event -= Basic_SwapFood;
        _stationController.Action2_Event -= Merge_Food;
    }



    // Get Available Cooked Food Merged with Table and Player Food
    /*
    private Food_ScrObj CookedFood()
    {
        FoodData_Controller icon = _stationController.Food_Icon();

        if (_stationController.detection.player.foodIcon.currentData.foodScrObj == null || icon.currentData.foodScrObj == null) return null;

        FoodData playerFoodData = _stationController.detection.player.foodIcon.currentData;

        // search for cooked food
        List<FoodData> ingredients = new();

        ingredients.Add(playerFoodData);
        ingredients.Add(icon.currentData);

        Food_ScrObj cookedFood = _stationController.mainController.dataController.CookedFood(ingredients);

        return cookedFood;
    }
    */

    // Swap Current and Player Food
    public void Basic_SwapFood()
    {
        FoodData_Controller playerController = _stationController.detection.player.foodIcon;
        _stationController.Food_Icon().Swap_Data(playerController);

        playerController.Show_Icon();
        playerController.Show_Condition();

        _stationController.Food_Icon().Show_Icon();
        _stationController.Food_Icon().Show_Condition();
    }

    // Merge Current and Player Food
    private void Merge_Food()
    {

    }
}