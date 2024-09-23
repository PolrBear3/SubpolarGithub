using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationShopNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
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

    private List<Station_ScrObj> _stationArchive = new();

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
        _interactable.Action2Event += Build_BookMarkedStations;

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
        _interactable.Action2Event -= Build_BookMarkedStations;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        List<int> stationIDs = new();

        foreach (var station in _stationArchive)
        {
            stationIDs.Add(station.id);
        }

        ES3.Save("StationShopNPC/_stationArchive", stationIDs);
    }

    public void Load_Data()
    {
        List<int> stationIDs = new();
        stationIDs = ES3.Load("StationShopNPC/_stationArchive", stationIDs);

        Data_Controller data = _npcController.mainController.dataController;

        for (int i = 0; i < stationIDs.Count; i++)
        {
            _stationArchive.Add(data.Station_ScrObj(stationIDs[i]));
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
        if (_stationArchive.Count <= 0) return null;

        int arrayNum = Random.Range(0, _stationArchive.Count);

        return _stationArchive[arrayNum];
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

        if (_stationArchive.Count <= _stationStocks.Length) return false;

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

        _npcController.interactable.LockInteract(false);

        CarryObject_SpriteToggle(false, null);

        // return to free roam
        NPC_Movement move = _npcController.movement;

        move.Stop_FreeRoam();
        move.Free_Roam(_currentSubLocation.roamArea, 0f);
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

            Cancel_Action();
            yield break;
        }

        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();

        _npcController.interactable.LockInteract(true);

        // get latest bookmark unlocked station
        ItemSlot_Data targetData = bookmarkedStations[bookmarkedStations.Count - 1];
        Station_ScrObj targetStation = targetData.currentStation;

        // remove station
        targetData.Empty_Item();

        // player give disopose station animation
        CoinLauncher playerLauncher = _interactable.detection.player.coinLauncher;
        playerLauncher.Parabola_CoinLaunch(targetStation.miniSprite, transform.position);

        yield return new WaitForSeconds(_disposeWaitTime);

        CarryObject_SpriteToggle(true, _carrySprites[1]);

        // move to scrap stack
        movement.Assign_TargetPosition(_scrapStack.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        // increase scrap stack amount
        _scrapStack.amountBar.Update_Amount(_disposeScrapAmount);
        _scrapStack.amountBar.Load();
        _scrapStack.Update_CurrentSprite();

        Cancel_Action();
    }


    private void Build_BookMarkedStations()
    {
        if (StationStocks_Full())
        {
            // dialog
            return;
        }

        _actionCoroutine = StartCoroutine(Build_BookMarkedStations_Coroutine());
    }
    private IEnumerator Build_BookMarkedStations_Coroutine()
    {
        // get all locked bookmark stations
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedStations = slots.BookMarked_Datas(menu.currentDatas, true);

        if (bookmarkedStations.Count <= 0)
        {
            // dialog

            Cancel_Action();
            yield break;
        }

        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();

        _npcController.interactable.LockInteract(true);

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;

            // check if there is scrap in stack
            if (_scrapStack.amountBar.currentAmount <= 0)
            {
                // dialog
                break;
            }

            // go to scrap
            movement.Assign_TargetPosition(_scrapStack.transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // collect scrap
            _scrapStack.amountBar.Update_Amount(-1);
            _scrapStack.amountBar.Load();
            _scrapStack.Update_CurrentSprite();

            //
            CarryObject_SpriteToggle(true, _carrySprites[1]);

            // move to random box stack
            movement.Assign_TargetPosition(_boxStackPoints[Random.Range(0, _boxStackPoints.Length)].position);
            while (movement.At_TargetPosition() == false) yield return null;

            //
            CarryObject_SpriteToggle(true, _carrySprites[0]);

            // move to _stationStocks[i]
            movement.Assign_TargetPosition(_stationStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // add to archive
            _stationArchive.Add(bookmarkedStations[i].currentStation);

            // restock locked bookmark station
            _stationStocks[i].Toggle_Discount(false);
            _stationStocks[i].Restock(bookmarkedStations[i].currentStation);

            bookmarkedStations[i].Empty_Item();

            CarryObject_SpriteToggle(false, null);

            if (i >= bookmarkedStations.Count - 1) break;
        }

        Cancel_Action();
    }
}
