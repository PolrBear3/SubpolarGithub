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

    public void Start()
    {
        _stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;
    }

    public void OnDestroy()
    {
        _stationController.Action1_Event -= Basic_SwapFood;
        _stationController.Action2_Event -= FoodMerge;

        _stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;
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
        if (MergedFood() == null)
        {
            Basic_SwapFood();
            return;
        }

        FoodData_Controller foodIcon = _stationController.Food_Icon();
        Sprite leftSprite = _stationController.stationScrObj.dialogIcon;

        if (foodIcon.hasFood)
        {
            leftSprite = foodIcon.currentData.foodScrObj.sprite;
        }

        Action_Bubble bubble = _stationController.ActionBubble();
        bubble.Update_Bubble(leftSprite, MergedFood().sprite);

        _stationController.PlayerInput_Activation(true);
        _stationController.Action1_Event += Basic_SwapFood;
        _stationController.Action2_Event += FoodMerge;
    }

    public void Hold_Interact()
    {
        Transfer_CurrentFood();
    }

    public void UnInteract()
    {
        Action_Bubble bubble = _stationController.ActionBubble();

        if (bubble != null) bubble.Toggle(false);

        _stationController.PlayerInput_Activation(false);

        _stationController.Action1_Event -= Basic_SwapFood;
        _stationController.Action2_Event -= FoodMerge;
    }


    // Functions
    public void Basic_SwapFood()
    {
        FoodData_Controller playerFoodIcon = _stationController.detection.player.foodIcon;

        // swap data with player
        _stationController.Food_Icon().Swap_Data(playerFoodIcon);

        // show player food data
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();
        playerFoodIcon.Toggle_SubDataBar(true);

        // show table food data
        _stationController.Food_Icon().Show_Icon();
        _stationController.Food_Icon().Show_Condition();

        // play sound on non empty food swap
        if (playerFoodIcon.hasFood || _stationController.Food_Icon().hasFood)
        {
            Audio_Controller.instance.Play_OneShot(gameObject, 1);
        }

        UnInteract();
    }


    private Food_ScrObj MergedFood()
    {
        FoodData tableData = _stationController.Food_Icon().currentData;
        FoodData playerData = _stationController.detection.player.foodIcon.currentData;

        Data_Controller data = Main_Controller.instance.dataController;
        List<FoodData> ingredientDatas = new() { tableData, playerData };

        return data.Food(ingredientDatas);
    }

    private void FoodMerge()
    {
        Food_ScrObj mergedFood = MergedFood();

        FoodData_Controller playerFoodIcon = _stationController.detection.player.foodIcon;
        FoodData_Controller stationFoodIcon = stationController.Food_Icon();

        bool bothHaveFood = playerFoodIcon.hasFood && stationFoodIcon.hasFood;

        // player food assign
        playerFoodIcon.Set_CurrentData(null);

        playerFoodIcon.Show_Icon();
        playerFoodIcon.Toggle_SubDataBar(true);
        playerFoodIcon.Show_Condition();

        // table food assign
        stationFoodIcon.Set_CurrentData(null);
        stationFoodIcon.Set_CurrentData(new FoodData(mergedFood));

        stationFoodIcon.Show_Icon();
        stationFoodIcon.Toggle_SubDataBar(true);
        stationFoodIcon.Show_Condition();

        // unlock if cooked food
        Data_Controller data = Main_Controller.instance.dataController;
        ArchiveMenu_Controller menu = _stationController.mainController.currentVehicle.menu.archiveMenu;

        menu.Archive_Food(mergedFood);
        menu.Unlock_FoodIngredient(mergedFood, 1);
        menu.controller.slotsController.Foods_ToggleLock(menu.currentDatas, mergedFood, data.Is_RawFood(mergedFood));

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 2);

        UnInteract();

        AbilityManager.IncreasePoint(1);

        // durability
        _stationController.data.Update_Durability(-1);
        _stationController.maintenance.Update_DurabilityBreak();
    }


    public void Transfer_CurrentFood()
    {
        FoodData_Controller tableIcon = _stationController.Food_Icon();
        FoodData_Controller playerIcon = _stationController.detection.player.foodIcon;

        // check if table empty or player full amount
        if (tableIcon.hasFood == false || playerIcon.DataCount_Maxed())
        {
            Basic_SwapFood();
            return;
        }

        // player
        playerIcon.Set_CurrentData(tableIcon.currentData);

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        // table
        tableIcon.Set_CurrentData(null);

        tableIcon.Show_Icon();
        tableIcon.Show_Condition();

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }

    public void Drop_CurrentFood()
    {
        _stationController.itemDropper.Drop_Food(_stationController.Food_Icon().AllDatas());
    }
}