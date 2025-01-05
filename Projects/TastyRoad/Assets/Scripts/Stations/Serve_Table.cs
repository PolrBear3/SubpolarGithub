using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serve_Table : Table, IInteractable
{
    private NPC_Controller _currentNPC;
    public NPC_Controller currentNPC => _currentNPC;

    private Coroutine _coroutine;
    private Coroutine _checkCoroutine;

    [SerializeField] private FoodData_Controller _foodOrderPreview;


    // UnityEngine
    private new void Start()
    {
        // Main_Controller.OrderOpen_ToggleEvent += Find_AttractedNPC;
        // Main_Controller.OrderOpen_ToggleEvent += CloseOrder;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // Main_Controller.OrderOpen_ToggleEvent -= Find_AttractedNPC;
        // Main_Controller.OrderOpen_ToggleEvent -= CloseOrder;
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
        List<Serve_Table> others = new();

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
        // if (_currentNPC.interaction.servedFoodData != null) return false;

        //
        return true;
    }


    // OrderOpen_ToggleEvent Functions
    private void Find_AttractedNPC()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _currentNPC = null;

        // if (Main_Controller.orderOpen == false) return;

        _coroutine = StartCoroutine(Find_AttractedNPC_Coroutine());
    }
    private IEnumerator Find_AttractedNPC_Coroutine()
    {
        FoodOrder_PreviewUpdate();

        while (_currentNPC == null)
        {
            Set_AttractedNPC();
            yield return null;
        }

        FoodOrder_PreviewUpdate();

        _coroutine = null;
        yield break;
    }

    private void Set_AttractedNPC()
    {
        List<NPC_Controller> allNPCs = stationController.mainController.All_NPCs();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].foodIcon.hasFood == false) continue;
            // if (allNPCs[i].interaction.servedFoodData != null) continue;
            if (Other_ServeTable_OrderTaken(allNPCs[i]) == true) continue;

            // set current npc
            _currentNPC = allNPCs[i];

            return;
        }
    }

    private void FoodOrder_PreviewUpdate()
    {
        /*
        if (Main_Controller.orderOpen == false || _currentNPC == null)
        {
            _foodOrderPreview.Set_CurrentData(null);
            _foodOrderPreview.Hide_Icon();
            _foodOrderPreview.Hide_Condition();

            return;
        }
        */

        _foodOrderPreview.Set_CurrentData(_currentNPC.foodIcon.currentData);
        _foodOrderPreview.Show_Icon();
        _foodOrderPreview.Show_Condition();

        LeanTween.alpha(_foodOrderPreview.gameObject, 0.5f, 0f);

        //
        Check_FoodServe();
    }

    private void Check_FoodServe()
    {
        if (_checkCoroutine != null)
        {
            StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }

        _checkCoroutine = StartCoroutine(Check_FoodServe_Coroutine());
    }
    private IEnumerator Check_FoodServe_Coroutine()
    {
        // current npc assigned, waiting for food order, time not ended
        while (_currentNPC != null && _currentNPC.foodIcon.hasFood) yield return null; // && _currentNPC.interaction.servedFoodData == null

        Find_AttractedNPC();
    }


    // Interact Functions
    private void Serve_FoodOrder_toNPC()
    {
        //
        if (_coroutine != null) return;

        // check if order is currently open
        // if (Main_Controller.orderOpen == false) return;

        // check if food order is set
        if (_currentNPC == null) return;

        // condition check
        if (Serve_Available() == false) return;
    }


    private void CloseOrder()
    {
        FoodOrder_PreviewUpdate();

        if (_currentNPC == null) return;

        SpriteRenderer locationRoamArea = stationController.mainController.currentLocation.data.roamArea;

        // stop coroutine
        if (_checkCoroutine != null)
        {
            StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }

        // npc returns to current location roam area
        _currentNPC.movement.Free_Roam(locationRoamArea);

        // reset
        _currentNPC = null;
    }
}
