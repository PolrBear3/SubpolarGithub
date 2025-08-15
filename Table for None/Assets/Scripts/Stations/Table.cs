using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Table : MonoBehaviour
{
    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    
    [Space(20)]
    [SerializeField] private FoodList_Controller _foodListController;

    [SerializeField] private Ability_ScrObj _foodPrinterAbility;


    // UnityEngine
    public void Awake()
    {
        _stationController = gameObject.GetComponent<Station_Controller>();
    }

    public void Start()
    {
        _stationController.detection.ExitEvent += UnInteract;
        _stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;

        IInteractable_Controller interactable = _stationController.iInteractable;
        FoodData_Controller stationFoodIcon = stationController.Food_Icon();
        
        interactable.OnTriggerInteract += stationFoodIcon.Show_Icon;
        interactable.OnTriggerInteract += stationFoodIcon.Show_Condition;
        
        interactable.OnInteract += Interact;
        interactable.OnUnInteract += UnInteract;

        interactable.OnAction1 += PlaceFood;
        interactable.OnAction2 += TakeFood;
    }

    public void OnDestroy()
    {
        _stationController.detection.ExitEvent -= UnInteract;
        _stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;

        IInteractable_Controller interactable = _stationController.iInteractable;

        interactable.OnInteract -= Interact;
        interactable.OnUnInteract -= UnInteract;

        interactable.OnAction1 += PlaceFood;
        interactable.OnAction2 += TakeFood;
        
        Input_Controller input = Input_Controller.instance;

        input.OnAction1 -= SwapFood;
        input.OnAction2 -= Merge_Food;
    }


    // IInteractable_Controller
    private void Interact()
    {
        if (MergedFood() == null)
        {
            SwapFood();
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

        input.OnAction1 += SwapFood;
        input.OnAction2 += Merge_Food;
    }

    private void UnInteract()
    {
        if (_stationController.movement.enabled) return;

        Action_Bubble bubble = _stationController.ActionBubble();

        if (bubble != null) bubble.Toggle(false);

        Input_Controller input = Input_Controller.instance;

        input.OnAction1 -= SwapFood;
        input.OnAction2 -= Merge_Food;
    }


    // Functions
    public void SwapFood()
    {
        FoodData_Controller stationIcon = _stationController.Food_Icon();
        FoodData_Controller playerFoodIcon = Main_Controller.instance.Player().foodIcon;

        // swap data with player
        stationIcon.Swap_Data(playerFoodIcon);

        // show player food data
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();
        playerFoodIcon.Toggle_SubDataBar(true);

        // show table food data
        stationIcon.Show_Icon();
        stationIcon.Show_Condition();

        // play sound on non empty food swap
        if (playerFoodIcon.hasFood || stationIcon.hasFood)
        {
            Audio_Controller.instance.Play_OneShot(gameObject, 1);
        }

        UnInteract();
    }

    public void PlaceFood()
    {
        Action_Bubble bubble = _stationController.ActionBubble();
        if (bubble != null && bubble.bubbleOn) return;

        FoodData_Controller foodIcon = _stationController.Food_Icon();
        FoodData_Controller playerIcon = Main_Controller.instance.Player().foodIcon;

        if (foodIcon.hasFood || playerIcon.hasFood == false)
        {
            SwapFood();
            return;
        }

        foodIcon.Set_CurrentData(new(playerIcon.currentData));
        foodIcon.Show_Icon();
        foodIcon.Show_Condition();
        
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);
    }

    public void TakeFood()
    {
        Action_Bubble bubble = _stationController.ActionBubble();
        if (bubble != null && bubble.bubbleOn) return;

        FoodData_Controller foodIcon = _stationController.Food_Icon();
        FoodData_Controller playerIcon = Main_Controller.instance.Player().foodIcon;

        if (foodIcon.hasFood == false || playerIcon.DataCount_Maxed())
        {
            SwapFood();
            return;
        }

        playerIcon.Set_CurrentData(new(foodIcon.currentData));
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);
        
        foodIcon.Set_CurrentData(null);
        foodIcon.Show_Icon();
        foodIcon.Show_Condition();
    }
    

    private Food_ScrObj MergedFood()
    {
        FoodData tableData = _stationController.Food_Icon().currentData;
        FoodData playerData = Main_Controller.instance.Player().foodIcon.currentData;

        return _foodListController.MergedFood(new List<FoodData>() { tableData, playerData });
    }

    private void Merge_Food()
    {
        Food_ScrObj mergedFood = MergedFood();

        Player_Controller player = Main_Controller.instance.Player();
        FoodData_Controller playerFoodIcon = player.foodIcon;
        FoodData_Controller stationFoodIcon = stationController.Food_Icon();

        bool bothHaveFood = playerFoodIcon.hasFood && stationFoodIcon.hasFood;

        // player food assign
        playerFoodIcon.Set_CurrentData(null);
        
        // duplicate Ability
        int duplicateAmount = player.abilityManager.data.Ability_ActivationCount(_foodPrinterAbility);

        for (int i = 0; i < duplicateAmount; i++)
        {
            if (playerFoodIcon.DataCount_Maxed()) break;
            playerFoodIcon.Set_CurrentData(new(mergedFood));
        }

        // player food icon update
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

        menu.Unlock_BookmarkToggle(menu.Archive_Food(mergedFood), data.CookedFood(mergedFood) != null);
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
    
    
    public void Drop_CurrentFood()
    {
        FoodData_Controller foodIcon = _stationController.Food_Icon();

        if (foodIcon.AllDatas().Count <= 0) return; 
        _stationController.itemDropper.Drop_Food(_stationController.Food_Icon().AllDatas());
    }
}