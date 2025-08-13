using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Table : Stack_Table
{
    [Space(20)] 
    [SerializeField] private Sprite[] _sprites;
    
    [Space(20)]
    [SerializeField][Range(0, 100)] private int _searchTime;

    private NPC_Controller _targetNPC;
    private Coroutine _coroutine;


    // UnityEngine
    private new void Start()
    {
        base.Start();
        
        Update_Sprite();
        Take_FoodOrder(stationController.movement.enabled == false);

        // subscriptions
        Main_Controller main = Main_Controller.instance;
        
        main.OnFoodBookmark += Take_FoodOrder;
        main.OnFoodBookmark += Update_Sprite;
        
        stationController.maintenance.OnDurabilityBreak += Update_TargetNPC;

        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract += Update_Sprite;
        interactable.OnInteract += Take_FoodOrder;
        
        interactable.OnHoldInteract += Update_Sprite;
        interactable.OnHoldInteract += Take_FoodOrder;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        Main_Controller main = Main_Controller.instance;
        
        main.OnFoodBookmark -= Take_FoodOrder;
        main.OnFoodBookmark -= Update_Sprite;

        stationController.maintenance.OnDurabilityBreak -= Update_TargetNPC;
        
        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract -= Update_Sprite;
        interactable.OnInteract -= Take_FoodOrder;
        
        interactable.OnHoldInteract -= Update_Sprite;
        interactable.OnHoldInteract -= Take_FoodOrder;
    }

    
    // Main Control
    private void Update_Sprite()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (foodIcon.hasFood == false || Main_Controller.instance.data.bookmarkedFoods.Count == 0)
        {
            stationController.spriteRenderer.sprite = _sprites[0];
            return;
        }
        stationController.spriteRenderer.sprite = _sprites[1];
    }
    
    
    // NPC
    private List<NPC_Controller> ConditionMatch_SortedNPCs(List<NPC_Controller> targetNPCs)
    {
        FoodData_Controller tableFoodIcon = stationController.Food_Icon();
        if (tableFoodIcon.hasFood == false) return targetNPCs;

        List<FoodCondition_Data> tableConditions = tableFoodIcon.currentData.conditionDatas;

        targetNPCs.Sort((npc1, npc2) =>
        {
            int match1 = npc1.foodIcon.currentData.ConditionLevel_MatchCount(tableConditions);
            int match2 = npc2.foodIcon.currentData.ConditionLevel_MatchCount(tableConditions);

            return match2.CompareTo(match1);
        });

        return targetNPCs;
    }

    private NPC_Controller FoodOrder_NPC()
    {
        Location_Controller location = Main_Controller.instance.currentLocation;
        
        List<NPC_Controller> foodOrderNPCs = location.foodOrderNPCs;
        List<NPC_Controller> orderNPCs = new();

        for (int i = 0; i < foodOrderNPCs.Count; i++)
        {
            if (!foodOrderNPCs[i].movement.roamActive) continue;
            
            NPC_FoodInteraction foodInteraction = foodOrderNPCs[i].foodInteraction;

            if (foodInteraction == null) continue;
            if (foodInteraction.timeCoroutine == null) continue;
            if (stationController.Food_Icon().Has_SameFood(foodOrderNPCs[i].foodIcon.currentData.foodScrObj) == false) continue;

            orderNPCs.Add(foodOrderNPCs[i]);
        }

        if (orderNPCs.Count == 0) return null;
        return ConditionMatch_SortedNPCs(orderNPCs)[0];
    }


    private void Take_FoodOrder(bool activate)
    {
        if (activate == false) return;

        Take_FoodOrder();
    }

    private void Take_FoodOrder()
    {
        if (_coroutine != null) return;
        
        if (stationController.Food_Icon().hasFood == false) return;
        if (Main_Controller.instance.data.bookmarkedFoods.Count == 0) return;

        _coroutine = StartCoroutine(Take_FoodOrder_Coroutine());
    }
    private IEnumerator Take_FoodOrder_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (stationController.Food_Icon().hasFood && Main_Controller.instance.data.bookmarkedFoods.Count == 0)
        {
            yield return new WaitForSeconds(_searchTime);

            _targetNPC = FoodOrder_NPC();
            if (_targetNPC == null || _targetNPC.gameObject == null) continue;
            
            NPC_Movement npcMovement = _targetNPC.movement;
            if (npcMovement.isLeaving) continue;

            npcMovement.Stop_FreeRoam();
            npcMovement.Assign_TargetPosition(transform.position);
            
            while (!npcMovement.At_TargetPosition(transform.position))  yield return null;
            
            NPC_FoodInteraction npcFoodInteraction = _targetNPC.foodInteraction;
            FoodData transferData = new(_targetNPC.foodIcon.currentData);

            if (stationController.Food_Icon().hasFood == false || npcFoodInteraction.Transfer_FoodOrder(transferData) == false)
            {
                Update_TargetNPC();
                continue;
            }
            
            foodIcon.Set_CurrentData(null);
            foodIcon.Show_Icon();
            foodIcon.Show_Condition();
            AmountBar_Toggle();
            
            Audio_Controller.instance.Play_OneShot(gameObject, 1);
            
            Station_Maintenance maintenance = stationController.maintenance;
            
            maintenance.Update_Durability(-1);
            maintenance.Update_DurabilityBreak();
        }

        _coroutine = null;
        yield break;
    }


    private void Update_TargetNPC()
    {
        if (_targetNPC == null) return;
        
        _targetNPC.foodInteraction.Update_RoamArea();
        _targetNPC = null;
    }
}