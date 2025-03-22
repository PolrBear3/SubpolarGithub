using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class CraftNPC_Smith : CraftNPC
{
    [Header("")]
    [SerializeField] private GameObject _smithTable;
    [SerializeField] private Sprite _smithTableSprite;

    [Header("")]
    [SerializeField] private ActionSelector_Data[] _actionDatas;

    private ActionSelector _setTable;


    [Header("")]
    [SerializeField] private Station_ScrObj[] _modifyStations;


    // MonoBehaviour
    private new void Awake()
    {
        base.Awake();
        Load_Data();
    }

    private new void Start()
    {
        base.Start();
        Subscribe_OnSave(Save_Data);

        // subscriptions
        GlobalTime_Controller.instance.OnTimeTik += MoveTo_SetPosition;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnIInteract += Toggle_PurchasePrice;
        interactable.OnAction1Input += Purchase;
    }

    private new void OnDestroy()
    {
        // subscriptions
        GlobalTime_Controller.instance.OnTimeTik -= MoveTo_SetPosition;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnIInteract -= Toggle_PurchasePrice;
        interactable.OnAction1Input -= Purchase;
    }


    // private Save and Load
    private void Save_Data()
    {
        ES3.Save("CraftNPC_Smith/data", data);
    }

    private void Load_Data()
    {
        Set_Data(ES3.Load("CraftNPC_Smith/data", new CraftNPC_Data(false)));
    }


    // Indications
    private void Update_ActionBubble()
    {
        ActionBubble_Interactable interactable = npcController.interactable;
        Action_Bubble bubble = interactable.bubble;

        if (_setTable == null || _setTable.currentDatas.Count <= 0)
        {
            bubble.Empty_Bubble();
            return;
        }

        bubble.Set_Bubble(_setTable.indicatorIcon.sprite, null);

        // dialog.Update_Dialog(_setTable.currentIndex); //
    }

    private void Toggle_PurchasePrice()
    {
        GoldSystem system = GoldSystem.instance;

        if (_setTable == null || data.payed == false)
        {
            system.Indicate_TriggerData(new(npcIconSprite, -defaultPrice));
            return;
        }

        // additional price indication //
    }


    // Set Table
    private bool Table_SetAvailable()
    {
        if (coroutine != null) return false;

        if (data.payed == false) return false;
        if (_setTable != null) return false;

        if (Table_SetPosition() == Vector2.zero) return false;

        return true;
    }

    private Vector2 Table_SetPosition()
    {
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

        return currentLocation.Random_SpawnPosition();
    }


    private void MoveTo_SetPosition()
    {
        if (Table_SetAvailable() == false) return;

        Set_Coroutine(StartCoroutine(MoveTo_SetPosition_Coroutine()));
    }
    private IEnumerator MoveTo_SetPosition_Coroutine()
    {
        Toggle_Coroutine(true);

        NPC_Movement movement = npcController.movement;
        Vector2 movePosition = Table_SetPosition();

        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(movePosition);

        while (movement.At_TargetPosition(movePosition) == false) yield return null;

        Set_Table();

        Toggle_Coroutine(false);
        yield break;
    }

    private void Set_Table()
    {
        if (Table_SetAvailable() == false) return;

        Main_Controller main = Main_Controller.instance;
        Vector2 dropPos = main.SnapPosition(transform.position);

        if (main.Position_Claimed(dropPos)) return;

        GameObject drop = Instantiate(_smithTable, dropPos, quaternion.identity);
        drop.transform.SetParent(main.otherFile);

        _setTable = drop.GetComponent<ActionSelector>();
        _setTable.interactable.OnInteract += Update_ActionBubble;

        _setTable.sr.sprite = _smithTableSprite;

        foreach (ActionSelector_Data data in _actionDatas)
        {
            _setTable.Add_ActionData(data);
        }
    }


    private void Purchase()
    {
        if (coroutine != null) return;
        if (_setTable == null) return;

        Set_Coroutine(StartCoroutine(Purchase_Coroutine()));
    }
    private IEnumerator Purchase_Coroutine()
    {
        Toggle_Coroutine(true);

        NPC_Movement movement = npcController.movement;

        // move to action selector
        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(_setTable.transform.position);

        while (movement.At_TargetPosition() == false) yield return null;
        yield return new WaitForSeconds(upgradeTimeValue);

        // activate action
        _setTable.Invoke_Action();

        // collect action selector
        GameObject currentToolBox = _setTable.gameObject;
        _setTable.interactable.OnInteract += Update_ActionBubble;

        _setTable = null;
        Destroy(currentToolBox);

        // reset data
        Set_Data(new(defaultPrice));

        Update_ActionBubble();

        Toggle_Coroutine(false);
        yield break;
    }


    // Purchase Upgrades
    private List<Station_Controller> Surrounding_Stations()
    {
        if (_setTable == null) return null;

        List<Vector2> tableSurroundings = _setTable.positionClaimer.All_SurroundPositions();
        List<Station_Controller> allStations = new(Main_Controller.instance.CurrentStations(false));

        for (int i = allStations.Count - 1; i >= 0; i--)
        {
            if (tableSurroundings.Contains(allStations[i].transform.position)) continue;

            allStations.RemoveAt(i);
        }

        return allStations;
    }


    public void Increase_StationDurability()
    {
        for (int i = 0; i < Surrounding_Stations().Count; i++)
        {
            StationData data = Surrounding_Stations()[i].data;
            Station_ScrObj station = Surrounding_Stations()[i].stationScrObj;

            int updateAmount = station.durability - data.durability;
            if (updateAmount <= 0) continue;

            data.Update_Amount(updateAmount);
            Surrounding_Stations()[i].maintenance.Update_DurabilityBreak();
        }
    }
}