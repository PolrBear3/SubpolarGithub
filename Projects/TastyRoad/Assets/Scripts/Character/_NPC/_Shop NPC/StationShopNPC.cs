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

    [SerializeField] private Station_ScrObj[] _restrictedDisposables;

    [Header("")]
    [SerializeField] private Transform[] _boxStackPoints;

    [SerializeField] private StationStock[] _stationStocks;

    [Header("")]
    [SerializeField][Range(0, 5)] private int _duplicateAmount;

    [SerializeField][Range(0, 100)] private int _restockCount;
    private int _currentRestockCount;


    private List<StationData> _archiveDatas = new();

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

        _interactable.OnIInteract += Cancel_Action;
        _interactable.OnIInteract += Interact_FacePlayer;

        _interactable.detection.EnterEvent += Toggle_RestockBar;
        _interactable.detection.ExitEvent += Toggle_RestockBar;

        _interactable.OnIInteract += Toggle_RestockBar;
        _interactable.OnUnIInteract += Toggle_RestockBar;

        _interactable.OnAction1Input += Dispose_BookMarkedStation;
        _interactable.OnAction2Input += Build_BookMarkedStations;

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

        _interactable.OnIInteract -= Cancel_Action;
        _interactable.OnIInteract -= Interact_FacePlayer;

        _interactable.detection.EnterEvent -= Toggle_RestockBar;
        _interactable.detection.ExitEvent -= Toggle_RestockBar;

        _interactable.OnIInteract -= Toggle_RestockBar;
        _interactable.OnUnIInteract -= Toggle_RestockBar;

        _interactable.OnAction1Input -= Dispose_BookMarkedStation;
        _interactable.OnAction2Input -= Build_BookMarkedStations;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("StationShopNPC/_archiveDatas", _archiveDatas);
        ES3.Save("StationShopNPC/_currentRestockCount", _currentRestockCount);

        Save_StationStocks();
    }

    public void Load_Data()
    {
        _archiveDatas = ES3.Load("StationShopNPC/_archiveDatas", _archiveDatas);
        _currentRestockCount = ES3.Load("StationShopNPC/_currentRestockCount", _currentRestockCount);

        Load_StationStocks();
    }


    // All
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

        _restockBar.Toggle_BarColor(_currentRestockCount >= _restockCount);
        _restockBar.Load_Custom(_restockCount, _currentRestockCount);
        _restockBar.Toggle(true);
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


    // Archived Station Data
    private bool DuplicateAmount_Stocked(Station_ScrObj checkStation)
    {
        int checkCount = 0;

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == true) continue;
            if (_stationStocks[i].currentStation.stationScrObj != checkStation) continue;
            checkCount++;
        }

        if (_duplicateAmount <= 0) _duplicateAmount = 1;

        if (checkCount < _duplicateAmount) return false;
        return true;
    }


    private void Archive_Station(Station_ScrObj station)
    {
        for (int i = 0; i < _archiveDatas.Count; i++)
        {
            // check if archived
            if (station != _archiveDatas[i].stationScrObj) continue;

            _archiveDatas[i].Update_Amount(1);
            _archiveDatas[i].Set_Amount(Mathf.Clamp(_archiveDatas[i].amount, 1, station.buildToArchiveCount));

            return;
        }

        // archive new
        _archiveDatas.Add(new(station, 1));
    }

    private StationData Archived_Data(Station_ScrObj station)
    {
        for (int i = 0; i < _archiveDatas.Count; i++)
        {
            if (station != _archiveDatas[i].stationScrObj) continue;
            return _archiveDatas[i];
        }
        return null;
    }


    private Station_ScrObj NonDuplicate_Station()
    {
        if (_archiveDatas.Count <= 0) return null;

        List<StationData> archivedStations = new(_archiveDatas);

        while (archivedStations.Count > 0)
        {
            int randIndex = Random.Range(0, archivedStations.Count);
            Station_ScrObj randStation = archivedStations[randIndex].stationScrObj;

            bool buildCountMax = archivedStations[randIndex].amount >= randStation.buildToArchiveCount;

            if (DuplicateAmount_Stocked(randStation) || buildCountMax == false)
            {
                archivedStations.RemoveAt(randIndex);
                continue;
            }

            return randStation;
        }

        return null;
    }

    private Station_ScrObj WeightRandom_Station()
    {
        // get total wieght
        float totalWeight = 0;

        foreach (StationData data in _archiveDatas)
        {
            float dataWeight = data.amount / data.stationScrObj.buildToArchiveCount;
            totalWeight += dataWeight;
        }

        // track values
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        // get random according to weight
        for (int i = 0; i < _archiveDatas.Count; i++)
        {
            float dataWeight = _archiveDatas[i].amount / _archiveDatas[i].stationScrObj.buildToArchiveCount;
            cumulativeWeight += dataWeight;

            if (randValue >= cumulativeWeight) continue;
            return _archiveDatas[i].stationScrObj;
        }

        return null;
    }


    // All Actions
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


    // Dispose
    private bool Dispose_Restricted(Station_ScrObj checkStation)
    {
        for (int i = 0; i < _restrictedDisposables.Length; i++)
        {
            if (_restrictedDisposables[i] != checkStation) continue;
            return true;
        }

        return false;
    }

    private ItemSlot_Data Dispose_SlotData()
    {
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedDatas = slots.BookMarked_Datas(menu.currentDatas, false);
        ItemSlot_Data disposeSlot = null;

        for (int i = bookmarkedDatas.Count - 1; i >= 0; i--)
        {
            if (Dispose_Restricted(bookmarkedDatas[i].currentStation)) continue;
            disposeSlot = bookmarkedDatas[i];
        }

        return disposeSlot;
    }

    private void Dispose_BookMarkedStation()
    {
        if (_actionCoroutine != null) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_scrapStack.amountBar.Is_MaxAmount())
        {
            dialog.Update_Dialog(5);
            return;
        }

        if (Dispose_SlotData() == null)
        {
            dialog.Update_Dialog(0);
            return;
        }

        _actionCoroutine = StartCoroutine(Dispose_BookMarkedStation_Coroutine());
    }
    private IEnumerator Dispose_BookMarkedStation_Coroutine()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // dialog
        dialog.Update_Dialog(1);

        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();

        _npcController.interactable.LockInteract(true);

        // remove dispose station
        ItemSlot_Data disposeData = Dispose_SlotData();
        Station_ScrObj disposeStation = disposeData.currentStation;

        Dispose_SlotData().Empty_Item();

        // player give disopose station animation
        CoinLauncher playerLauncher = _interactable.detection.player.coinLauncher;
        playerLauncher.Parabola_CoinLaunch(disposeStation.miniSprite, transform.position);

        yield return new WaitForSeconds(_disposeWaitTime);

        CarryObject_SpriteToggle(true, _carrySprites[1]);

        // move to scrap stack
        movement.Assign_TargetPosition(_scrapStack.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        // increase scrap stack amount
        _scrapStack.amountBar.Update_Amount(_disposeScrapAmount);
        _scrapStack.amountBar.Toggle_BarColor(_scrapStack.amountBar.Is_MaxAmount());
        _scrapStack.amountBar.Load();
        _scrapStack.Update_CurrentSprite();

        Cancel_Action();
    }


    // Build
    private void Build_BookMarkedStations()
    {
        if (_actionCoroutine != null) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (StationStocks_Full())
        {
            dialog.Update_Dialog(2);
            return;
        }

        if (_scrapStack.amountBar.Is_MaxAmount() == false)
        {
            dialog.Update_Dialog(4);
            return;
        }

        // get all locked bookmark stations
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedData = slots.BookMarked_Datas(menu.currentDatas, true);

        // check if there are bookmarked stations
        if (bookmarkedData.Count <= 0)
        {
            dialog.Update_Dialog(2);
            return;
        }

        dialog.Update_Dialog(3);

        _actionCoroutine = StartCoroutine(Build_BookMarkedStations_Coroutine());
    }
    private IEnumerator Build_BookMarkedStations_Coroutine()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // get all locked bookmark stations
        StationMenu_Controller menu = _npcController.mainController.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        // get recent bookmarked station
        List<ItemSlot_Data> bookmarkedDatas = slots.BookMarked_Datas(menu.currentDatas, true);

        ItemSlot_Data bookmarkedData = bookmarkedDatas[bookmarkedDatas.Count - 1];
        Station_ScrObj recentStation = bookmarkedData.currentStation;

        // refresh movement
        NPC_Movement movement = _npcController.movement;
        movement.Stop_FreeRoam();

        _npcController.interactable.LockInteract(true);

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;

            // go to scrap
            movement.Assign_TargetPosition(_scrapStack.transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // check if bookmarked data was removed before arrival
            if (bookmarkedData.hasItem == false)
            {
                Cancel_Action();
                yield break;
            }

            CarryObject_SpriteToggle(true, _carrySprites[1]);

            // check if scrap max amount
            if (_scrapStack.amountBar.Is_MaxAmount() == false)
            {
                dialog.Update_Dialog(4);

                Cancel_Action();
                yield break;
            }

            // collect scrap
            _scrapStack.amountBar.Set_Amount(0);
            _scrapStack.amountBar.Toggle_BarColor(_scrapStack.amountBar.Is_MaxAmount());
            _scrapStack.amountBar.Load();
            _scrapStack.Update_CurrentSprite();

            // remove bookmarked station
            bookmarkedDatas[bookmarkedDatas.Count - 1].Empty_Item();
            bookmarkedDatas.RemoveAt(bookmarkedDatas.Count - 1);

            // move to random box stack
            movement.Assign_TargetPosition(_boxStackPoints[Random.Range(0, _boxStackPoints.Length)].position);
            while (movement.At_TargetPosition() == false) yield return null;

            CarryObject_SpriteToggle(true, _carrySprites[0]);

            // move to _stationStocks[i]
            movement.Assign_TargetPosition(_stationStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // add to archive
            Archive_Station(recentStation);

            // dialog
            string buildComplete = "Build Complete" + "\n\nBuild count    " + Archived_Data(recentStation).amount + "/" + recentStation.buildToArchiveCount;
            dialog.Update_Dialog(new DialogData(recentStation.dialogIcon, buildComplete));

            // restock locked bookmark station
            _stationStocks[i].Toggle_Discount(false);
            _stationStocks[i].Restock(recentStation);

            CarryObject_SpriteToggle(false, null);

            Cancel_Action();
            yield break;
        }

        Cancel_Action();
        yield break;
    }


    // Restock
    private bool Restock_Available()
    {
        if (_actionCoroutine != null) return false;

        if (StationStocks_Full()) return false;
        if (NonDuplicate_Station() == null) return false;

        if (_scrapStack.amountBar.currentAmount <= 0) return false;

        return true;
    }

    private void Restock_New()
    {
        for (int i = 0; i < _stationStocks.Length; i++)
        {
            _stationStocks[i].Restock(WeightRandom_Station());
            _stationStocks[i].Toggle_Discount(false);
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

        if (Restock_Available() == false) return;

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

            // go to scrap
            movement.Assign_TargetPosition(_scrapStack.transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // check if scrap available
            if (_scrapStack.amountBar.currentAmount <= 0)
            {
                Cancel_Action();
                yield break;
            }

            // collect scrap
            _scrapStack.amountBar.Update_Amount(-1);
            _scrapStack.amountBar.Toggle_BarColor(_scrapStack.amountBar.Is_MaxAmount());
            _scrapStack.amountBar.Load();
            _scrapStack.Update_CurrentSprite();

            CarryObject_SpriteToggle(true, _carrySprites[1]);

            // move to random box stack
            movement.Assign_TargetPosition(_boxStackPoints[Random.Range(0, _boxStackPoints.Length)].position);
            while (movement.At_TargetPosition() == false) yield return null;

            CarryObject_SpriteToggle(true, _carrySprites[0]);

            // move to _stationStocks[i]
            movement.Assign_TargetPosition(_stationStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // restock
            _stationStocks[i].Restock(NonDuplicate_Station());

            // discount tag update
            if (DiscountStock_Amount() <= 1)
            {
                Cancel_Action();
                yield break;
            }

            _stationStocks[i].Toggle_Discount(false);

            Cancel_Action();
            yield break;
        }

        Cancel_Action();
        yield break;
    }
}