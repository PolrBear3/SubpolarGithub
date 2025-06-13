using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour, IInteractable
{
    [Space(20)]
    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;
    
    [Space(20)]
    [SerializeField] private Ability_ScrObj _mergeAbility;


    // UnityEngine
    private void Awake()
    {
        _stationController = gameObject.GetComponent<Station_Controller>();
    }

    public void Start()
    {
        _stationController.detection.ExitEvent += UnInteract;
        _stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;
    }

    public void OnDestroy()
    {
        _stationController.detection.ExitEvent -= UnInteract;
        _stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 -= Basic_SwapFood;
        input.OnAction2 -= Merge_Food;
    }


    // IInteractable
    public void Interact()
    {
        if (MergedFood() == null)
        {
            Basic_SwapFood();
            return;
        }

        Action_Bubble bubble = _stationController.ActionBubble();

        if (bubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        FoodData_Controller foodIcon = _stationController.Food_Icon();
        Sprite leftSprite = _stationController.stationScrObj.dialogIcon;

        if (foodIcon.hasFood)
        {
            leftSprite = foodIcon.currentData.foodScrObj.sprite;
        }

        bubble.Update_Bubble(leftSprite, MergedFood().sprite);

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 += Basic_SwapFood;
        input.OnAction2 += Merge_Food;
    }

    public void Hold_Interact()
    {
        Transfer_CurrentFood();
    }

    public void UnInteract()
    {
        if (_stationController.movement.enabled) return;

        Action_Bubble bubble = _stationController.ActionBubble();

        if (bubble != null) bubble.Toggle(false);

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 -= Basic_SwapFood;
        input.OnAction2 -= Merge_Food;
    }


    // Functions
    public void Basic_SwapFood()
    {
        FoodData_Controller playerFoodIcon = Main_Controller.instance.Player().foodIcon;

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
        FoodData playerData = Main_Controller.instance.Player().foodIcon.currentData;

        Data_Controller data = Main_Controller.instance.dataController;
        List<FoodData> ingredientDatas = new() { tableData, playerData };

        return data.Food(ingredientDatas);
    }

    private void Merge_Food()
    {
        Food_ScrObj mergedFood = MergedFood();

        FoodData_Controller playerFoodIcon = Main_Controller.instance.Player().foodIcon;
        FoodData_Controller stationFoodIcon = stationController.Food_Icon();

        bool bothHaveFood = playerFoodIcon.hasFood && stationFoodIcon.hasFood;

        // player food assign
        playerFoodIcon.Set_CurrentData(null);

        playerFoodIcon.Show_Icon();
        playerFoodIcon.Toggle_SubDataBar(true);
        playerFoodIcon.Show_Condition();

        // table food assign
        stationFoodIcon.Update_AllDatas(null);
        stationFoodIcon.Set_CurrentData(new(mergedFood));

        stationFoodIcon.Show_Icon();
        stationFoodIcon.Toggle_SubDataBar(true);
        stationFoodIcon.Show_Condition();

        // unlock if cooked food
        Main_Controller main = Main_Controller.instance;
        Data_Controller data = main.dataController;

        ArchiveMenu_Controller menu = main.currentVehicle.menu.archiveMenu;

        menu.Unlock_BookmarkToggle(menu.Archive_Food(mergedFood), true);
        menu.Unlock_FoodIngredient(mergedFood, 1);
        
        // quest
        TutorialQuest_Controller.instance.Complete_Quest("Make" + mergedFood.name, 1);

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 2);

        UnInteract();
        AbilityManager.IncreasePoint(1);

        // durability
        Station_Maintenance maintenance = stationController.maintenance;
            
        maintenance.Update_Durability(-1);
        maintenance.Update_DurabilityBreak();
    }


    public void Transfer_CurrentFood()
    {
        FoodData_Controller tableIcon = _stationController.Food_Icon();
        FoodData_Controller playerIcon = Main_Controller.instance.Player().foodIcon;

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