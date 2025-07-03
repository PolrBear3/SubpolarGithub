using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class CraftNPC_Smith : CraftNPC
{
    [Space(60)]
    [SerializeField] private GameObject _smithTable;
    [SerializeField] private Sprite _smithTableSprite;

    [Space(20)]
    [SerializeField] private ActionSelector_Data[] _actionDatas;

    [Space(20)]
    [SerializeField] private ItemDropper _itemDropper;
    [SerializeField] private Station_ScrObj[] _modifyStations;
    
    
    private ActionSelector _setTable;
    

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
        globaltime.instance.OnTimeTik += MoveTo_SetPosition;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnInteract += Toggle_PurchasePrice;
        interactable.OnInteract += Update_ActionBubble;

        interactable.OnAction1 += Purchase;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        Collect_Table();
        
        // subscriptions
        globaltime.instance.OnTimeTik -= MoveTo_SetPosition;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnInteract -= Toggle_PurchasePrice;
        interactable.OnInteract -= Update_ActionBubble;

        interactable.OnAction1 -= Purchase;
    }


    // private Save and Load
    private void Save_Data()
    {
        // ES3.Save("CraftNPC_Smith/CraftNPC_Data", data);
        ES3.Save("CraftNPC_Smith/PurchaseData", purchaseData);
    }

    private void Load_Data()
    {
        // Set_Data(ES3.Load("CraftNPC_Smith/CraftNPC_Data"));
        Set_PurchaseData(ES3.Load("CraftNPC_Smith/PurchaseData", new PurchaseData(defaultPrice)));
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

        if (bubble.bubbleOn == false) return;

        dialog.Update_Dialog(_setTable.currentIndex);
    }

    private void Toggle_PurchasePrice()
    {
        GoldSystem system = GoldSystem.instance;

        if (_setTable == null || purchaseData.purchased == false)
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

        if (purchaseData.purchased == false) return false;
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
        Toggle_Action(true);

        NPC_Movement movement = npcController.movement;
        
        Vector2 movePosition = Table_SetPosition();
        movement.Assign_TargetPosition(movePosition);

        while (movement.At_TargetPosition(movePosition) == false) yield return null;

        Set_Table();

        Toggle_Action(false);
        yield break;
    }

    private void Set_Table()
    {
        if (Table_SetAvailable() == false) return;

        Main_Controller main = Main_Controller.instance;
        Vector2 dropPos = Utility.SnapPosition(transform.position);

        if (main.Position_Claimed(dropPos)) return;

        GameObject drop = Instantiate(_smithTable, dropPos, quaternion.identity);
        drop.transform.SetParent(main.otherFile);

        _setTable = drop.GetComponent<ActionSelector>();
        _setTable.sr.sprite = _smithTableSprite;

        foreach (ActionSelector_Data data in _actionDatas)
        {
            _setTable.Add_ActionData(data);
        }

        Update_ActionBubble();
    }

    private void Collect_Table()
    {
        if (_setTable == null) return;
        
        GameObject setTable = _setTable.gameObject;
        
        _setTable = null;
        Destroy(setTable);
    }


    private void Purchase()
    {
        if (coroutine != null) return;
        if (_setTable == null) return;

        Set_Coroutine(StartCoroutine(Purchase_Coroutine()));
    }
    private IEnumerator Purchase_Coroutine()
    {
        Toggle_Action(true);
        
        // move to action selector
        NPC_Movement movement = npcController.movement;
        movement.Assign_TargetPosition(_setTable.transform.position);

        while (movement.At_TargetPosition() == false) yield return null;
        yield return new WaitForSeconds(upgradeTimeValue);

        // activate action
        _setTable.Invoke_Action();

        Collect_Table();
        Update_ActionBubble();

        Toggle_Action(false);
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
        List<Station_Controller> surroundingStations = Surrounding_Stations();

        if (surroundingStations.Count <= 0)
        {
            purchaseData.Toggle_PurchaseState(true);
            Toggle_PayIcon();

            return;
        }

        bool increased = false;

        for (int i = 0; i < surroundingStations.Count; i++)
        {
            StationData data = Surrounding_Stations()[i].data;
            Station_ScrObj station = Surrounding_Stations()[i].stationScrObj;

            if (data.durability >= station.durability) continue;

            data.Set_Durability(station.durability);
            Surrounding_Stations()[i].maintenance.Update_DurabilityBreak();

            increased = true;
        }

        if (increased == false)
        {
            purchaseData.Toggle_PurchaseState(true);
            Toggle_PayIcon();

            return;
        }

        Set_PurchaseData(new(defaultPrice));
        Toggle_PayIcon();
    }

    public void Drop_BluePrint()
    {
        List<Station_Controller> surroundingStations = Surrounding_Stations();

        for (int i = surroundingStations.Count - 1; i >= 0; i--)
        {
            if (surroundingStations[i].maintenance != null) continue;
            surroundingStations.RemoveAt(i);
        }

        if (surroundingStations.Count <= 0)
        {
            purchaseData.Toggle_PurchaseState(true);
            Toggle_PayIcon();

            return;
        }

        // destroy random modify station
        int randIndex = UnityEngine.Random.Range(0, surroundingStations.Count);

        Station_Controller modifyStation = surroundingStations[randIndex];
        StationData stationData = new(modifyStation.data);

        modifyStation.Destroy_Station();
        Main_Controller.instance.UnClaim_Position(stationData.position);

        // drop
        int newRandIndex = UnityEngine.Random.Range(0, _modifyStations.Length);
        Station_ScrObj newStationScrObj = _modifyStations[newRandIndex];

        GameObject spawnCollectCard = _itemDropper.SnapPosition_Spawn(_itemDropper.collectCard, stationData.position);
        CollectCard collectCard = spawnCollectCard.GetComponent<CollectCard>();
        
        collectCard.Set_Blueprint(newStationScrObj);
        collectCard.Assign_Pickup(collectCard.StationBluePrint_toArchive);

        Set_PurchaseData(new(defaultPrice));
        Toggle_PayIcon();
    }
}