using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationShopNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
    [SerializeField] private SpriteRenderer _stationObject;

    [SerializeField] private SpriteRenderer _carryObject;
    [SerializeField] private Sprite[] _carrySprites;

    [Header("")]
    [SerializeField] private SubLocation _currentSubLocation;

    [Header("")]
    [SerializeField] private ScrapStack _scrapStack;
    [SerializeField][Range(0, 10)] private float _disposeWaitTime;
    [SerializeField][Range(0, 10)] private int _disposeScrapAmount;

    [Header("")]
    [SerializeField] private Transform[] _boxStackPoints;

    [Header("")]
    [SerializeField] private StationStock[] _stationStocks;
    [SerializeField][Range(0, 5)] private int _duplicateAmount;

    private List<Station_ScrObj> _unlockedStations = new();

    private Coroutine _actionCoroutine;


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
        // GlobalTime_Controller.TimeTik_Update += Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent += CarryObject_DirectionUpdate;

        // interaction subscription
        _interactable.InteractEvent += Cancel_Action;
        _interactable.InteractEvent += Interact_FacePlayer;

        _interactable.Action1Event += Dispose_BookMarkedStation;
        _interactable.Action2Event += Unlock_BookMarkedStations;

        // start free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        CarryObject_SpriteToggle(false, null);
    }

    private void OnDestroy()
    {
        Save_Data();

        // event subscriptions
        // GlobalTime_Controller.TimeTik_Update -= Restock_StationStocks;
        _npcController.movement.TargetPosition_UpdateEvent -= CarryObject_DirectionUpdate;

        // interaction subscription
        _interactable.InteractEvent -= Cancel_Action;
        _interactable.InteractEvent -= Interact_FacePlayer;

        _interactable.Action1Event -= Dispose_BookMarkedStation;
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


    // Basic Actions
    private void CarryObject_SpriteToggle(bool toggleOn, Sprite objectSprite)
    {
        if (toggleOn == false)
        {
            _carryObject.color = Color.clear;
            return;
        }

        _carryObject.sprite = objectSprite;
        _carryObject.color = Color.white;
    }

    private void CarryObject_DirectionUpdate()
    {
        NPC_Movement move = _npcController.movement;

        // left
        if (move.Move_Direction() == -1) _carryObject.flipX = true;

        // right
        else _carryObject.flipX = false;
    }


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
        if (_actionCoroutine == null) return;

        StopCoroutine(_actionCoroutine);
        _actionCoroutine = null;

        CarryObject_SpriteToggle(false, null);

        // return to free roam
        NPC_Movement move = _npcController.movement;
        move.Free_Roam(_currentSubLocation.roamArea, move.Random_IntervalTime());
    }


    private void Restock_StationStocks()
    {
        if (_unlockedStations.Count <= 0) return;

        if (_actionCoroutine != null) return;

        if (StationStocks_Full()) return;

        _actionCoroutine = StartCoroutine(Restock_StationStocks_Coroutine());
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
            CarryObject_SpriteToggle(true, _carrySprites[0]);

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
            CarryObject_SpriteToggle(false, null);
        }

        // return to free roam
        move.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        _actionCoroutine = null;
        yield break;
    }


    private void Dispose_BookMarkedStation()
    {
        _actionCoroutine = StartCoroutine(Dispose_BookMarkedStation_Coroutine());
    }
    private IEnumerator Dispose_BookMarkedStation_Coroutine()
    {
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedStations = slots.BookMarked_Datas(menu.currentDatas, false);

        // check if there are any bookmarked stations
        if (bookmarkedStations.Count <= 0)
        {
            // dialog

            _actionCoroutine = null;
            yield break;
        }

        _npcController.interactable.LockInteract(true);

        // get latest bookmark unlocked station
        ItemSlot_Data targetData = bookmarkedStations[bookmarkedStations.Count - 1];
        Station_ScrObj targetStation = targetData.currentStation;

        // remove station
        targetData.Empty_Item();

        // set _stationObject sprite transparency animation
        _stationObject.sprite = targetStation.miniSprite;
        _stationObject.color = Color.white;

        yield return new WaitForSeconds(_disposeWaitTime);

        CarryObject_SpriteToggle(true, _carrySprites[1]);
        _stationObject.color = Color.clear;

        // move to scrap stack
        NPC_Movement movement = _npcController.movement;
        movement.Assign_TargetPosition(_scrapStack.transform.position);

        while (movement.At_TargetPosition() == false) yield return null;

        // increase scrap stack amount
        _scrapStack.amountBar.Update_Amount(_disposeScrapAmount);
        _scrapStack.amountBar.Load();
        _scrapStack.Update_CurrentSprite();

        _npcController.interactable.LockInteract(false);
        CarryObject_SpriteToggle(false, null);

        // return to free roam
        movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        _actionCoroutine = null;
        yield break;
    }


    private void Unlock_BookMarkedStations()
    {
        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slotsController = menu.controller.slotsController;

        List<ItemSlot_Data> bookMarkedDatas = slotsController.BookMarked_Datas(menu.currentDatas, true);

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (bookMarkedDatas.Count <= 0)
        {
            dialog.Update_Dialog(2);
            return;
        }

        for (int i = 0; i < bookMarkedDatas.Count; i++)
        {
            if (bookMarkedDatas[i].isLocked == false) continue;

            // empty data
            Station_ScrObj unlockStation = bookMarkedDatas[i].currentStation;
            slotsController.SlotData(menu.currentDatas, bookMarkedDatas[i]).Empty_Item();

            // add station to _unlockedStations
            if (_unlockedStations.Contains(unlockStation)) continue;
            _unlockedStations.Add(unlockStation);
        }

        dialog.Update_Dialog(3);
    }
}
