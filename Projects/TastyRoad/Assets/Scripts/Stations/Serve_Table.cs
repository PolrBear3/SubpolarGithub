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

        Take_FoodOrder();

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
    private List<NPC_Controller> ClosestFiltered_NPCs(List<NPC_Controller> targetNPCs)
    {
        targetNPCs.Sort((npc1, npc2) =>
        {
            float distance1 = Vector2.Distance(transform.position, npc1.transform.position);
            float distance2 = Vector2.Distance(transform.position, npc2.transform.position);
            return distance1.CompareTo(distance2);
        });

        return targetNPCs;
    }

    private List<NPC_Controller> FoodOrder_NPCs()
    {
        List<NPC_Controller> allNPCs = stationController.mainController.All_NPCs();
        List<NPC_Controller> orderNPCs = new();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].gameObject.TryGetComponent(out NPC_FoodInteraction foodInteract) == false) continue;

            if (foodInteract.timeCoroutine == null) continue;
            if (stationController.Food_Icon().Has_SameFood(allNPCs[i].foodIcon.currentData.foodScrObj) == false) continue;

            orderNPCs.Add(allNPCs[i]);
        }

        return ClosestFiltered_NPCs(orderNPCs);
    }


    private void Take_FoodOrder()
    {
        if (stationController.Food_Icon().hasFood == false) return;

        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = null;

        _coroutine = StartCoroutine(Take_FoodOrder_Coroutine());
    }
    private IEnumerator Take_FoodOrder_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (true)
        {
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