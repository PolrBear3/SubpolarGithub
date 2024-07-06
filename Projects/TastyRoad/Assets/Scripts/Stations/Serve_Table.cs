using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serve_Table : Table, IInteractable
{
    private NPC_Controller _currentNPC;
    public NPC_Controller currentNPC => _currentNPC;

    private Coroutine _coroutine;

    [SerializeField] private FoodData_Controller _foodOrderPreview;


    // UnityEngine
    private void Start()
    {
        Main_Controller.OrderOpen_ToggleEvent += Find_AttractedNPC;
        Main_Controller.OrderOpen_ToggleEvent += CloseOrder;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        Main_Controller.OrderOpen_ToggleEvent -= Find_AttractedNPC;
        Main_Controller.OrderOpen_ToggleEvent -= CloseOrder;
    }


    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();
        Serve_FoodOrder_toNPC();
    }


    // Gets
    private List<Serve_Table> Other_ServeTables()
    {
        List<Station_Controller> currentStations = stationController.mainController.currentStations;
        List <Serve_Table> others = new();

        for (int i = 0; i < currentStations.Count; i++)
        {
            // if currentStations[i] is this serve table, continue
            if (currentStations[i] == stationController) continue;

            // if currentStations[i] is not serve table, continue
            if (currentStations[i].stationScrObj != stationController.stationScrObj) continue;

            others.Add(currentStations[i].GetComponent<Serve_Table>());
        }

        return others;
    }

    private OrderStand Opened_OrderStand()
    {
        Station_ScrObj orderStandScrObj = stationController.mainController.dataController.Station_ScrObj("Order Stand");
        List<Station_Controller> orderStands = stationController.mainController.CurrentStations(orderStandScrObj);

        for (int i = 0; i < orderStands.Count; i++)
        {
            OrderStand orderStand = orderStands[i].GetComponent<OrderStand>();

            if (orderStand.orderOpen == false) continue;
            return orderStand;
        }

        return null;
    }


    // Checks
    private bool Other_ServeTable_OrderTaken(NPC_Controller npc)
    {
        for (int i = 0; i < Other_ServeTables().Count; i++)
        {
            if (npc != Other_ServeTables()[i].currentNPC) continue;
            return true;
        }
        return false;
    }

    private bool Serve_Available()
    {
        // check if food is placed
        if (stationController.Food_Icon().hasFood == false) return false;

        // check if correct food
        if (stationController.Food_Icon().currentData.foodScrObj != _currentNPC.foodIcon.currentData.foodScrObj) return false;

        // check if food is already served
        if (_currentNPC.interaction.servedFoodData != null) return false;

        //
        return true;
    }


    // Functions
    private void Set_AttractedNPC()
    {
        List<NPC_Controller> allNPCs = stationController.mainController.All_NPCs();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].foodIcon.hasFood == false) continue;
            if (allNPCs[i].interaction.servedFoodData != null) continue;
            if (Other_ServeTable_OrderTaken(allNPCs[i]) == true) continue;

            // set current npc
            _currentNPC = allNPCs[i];

            //
            FoodOrder_PreviewUpdate();

            return;
        }
    }

    private void FoodOrder_PreviewUpdate()
    {
        if (_currentNPC == null)
        {
            _foodOrderPreview.Set_CurrentData(null);
            _foodOrderPreview.Show_Icon();
            _foodOrderPreview.Show_Condition();

            return;
        }

        _foodOrderPreview.Set_CurrentData(_currentNPC.foodIcon.currentData);
        _foodOrderPreview.Show_Icon();
        _foodOrderPreview.Show_Condition();

        LeanTween.alpha(_foodOrderPreview.gameObject, 0.3f, 0f);
    }


    private void Find_AttractedNPC()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (Main_Controller.orderOpen == false) return;

        _coroutine = StartCoroutine(Find_AttractedNPC_Coroutine());
    }
    private IEnumerator Find_AttractedNPC_Coroutine()
    {
        while (_currentNPC == null)
        {
            Set_AttractedNPC();

            yield return null;
        }

        _coroutine = null;
        yield break;
    }

    private void Serve_FoodOrder_toNPC()
    {
        //
        if (_coroutine != null) return;

        // check if order is currently open
        if (Main_Controller.orderOpen == false) return;

        // check if food order is set
        if (_currentNPC == null) return;

        // condition check
        if (Serve_Available() == false) return;

        //
        _coroutine = StartCoroutine(Serve_FoodOrder_toNPC_Coroutine());
    }
    private IEnumerator Serve_FoodOrder_toNPC_Coroutine()
    {
        NPC_Interaction interaction = _currentNPC.interaction;
        NPC_Movement movement = _currentNPC.movement;

        // interact lock
        interaction.InteractLock_Toggle(true);

        // set npc destination to this serve table
        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(transform.position);

        // wait until npc arrives
        while (movement.At_TargetPosition(transform.position) == false) yield return null;

        // interact unlock
        interaction.InteractLock_Toggle(false);

        // set npc roam area to opened order stand
        movement.Free_Roam(Opened_OrderStand().orderingArea, 0f);

        // condition check
        if (Serve_Available() == false)
        {
            _coroutine = null;
            yield break;
        }

        // serve npc food
        interaction.Serve_FoodOrder(stationController.Food_Icon().currentData);

        // empty current food
        stationController.Food_Icon().Set_CurrentData(null);
        stationController.Food_Icon().Show_Icon();
        stationController.Food_Icon().Show_Condition();

        // reset data
        _currentNPC = null;

        //
        FoodOrder_PreviewUpdate();

        Find_AttractedNPC();
    }


    private void CloseOrder()
    {
        if (Main_Controller.orderOpen == true) return;
        if (_currentNPC == null) return;

        SpriteRenderer locationRoamArea = stationController.mainController.currentLocation.roamArea;

        // npc returns to current location roam area
        _currentNPC.movement.Free_Roam(locationRoamArea, _currentNPC.interaction.Random_RoamDelayTime());

        // reset
        _currentNPC = null;

        //
        FoodOrder_PreviewUpdate();
    }
}
