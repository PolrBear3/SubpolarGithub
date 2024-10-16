using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;


    // UnityEngine
    private void Awake()
    {
        _stationController = gameObject.GetComponent<Station_Controller>();
    }

    public void OnDestroy()
    {
        _stationController.Action1_Event -= Basic_SwapFood;
        _stationController.Action2_Event -= Merge_Food;
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
        Data_Controller data = _stationController.mainController.dataController;
        FoodData_Controller playerFoodIcon = _stationController.detection.player.foodIcon;

        Food_ScrObj cookedFood = data.CookedFood(_stationController.Food_Icon(), playerFoodIcon);

        // if cooked food is not available, swap only
        if (cookedFood == null)
        {
            Basic_SwapFood();
            return;
        }

        Action_Bubble bubble = _stationController.ActionBubble();

        bubble.Update_Bubble(_stationController.Food_Icon().currentData.foodScrObj, cookedFood);

        _stationController.PlayerInput_Activation(true);
        _stationController.Action1_Event += Basic_SwapFood;
        _stationController.Action2_Event += Merge_Food;
    }

    public void UnInteract()
    {
        Action_Bubble bubble = _stationController.ActionBubble();

        if (bubble != null) bubble.Toggle(false);

        _stationController.PlayerInput_Activation(false);
        _stationController.Action1_Event -= Basic_SwapFood;
        _stationController.Action2_Event -= Merge_Food;
    }


    // Swap Current and Player Food
    public void Basic_SwapFood()
    {
        // swap data with player
        FoodData_Controller playerFoodIcon = _stationController.detection.player.foodIcon;
        _stationController.Food_Icon().Swap_Data(playerFoodIcon);

        // show player food data
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();

        // show table food data
        _stationController.Food_Icon().Show_Icon();
        _stationController.Food_Icon().Show_Condition();

        // play sound on non empty food swap
        if (playerFoodIcon.hasFood || _stationController.Food_Icon().hasFood)
        {
            Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
        }

        UnInteract();
    }


    // Merge Current and Player Food
    private void Merge_Food()
    {
        Data_Controller data = _stationController.mainController.dataController;
        FoodData_Controller playerFoodIcon = _stationController.detection.player.foodIcon;

        Food_ScrObj cookedFood = data.CookedFood(_stationController.Food_Icon(), playerFoodIcon);

        // clear player food
        playerFoodIcon.Set_CurrentData(null);

        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();

        // assign new cooked food to this table
        _stationController.Food_Icon().Set_CurrentData(new FoodData(cookedFood));

        _stationController.Food_Icon().Show_Icon();
        _stationController.Food_Icon().Show_Condition();

        // unlock if cooked food
        ArchiveMenu_Controller menu = _stationController.mainController.currentVehicle.menu.archiveMenu;

        menu.Archive_Food(cookedFood);
        menu.Unlock_FoodIngredient(cookedFood);
        menu.controller.slotsController.Foods_ToggleLock(menu.currentDatas, cookedFood, data.Is_RawFood(cookedFood));

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_merge", transform.position);

        UnInteract();
    }
}