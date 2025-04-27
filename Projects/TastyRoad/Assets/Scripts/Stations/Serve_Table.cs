using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serve_Table : Stack_Table, IInteractable
{
    [Header("")]
    [SerializeField][Range(0, 100)] private int _searchTime;

    private Coroutine _coroutine;


    // UnityEngine
    private new void Start()
    {
        base.Start();

        Take_FoodOrder(stationController.movement.enabled == false);

        // subscriptions
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
    }


    // IInteractable
    public new void Interact()
    {
        base.Interact();

        Take_FoodOrder();
    }


    // NPC
    private List<NPC_Controller> ConditionLevelMatch_NPCs(List<NPC_Controller> targetNPCs)
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

    private List<NPC_Controller> FoodOrder_NPCs()
    {
        Location_Controller location = Main_Controller.instance.currentLocation;
        
        List<NPC_Controller> foodOrderNPCs = location.foodOrderNPCs;
        List<NPC_Controller> orderNPCs = new();

        for (int i = 0; i < foodOrderNPCs.Count; i++)
        {
            NPC_Controller npc = foodOrderNPCs[i];

            if (npc.foodInteraction == null) continue;
            if (npc.foodInteraction.timeCoroutine == null) continue;
            if (stationController.Food_Icon().Has_SameFood(npc.foodIcon.currentData.foodScrObj) == false) continue;

            orderNPCs.Add(npc);
        }

        return ConditionLevelMatch_NPCs(orderNPCs);
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

        _coroutine = StartCoroutine(Take_FoodOrder_Coroutine());
    }
    private IEnumerator Take_FoodOrder_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (true)
        {
            yield return new WaitForSeconds(_searchTime);
            
            if (FoodOrder_NPCs().Count <= 0)
            {
                if (foodIcon.hasFood == false) break;

                yield return new WaitForSeconds(_searchTime);
                continue;
            }

            for (int i = 0; i < FoodOrder_NPCs().Count; i++)
            {
                NPC_Controller targetNPC = FoodOrder_NPCs()[i];
                NPC_Movement movement = targetNPC.movement;

                movement.Stop_FreeRoam();
                movement.Assign_TargetPosition(transform.position);

                while (movement.At_TargetPosition(transform.position) == false) yield return null;

                Food_ScrObj orderFood = targetNPC.foodIcon.currentData.foodScrObj;
                NPC_FoodInteraction interaction = targetNPC.foodInteraction;

                FoodData transferData = foodIcon.Empty_TargetData(orderFood);

                if (interaction.Transfer_FoodOrder(transferData))
                {
                    Audio_Controller.instance.Play_OneShot(gameObject, 1);
                    break;
                }

                interaction.Update_RoamArea();
            }

            foodIcon.Show_Icon();
            foodIcon.Show_Condition();
            AmountBar_Toggle();
        }

        _coroutine = null;
        yield break;
    }
}