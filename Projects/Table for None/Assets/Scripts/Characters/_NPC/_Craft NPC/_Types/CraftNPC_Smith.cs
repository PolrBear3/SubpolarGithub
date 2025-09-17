using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Station_ScrObj[] _defaultStations;
    
    
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
        ActionBubble_Interactable interactable = npcController.interactable;

        GlobalTime_Controller.instance.OnTimeTik += MoveTo_SetPosition;
        interactable.OnHoldInteract += MoveTo_SetPosition;

        interactable.OnInteract += Toggle_PurchasePrice;
        interactable.OnInteract += Update_ActionBubble;

        interactable.OnAction1 += Purchase;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        Collect_Table();
        
        // subscriptions
        ActionBubble_Interactable interactable = npcController.interactable;
        
        GlobalTime_Controller.instance.OnTimeTik -= MoveTo_SetPosition;
        interactable.OnHoldInteract -= MoveTo_SetPosition;

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

        int actionIndex = _setTable.currentIndex;
        ActionBubble_Data data = new(_setTable.indicatorIcon.sprite, bubble.bubbleDatas[actionIndex].LocalizedInfo());
        
        bubble.Set_IndicatorToggleDatas(new() { data });
        bubble.Set_Bubble(_setTable.indicatorIcon.sprite, null);
    }

    private void Toggle_PurchasePrice()
    {
        GoldSystem system = GoldSystem.instance;

        if (_setTable != null || purchaseData.purchased) return;
        system.Indicate_TriggerData(new(npcIconSprite, -defaultPrice));
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
        List<Vector2> setPositions = currentLocation.All_SpawnPositions(transform.position);
        
        return setPositions[0];
    }


    private void MoveTo_SetPosition()
    {
        if (Table_SetAvailable() == false) return;

        Set_Coroutine(StartCoroutine(MoveTo_SetPosition_Coroutine()));
    }
    private IEnumerator MoveTo_SetPosition_Coroutine()
    {
        Toggle_Action(true);

        Main_Controller main = Main_Controller.instance;
        NPC_Movement movement = npcController.movement;
        
        Vector2 setPosition = Table_SetPosition();
        movement.Assign_TargetPosition(setPosition);

        while (movement.At_TargetPosition(setPosition) == false)
        {
            if (main.data.Position_Claimed(setPosition))
            {
                setPosition = Table_SetPosition();
                movement.Assign_TargetPosition(setPosition);
            }
            yield return null;
        }

        Set_Table();

        Toggle_Action(false);
        yield break;
    }

    private void Set_Table()
    {
        if (Table_SetAvailable() == false) return;

        Main_Controller main = Main_Controller.instance;
        Vector2 dropPos = Utility.SnapPosition(transform.position);

        if (main.data.Position_Claimed(dropPos)) return;

        GameObject drop = Instantiate(_smithTable, dropPos, Quaternion.identity);
        drop.transform.SetParent(main.otherFile);

        _setTable = drop.GetComponent<ActionSelector>();
        _setTable.sr.sprite = _smithTableSprite;

        foreach (ActionSelector_Data data in _actionDatas)
        {
            _setTable.Add_ActionData(data);
        }

        Update_ActionBubble();
        
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
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
        _setTable.interactable.Toggle_Lock(true);
        
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

    private Station_ScrObj Random_LinkedStation(Station_ScrObj exchangeStation)
    {
        int randIndex = UnityEngine.Random.Range(0, exchangeStation.linkedStationDatas.Length);
        
        if (exchangeStation.linkedStationDatas.Length > 0)
        {
            return exchangeStation.linkedStationDatas[randIndex].stationScrObj;
        }
        
        if (_defaultStations.Length == 0)
        {
            Debug.Log(gameObject.name + " _defaultStation list empty !");
            return null;
        }
        
        randIndex = UnityEngine.Random.Range(0, _defaultStations.Length);
        return _defaultStations[randIndex];
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

        int randIndex = UnityEngine.Random.Range(0, surroundingStations.Count);

        Station_Controller exchangeStation = surroundingStations[randIndex];
        StationData stationData = new(exchangeStation.data);

        Station_ScrObj updateStation = Random_LinkedStation(exchangeStation.stationScrObj);
        if (updateStation == null) return;
        
        // destroy exchange station
        exchangeStation.Destroy_Station();
        Main_Controller.instance.data.claimedPositions.Remove(stationData.position);

        // drop
        GameObject spawnCollectCard = _itemDropper.SnapPosition_Spawn(_itemDropper.collectCard, stationData.position);
        CollectCard collectCard = spawnCollectCard.GetComponent<CollectCard>();
        
        collectCard.Set_Blueprint(updateStation);
        collectCard.Assign_Pickup(collectCard.StationBluePrint_toArchive);

        Set_PurchaseData(new(defaultPrice));
        Toggle_PayIcon();
        
        Audio_Controller.instance.Play_OneShot(gameObject, 2);
    }
}