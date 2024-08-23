using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationShopNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
    [SerializeField] private SubLocation _currentSubLocation;

    [SerializeField] private Transform[] _boxStackPoints;
    [SerializeField] private StationStock[] _stationStocks;

    [SerializeField] [Range(0, 5)] private int _duplicateAmount;

    [Header("")]
    [SerializeField] private StationStock _mergeStationStock;
    [SerializeField] private Station_ScrObj[] _mergeStations;

    [Header("")]
    [SerializeField] private SpriteRenderer _carryBox;


    private List<Station_ScrObj> _unlockedStations = new(); 

    private Coroutine _restockCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // event subscriptions
        GlobalTime_Controller.TimeTik_Update += Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent += CarryBox_DirectionUpdate;

        // interaction subscription
        _interactable.InteractEvent += Cancel_Action;
        _interactable.InteractEvent += Interact_FacePlayer;

        _interactable.Action1Event += Merge_BookMarkedStations;
        _interactable.Action2Event += Unlock_BookMarkedStations;

        // start free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        CarryBox_SpriteToggle(false);
    }

    private void OnDestroy()
    {
        Save_Data();

        // event subscriptions
        GlobalTime_Controller.TimeTik_Update -= Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent -= CarryBox_DirectionUpdate;

        // interaction subscription
        _interactable.InteractEvent -= Cancel_Action;
        _interactable.InteractEvent -= Interact_FacePlayer;

        _interactable.Action1Event -= Merge_BookMarkedStations;
        _interactable.Action2Event -= Unlock_BookMarkedStations;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        List<int> stationIDs = new();

        foreach (var station in _unlockedStations)
        {
            stationIDs.Add(station.id);
        }

        ES3.Save("StationShopNPC/_unlockedStations", stationIDs);
    }

    public void Load_Data()
    {
        List<int> stationIDs = new();
        stationIDs = ES3.Load("StationShopNPC/_unlockedStations", stationIDs);

        Data_Controller data = _npcController.mainController.dataController;

        for (int i = 0; i < stationIDs.Count; i++)
        {
            _unlockedStations.Add(data.Station_ScrObj(stationIDs[i]));
        }
    }


    // Carry Box Sprite Control
    private void CarryBox_SpriteToggle(bool toggleOn)
    {
        if (toggleOn) _carryBox.color = Color.white;
        else _carryBox.color = Color.clear;
    }

    private void CarryBox_DirectionUpdate()
    {
        NPC_Movement move = _npcController.movement;

        // left
        if (move.Move_Direction() == -1) _carryBox.flipX = true;

        // right
        else _carryBox.flipX = false;
    }


    // Basic Actions
    private void Interact_FacePlayer()
    {
        // facing to player direction
        _npcController.basicAnim.Flip_Sprite(_npcController.interactable.detection.player.gameObject);

        NPC_Movement movement = _npcController.movement;

        movement.Stop_FreeRoam();
        movement.Free_Roam(_currentSubLocation.roamArea, Random.Range(movement.intervalTimeRange.x, movement.intervalTimeRange.y));
    }


    // Unlocked Station Control
    private Station_ScrObj Unlocked_Station()
    {
        if (_unlockedStations.Count <= 0) return null;

        int arrayNum = Random.Range(0, _unlockedStations.Count);

        return _unlockedStations[arrayNum];
    }

    private void Unlock_Station(Station_ScrObj unlockStation)
    {
        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;
        List<ItemSlot> lockedSlots = menu.controller.slotsController.LockedSlots();

        // remove all locked unlockStation
        for (int i = 0; i < lockedSlots.Count; i++)
        {
            if (unlockStation != lockedSlots[i].data.currentStation) continue;

            lockedSlots[i].Empty_ItemBox();
        }

        if (_unlockedStations.Contains(unlockStation)) return;

        _unlockedStations.Add(unlockStation);
    }


    // Station Stock Control
    private bool StationStocks_Full()
    {
        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold) return false;
        }

        return true;
    }


    private bool DuplicateAmount_Stocked(Station_ScrObj checkStation)
    {
        int checkCount = 0;

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == true) continue; 
            if (_stationStocks[i].currentStation != checkStation) continue;
            checkCount++;
        }

        if (_unlockedStations.Count <= _stationStocks.Length) return false;

        if (_duplicateAmount <= 0) _duplicateAmount = 1;
        if (checkCount >= _duplicateAmount) return true;

        return false;
    }

    private Station_ScrObj NonDuplicate_Station()
    {
        Station_ScrObj station = Unlocked_Station();

        do
        {
            station = Unlocked_Station();
        }
        while (DuplicateAmount_Stocked(station));

        return station;
    }


    // Main Actions
    private void Cancel_Action()
    {
        if (_restockCoroutine == null) return;

        StopCoroutine(_restockCoroutine);
        _restockCoroutine = null;

        CarryBox_SpriteToggle(false);

        // return to free roam
        NPC_Movement move = _npcController.movement;
        move.Free_Roam(_currentSubLocation.roamArea, move.Random_IntervalTime());
    }


    private void Restock_StationStocks()
    {
        if (_unlockedStations.Count <= 0) return;

        if (_restockCoroutine != null) return;

        if (StationStocks_Full()) return;

        _restockCoroutine = StartCoroutine(Restock_StationStocks_Coroutine());
    }
    private IEnumerator Restock_StationStocks_Coroutine()
    {
        NPC_Movement move = _npcController.movement;

        _interactable.UnInteract();

        //
        move.Stop_FreeRoam();

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;

            // Part 1 //

            // go to one of the box stacks
            Transform randBoxStack = _boxStackPoints[Random.Range(0, _boxStackPoints.Length)];
            move.Assign_TargetPosition(randBoxStack.position);

            // wait until destination reach
            while (move.At_TargetPosition() == false) yield return null;

            // carry box toggle on
            CarryBox_SpriteToggle(true);

            // Part 2 //

            // go to sold station stock
            move.Assign_TargetPosition(_stationStocks[i].transform.position);

            // wait until destination reach
            while (move.At_TargetPosition() == false) yield return null;

            // set station
            _stationStocks[i].Set_Data(NonDuplicate_Station());

            // restock station stock
            _stationStocks[i].Restock();

            // carry box toggle off
            CarryBox_SpriteToggle(false);
        }

        // return to free roam
        move.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        _restockCoroutine = null;
        yield break;
    }


    private void Merge_BookMarkedStations()
    {
        // check if currently on action
        if (_restockCoroutine != null) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // check if _mergeStationStock is empty
        if (_mergeStationStock.sold == false)
        {
            dialog.Update_Dialog(0);
            return;
        }

        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        List<ItemSlot> bookMarkedSlots = menu.controller.slotsController.BookMarked_Slots(false);

        // check if there are more than 2 bookmarked stations
        if (bookMarkedSlots.Count < 2)
        {
            dialog.Update_Dialog(1);
            return;
        }

        // empty bookmarked stations
        for (int i = 0; i < bookMarkedSlots.Count; i++)
        {
            bookMarkedSlots[i].Empty_ItemBox();
        }

        _restockCoroutine = StartCoroutine(Merge_BookMarkedStations_Coroutine());
    }
    private IEnumerator Merge_BookMarkedStations_Coroutine()
    {
        NPC_Movement move = _npcController.movement;

        //
        CarryBox_SpriteToggle(true);

        // move to _mergeStationStock
        move.Assign_TargetPosition(_mergeStationStock.transform.position);

        // wait until arrival
        while (move.At_TargetPosition() == false) yield return null;

        Station_ScrObj mergeStation = _mergeStations[Random.Range(0, _mergeStations.Length)];

        // restock _mergeStationStock
        _mergeStationStock.Restock(mergeStation);

        // set _mergeStationStock price to 0
        _mergeStationStock.Update_Price(0);

        //
        CarryBox_SpriteToggle(false);

        // return to free roam
        move.Free_Roam(_currentSubLocation.roamArea, 0f);

        // merge complete dialog
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(4);

        //
        _restockCoroutine = null;
        yield break;
    }


    private void Unlock_BookMarkedStations()
    {
        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;
        List<ItemSlot> bookmarkedSlots = menu.controller.slotsController.BookMarked_Slots(true);

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (bookmarkedSlots.Count <= 0)
        {
            dialog.Update_Dialog(2);
            return;
        }

        for (int i = 0; i < bookmarkedSlots.Count; i++)
        {
            Unlock_Station(bookmarkedSlots[i].data.currentStation);
        }

        dialog.Update_Dialog(3);
    }
}
