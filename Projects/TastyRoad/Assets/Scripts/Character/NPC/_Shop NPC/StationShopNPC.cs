using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationShopNPC : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _npcController;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
    [SerializeField] private GameObject _restockBarObject;
    [SerializeField] private AmountBar _restockBar;

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

    [SerializeField] private StationStock[] _stationStocks;

    [Header("")]
    [SerializeField][Range(0, 5)] private int _duplicateAmount;

    [SerializeField][Range(0, 100)] private int _restockCount;
    private int _currentRestockCount;


    private List<Station_ScrObj> _archivedStations = new();

    private Coroutine _actionCoroutine;


    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        _restockBar.Toggle_BarColor(true);
        Toggle_RestockBar();

        // untrack
        _npcController.mainController.UnTrack_CurrentCharacter(gameObject);

        // event subscriptions
        _npcController.movement.TargetPosition_UpdateEvent += CarryObject_DirectionUpdate;

        // subscription
        WorldMap_Controller.NewLocation_Event += Restock_New;
        GlobalTime_Controller.TimeTik_Update += Restock_ArchivedStation;

        _interactable.InteractEvent += Cancel_Action;
        _interactable.InteractEvent += Interact_FacePlayer;

        _interactable.detection.EnterEvent += Toggle_RestockBar;
        _interactable.InteractEvent += Toggle_RestockBar;
        _interactable.UnInteractEvent += Toggle_RestockBar;

        _interactable.OnAction1Event += Dispose_BookMarkedStation;
        _interactable.OnAction2Event += Build_BookMarkedStations;

        // start free roam
        _npcController.movement.Free_Roam(_currentSubLocation.roamArea, 0f);

        //
        CarryObject_SpriteToggle(false, null);
    }

    private void OnDestroy()
    {
        // event subscriptions
        _npcController.movement.TargetPosition_UpdateEvent -= CarryObject_DirectionUpdate;

        // interaction subscription
        WorldMap_Controller.NewLocation_Event -= Restock_New;
        GlobalTime_Controller.TimeTik_Update -= Restock_ArchivedStation;

        _interactable.InteractEvent -= Cancel_Action;
        _interactable.InteractEvent -= Interact_FacePlayer;

        _interactable.detection.EnterEvent -= Toggle_RestockBar;
        _interactable.InteractEvent -= Toggle_RestockBar;
        _interactable.UnInteractEvent -= Toggle_RestockBar;

        _interactable.OnAction1Event -= Dispose_BookMarkedStation;
        _interactable.OnAction2Event -= Build_BookMarkedStations;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        List<int> stationIDs = new();

        foreach (var station in _archivedStations)
        {
            stationIDs.Add(station.id);
        }
        ES3.Save("StationShopNPC/_archivedStations", stationIDs);

        Save_StationStocks();
        ES3.Save("StationShopNPC/_currentRestockCount", _currentRestockCount);
    }

    public void Load_Data()
    {
        List<int> stationIDs = new();
        stationIDs = ES3.Load("StationShopNPC/_archivedStations", stationIDs);

        Data_Controller data = _npcController.mainController.dataController;

        for (int i = 0; i < stationIDs.Count; i++)
        {
            _archivedStations.Add(data.Station_ScrObj(stationIDs[i]));
        }

        Load_StationStocks();
        _currentRestockCount = ES3.Load("StationShopNPC/_currentRestockCount", _currentRestockCount);
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


    private void Toggle_RestockBar()
    {
        ActionBubble_Interactable interactable = _npcController.interactable;

        if (interactable.detection.player == null)
        {
            _restockBarObject.SetActive(false);
            return;
        }

        Action_Bubble bubble = interactable.bubble;

        _restockBarObject.SetActive(!bubble.bubbleOn);

        if (bubble.bubbleOn) return;
        _restockBar.Load_Custom(_restockCount, _currentRestockCount);
    }


    // Station Stock Control
    private void Save_StationStocks()
    {
        Dictionary<StationData, StockData> stationStockDatas = new();

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            stationStockDatas.Add(new(_stationStocks[i].currentStation), new(_stationStocks[i].stockData));
        }

        ES3.Save("StationShopNPC/stationStockDatas", stationStockDatas);
    }

    private void Load_StationStocks()
    {
        Dictionary<StationData, StockData> stationStockDatas = new();
        stationStockDatas = ES3.Load("StationShopNPC/stationStockDatas", stationStockDatas);

        List<StationData> stations = new(stationStockDatas.Keys);
        List<StockData> stockDatas = new(stationStockDatas.Values);

        for (int i = 0; i < stationStockDatas.Count; i++)
        {
            // check if not enough station stocks are available to load
            if (i > _stationStocks.Length - 1) return;

            _stationStocks[i].Set_StationData(stations[i]);
            _stationStocks[i].Set_StockData(stockDatas[i]);
        }
    }


    private bool StationStocks_Full()
    {
        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold) return false;
        }

        return true;
    }

    private int DiscountStock_Amount()
    {
        int count = 0;

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].stockData.isDiscount == false) continue;
            count++;
        }

        return count;
    }


    private Station_ScrObj Archived_Station()
    {
        if (_archivedStations.Count <= 0) return null;

        int arrayNum = Random.Range(0, _archivedStations.Count);

        return _archivedStations[arrayNum];
    }

    private void Archive_Station(Station_ScrObj station)
    {
        if (_archivedStations.Contains(station)) return;
        _archivedStations.Add(station);
    }


    private bool DuplicateAmount_Stocked(Station_ScrObj checkStation)
    {
        int checkCount = 0;

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == true) continue;
            if (_stationStocks[i].currentStation.stationScrObj != checkStation) continue;
            checkCount++;
        }

        if (_archivedStations.Count <= _stationStocks.Length) return false;

        if (_duplicateAmount <= 0) _duplicateAmount = 1;
        if (checkCount >= _duplicateAmount) return true;

        return false;
    }

    private Station_ScrObj NonDuplicate_Station()
    {
        Station_ScrObj station = Archived_Station();

        do
        {
            station = Archived_Station();
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
        if (_actionCoroutine != null) return;

        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedStations = slots.BookMarked_Datas(menu.currentDatas, false);

        if (bookmarkedStations.Count <= 0)
        {
            DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
            dialog.Update_Dialog(0);

            return;
        }

        _actionCoroutine = StartCoroutine(Dispose_BookMarkedStation_Coroutine());
    }
    private IEnumerator Dispose_BookMarkedStation_Coroutine()
    {
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // dialog
        dialog.Update_Dialog(1);

        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();

        _npcController.interactable.LockInteract(true);

        // get latest bookmark unlocked station
        List<ItemSlot_Data> bookmarkedStations = slots.BookMarked_Datas(menu.currentDatas, false);

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
        if (_actionCoroutine != null) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (StationStocks_Full())
        {
            // dialog
            dialog.Update_Dialog(2);
            return;
        }

        // get all locked bookmark stations
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedData = slots.BookMarked_Datas(menu.currentDatas, true);

        // check if there are bookmarked stations
        if (bookmarkedData.Count <= 0)
        {
            // dialog
            dialog.Update_Dialog(2);
            return;
        }

        _actionCoroutine = StartCoroutine(Build_BookMarkedStations_Coroutine());
    }
    private IEnumerator Build_BookMarkedStations_Coroutine()
    {
        // get all locked bookmark stations
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedData = slots.BookMarked_Datas(menu.currentDatas, true);

        // dialog
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        dialog.Update_Dialog(3);

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
                dialog.Update_Dialog(4);

                Cancel_Action();
                yield break;
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

            // get recent bookmarked station
            Station_ScrObj recentStation = bookmarkedData[bookmarkedData.Count - 1].currentStation;

            // add to archive
            Archive_Station(recentStation);

            // restock locked bookmark station
            _stationStocks[i].Toggle_Discount(false);
            _stationStocks[i].Restock(recentStation);

            bookmarkedData[bookmarkedData.Count - 1].Empty_Item();
            bookmarkedData.RemoveAt(bookmarkedData.Count - 1);

            CarryObject_SpriteToggle(false, null);

            if (bookmarkedData.Count <= 0)
            {
                Cancel_Action();
                break;
            }
        }

        Cancel_Action();
        yield break;
    }


    private void Restock_New()
    {
        List<Station_ScrObj> archivedStations = new(_archivedStations);

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (archivedStations.Count <= 0) break;

            Station_ScrObj randStation = archivedStations[Random.Range(0, archivedStations.Count)];

            _stationStocks[i].Restock(randStation);
            _stationStocks[i].Toggle_Discount(false);

            archivedStations.Remove(randStation);
        }
    }

    private void Restock_ArchivedStation()
    {
        if (_currentRestockCount < _restockCount)
        {
            _currentRestockCount++;
            Toggle_RestockBar();

            return;
        }

        if (_actionCoroutine != null) return;
        if (_archivedStations.Count <= 0) return;
        if (StationStocks_Full()) return;

        _currentRestockCount = 0;
        Toggle_RestockBar();

        _actionCoroutine = StartCoroutine(Restock_ArchivedStation_Coroutine());
    }
    private IEnumerator Restock_ArchivedStation_Coroutine()
    {
        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;

            // start action
            _npcController.interactable.LockInteract(true);

            NPC_Movement movement = _npcController.movement;
            movement.Stop_FreeRoam();

            // move to random box stack
            movement.Assign_TargetPosition(_boxStackPoints[Random.Range(0, _boxStackPoints.Length)].position);
            while (movement.At_TargetPosition() == false) yield return null;

            CarryObject_SpriteToggle(true, _carrySprites[0]);

            // move to _stationStocks[i]
            movement.Assign_TargetPosition(_stationStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // restock station
            _stationStocks[i].Restock(NonDuplicate_Station());

            if (DiscountStock_Amount() <= 1)
            {
                Cancel_Action();
                break;
            }

            // toggle discount false if more than 1 discount stocks are found
            _stationStocks[i].Toggle_Discount(false);

            Cancel_Action();
            yield break;
        }

        Cancel_Action();
        yield break;
    }
}
