using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class StationShopNPC : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private NPC_Controller _npcController;
    [SerializeField] private ActionBubble_Interactable _interactable;

    [SerializeField] private Clock_Timer _actionTimer;

    [Space(20)]
    [SerializeField] private SpriteRenderer _carryObject;
    [SerializeField] private Sprite[] _carrySprites;

    [Space(20)]
    [SerializeField] private SubLocation _currentSubLocation;

    [Space(20)]
    [SerializeField] private ScrapStack _scrapStack;

    [SerializeField][Range(0, 10)] private float _disposeWaitTime;
    [SerializeField][Range(0, 10)] private int _disposeScrapAmount;

    [SerializeField] private Station_ScrObj[] _restrictedDisposables;

    [Space(20)]
    [SerializeField] private Transform[] _boxStackPoints;
    [SerializeField] private StationStock[] _stationStocks;

    [Space(20)]
    [SerializeField][Range(1, 100)] private int _duplicateAmount;
    
    [Space(60)] 
    [SerializeField] private VideoGuide_Trigger _guideTrigger;
    

    private StationShopNPC_Data _data;
    private Coroutine _actionCoroutine;
    

    // UnityEngine
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        ResumeBuild_BookMarkedStation();
        
        // untrack
        Main_Controller.instance.UnTrack_CurrentCharacter(gameObject);

        // event subscriptions
        _npcController.movement.TargetPosition_UpdateEvent += CarryObject_DirectionUpdate;

        // subscription
        WorldMap_Controller worldMap = Main_Controller.instance.worldMap;
        worldMap.OnNewLocation += Restock_New;
        
        globaltime.instance.OnDayTime += Restock_ArchivedStation;
        
        _interactable.OnInteract += _guideTrigger.Trigger_CurrentGuide;

        _interactable.OnInteract += Cancel_Action;
        _interactable.OnInteract += Interact_FacePlayer;

        _interactable.OnAction1 += Dispose_BookMarkedStation;
        _interactable.OnAction2 += Build_BookMarkedStation;

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
        WorldMap_Controller worldMap = Main_Controller.instance.worldMap;
        worldMap.OnNewLocation -= Restock_New;
        
        globaltime.instance.OnDayTime -= Restock_ArchivedStation;
        
        _interactable.OnInteract -= _guideTrigger.Trigger_CurrentGuide;

        _interactable.OnInteract -= Cancel_Action;
        _interactable.OnInteract -= Interact_FacePlayer;

        _interactable.OnAction1 -= Dispose_BookMarkedStation;
        _interactable.OnAction2 -= Build_BookMarkedStation;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("StationShopNPC/StationShopNPC_Data", _data);
        
        Save_StationStocks();
    }

    public void Load_Data()
    {
        _data = ES3.Load("StationShopNPC/StationShopNPC_Data", new StationShopNPC_Data(new()));
        
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
        movement.Free_Roam(_currentSubLocation.roamArea, movement.intervalTime);
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
    private void Archive_Station(Station_ScrObj station)
    {
        List<StationData> archiveDatas = _data.archiveDatas;
        
        for (int i = 0; i < archiveDatas.Count; i++)
        {
            // check if archived
            if (station != archiveDatas[i].stationScrObj) continue;

            archiveDatas[i].Update_Amount(1);
            archiveDatas[i].Set_Amount(Mathf.Clamp(archiveDatas[i].amount, 1, station.buildToArchiveCount));

            return;
        }

        // archive new
        archiveDatas.Add(new(station, 1));
    }

    private StationData Archived_StationData(Station_ScrObj station)
    {
        List<StationData> archiveDatas = _data.archiveDatas;
        
        for (int i = 0; i < archiveDatas.Count; i++)
        {
            if (station != archiveDatas[i].stationScrObj) continue;
            return archiveDatas[i];
        }
        return null;
    }


    private int ArchivedStation_BuildCount(Station_ScrObj station)
    {
        StationData archivedData = Archived_StationData(station);

        if (archivedData == null) return 0;
        return archivedData.amount;
    }

    private bool BuildArchiveCount_Maxed(Station_ScrObj station)
    {
        StationData targetData = Archived_StationData(station);

        if (targetData == null) return false;
        if (targetData.amount < station.buildToArchiveCount) return false;

        return true;
    }


    private List<Station_ScrObj> BuildArchiveCount_MaxStations()
    {
        List<StationData> archiveDatas = _data.archiveDatas;
        List<Station_ScrObj> archivedStations = new();

        for (int i = 0; i < archiveDatas.Count; i++)
        {
            Station_ScrObj station = archiveDatas[i].stationScrObj;

            if (archiveDatas[i].amount < station.buildToArchiveCount) continue;
            archivedStations.Add(station);
        }

        return archivedStations;
    }

    private Station_ScrObj MaxBuildCount_RandomStation()
    {
        List<StationData> archiveDatas = _data.archiveDatas;
        int totalWeight = 0;

        foreach (StationData data in archiveDatas)
        {
            if (data.amount < data.stationScrObj.buildToArchiveCount) continue;
            totalWeight += data.amount;
        }

        int randValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < archiveDatas.Count; i++)
        {
            Station_ScrObj archiveStation = archiveDatas[i].stationScrObj;
            int buildCount = archiveDatas[i].amount;

            if (buildCount < archiveStation.buildToArchiveCount) continue;

            cumulativeWeight += buildCount;

            if (randValue >= cumulativeWeight) continue;
            return archiveStation;
        }

        return null;
    }


    // Duplicate Check Station Data
    private bool DuplicateAmount_Stocked(Station_ScrObj checkStation)
    {
        int checkCount = 0;
        int duplicateAmount = 1;

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold) continue;
            if (_stationStocks[i].currentStation.stationScrObj != checkStation) continue;
            checkCount++;
        }
        
        List<StationData> archiveDatas = _data.archiveDatas;

        if (archiveDatas.Count < _stationStocks.Length)
        {
            duplicateAmount = _duplicateAmount;
        }
        
        if (checkCount < duplicateAmount) return false;
        return true;
    }


    private Station_ScrObj NonDuplicate_RandomStation(List<Station_ScrObj> stations)
    {
        if (stations.Count <= 0) return null;

        List<Station_ScrObj> archivedStations = new(stations);

        while (archivedStations.Count > 0)
        {
            int randIndex = Random.Range(0, archivedStations.Count);
            Station_ScrObj randStation = archivedStations[randIndex];

            if (DuplicateAmount_Stocked(randStation))
            {
                archivedStations.RemoveAt(randIndex);
                continue;
            }

            return randStation;
        }

        return null;
    }

    private List<Station_ScrObj> NonDuplicate_Stations(List<Station_ScrObj> stations)
    {
        List<Station_ScrObj> targetStations = new();

        for (int i = 0; i < stations.Count; i++)
        {
            if (DuplicateAmount_Stocked(stations[i])) continue;

            targetStations.Add(stations[i]);
        }

        return targetStations;
    }


    // Action Control
    private void Start_Action()
    {
        if (_actionCoroutine != null) return;
        
        _npcController.interactable.LockInteract(true);
        _actionTimer.Toggle_RunAnimation(true);
        
        NPC_Movement movement = _npcController.movement;
        
        movement.Stop_FreeRoam();
        movement.Set_MoveSpeed(movement.defaultMoveSpeed + 3);
    }
    
    private void Cancel_Action()
    {
        _data.Reset_ActionState();
        
        if (_actionCoroutine == null) return;

        StopCoroutine(_actionCoroutine);
        _actionCoroutine = null;

        _npcController.interactable.LockInteract(false);
        _actionTimer.Toggle_RunAnimation(false);

        CarryObject_SpriteToggle(false, null);

        // return to free roam
        NPC_Movement move = _npcController.movement;

        move.Stop_FreeRoam();
        move.Set_MoveSpeed(move.defaultMoveSpeed);
        
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
        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
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
        Start_Action();
        
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // dialog
        dialog.Update_Dialog(1);

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
        NPC_Movement movement = _npcController.movement;
        
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
    private void Build_BookMarkedStation()
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
        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;
        ItemSlots_Controller slots = menu.controller.slotsController;

        List<ItemSlot_Data> bookmarkedData = slots.BookMarked_Datas(menu.currentDatas, true);

        // check if there are bookmarked stations
        if (bookmarkedData.Count <= 0)
        {
            dialog.Update_Dialog(2);
            return;
        }

        dialog.Update_Dialog(3);

        _actionCoroutine = StartCoroutine(Build_BookMarkedStation_Coroutine());
    }
    private IEnumerator Build_BookMarkedStation_Coroutine()
    {
        Start_Action();
        
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // get all locked bookmark stations
        VehicleMenu_Controller menu = Main_Controller.instance.currentVehicle.menu;
        StationMenu_Controller stationMenu = menu.stationMenu;

        ItemSlots_Controller slots = stationMenu.controller.slotsController;

        // get recent bookmarked station
        List<ItemSlot_Data> bookmarkedDatas = slots.BookMarked_Datas(stationMenu.currentDatas, true);

        ItemSlot_Data bookmarkedData = bookmarkedDatas[bookmarkedDatas.Count - 1];
        Station_ScrObj recentStation = bookmarkedData.currentStation;
        
        _data.Build_Station(recentStation);

        // refresh movement
        NPC_Movement movement = _npcController.movement;

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
            _data.Collect_Scrap();
            
            _scrapStack.amountBar.Set_Amount(0);
            _scrapStack.amountBar.Toggle_BarColor(_scrapStack.amountBar.Is_MaxAmount());
            _scrapStack.amountBar.Load();
            _scrapStack.Update_CurrentSprite();
 
            // remove bookmarked station
            bookmarkedDatas[bookmarkedDatas.Count - 1].Empty_Item();
            bookmarkedDatas.RemoveAt(bookmarkedDatas.Count - 1);

            menu.Update_ItemSlots(stationMenu.gameObject, stationMenu.currentDatas[stationMenu.currentPageNum]);

            // move to random box stack
            movement.Assign_TargetPosition(_boxStackPoints[Random.Range(0, _boxStackPoints.Length)].position);
            while (movement.At_TargetPosition() == false) yield return null;

            CarryObject_SpriteToggle(true, _carrySprites[0]);

            // move to _stationStocks[i]
            movement.Assign_TargetPosition(_stationStocks[i].transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            // add to archive
            int restockPrice = ArchivedStation_BuildCount(recentStation) <= 0 ? 0 : recentStation.price;
            Archive_Station(recentStation);

            // dialog
            DialogBox updateBox = dialog.Update_Dialog(6);
            updateBox.Set_IconImage(recentStation.dialogIcon);
            
            Dictionary<string, string> buildCountStrings = new()
            {
                { "currentCount", ArchivedStation_BuildCount(recentStation).ToString() },
                { "maxCount", recentStation.buildToArchiveCount.ToString() }
            };
            updateBox.data.Set_SmartInfo(buildCountStrings);
            Main_Controller.instance.dialogSystem.RefreshCurrent_DialogInfo();
            
            // restock station blueprint bookmark station
            _stationStocks[i].Toggle_Discount(false);
            _stationStocks[i].Restock(new(recentStation, restockPrice));

            CarryObject_SpriteToggle(false, null);
            
            // quest
            TutorialQuest_Controller quest = TutorialQuest_Controller.instance;
            
            quest.Complete_Quest("BuildStation", 1);
            quest.Complete_Quest("Build" + recentStation.stationName, 1);

            Cancel_Action();
            yield break;
        }

        Cancel_Action();
        yield break;
    }

    private void ResumeBuild_BookMarkedStation()
    {
        if (_data.scrapCollected == false) return;

        Station_ScrObj buildStation = _data.buildStation;

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;
            
            // set price
            int restockPrice = ArchivedStation_BuildCount(buildStation) <= 0 ? 0 : buildStation.price;
            
            // restock blueprint bookmark station
            _stationStocks[i].Toggle_Discount(false);
            _stationStocks[i].Restock(new(buildStation, restockPrice));
            
            Archive_Station(buildStation);
            
            // quest
            TutorialQuest_Controller quest = TutorialQuest_Controller.instance;
            
            quest.Complete_Quest("BuildStation", 1);
            quest.Complete_Quest("Build" + buildStation.stationName, 1);

            break;
        }
        
        _data.Reset_ActionState();
    }


    // Restock
    private bool Restock_Available()
    {
        if (_actionCoroutine != null) return false;
        if (_scrapStack.amountBar.currentAmount <= 0) return false;

        if (StationStocks_Full()) return false;
        if (NonDuplicate_Stations(BuildArchiveCount_MaxStations()).Count <= 0) return false;

        return true;
    }


    private void Clear_StationStocks()
    {
        foreach (StationStock stock in _stationStocks)
        {
            stock.Update_toSold();
        }
    }

    private void Restock_New()
    {
        ResumeBuild_BookMarkedStation();
        Clear_StationStocks();

        for (int i = 0; i < _stationStocks.Length; i++)
        {
            Station_ScrObj restockStation = MaxBuildCount_RandomStation();

            if (restockStation == null) return;
            if (DuplicateAmount_Stocked(restockStation)) continue;

            _stationStocks[i].Restock(new(restockStation, restockStation.price));
            _stationStocks[i].Toggle_Discount(false);
        }
    }


    private void Restock_ArchivedStation()
    {
        if (Restock_Available() == false) return;

        _actionCoroutine = StartCoroutine(Restock_ArchivedStation_Coroutine());
    }
    private IEnumerator Restock_ArchivedStation_Coroutine()
    {
        Start_Action();
        
        for (int i = 0; i < _stationStocks.Length; i++)
        {
            if (_stationStocks[i].sold == false) continue;

            NPC_Movement movement = _npcController.movement;

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
            Station_ScrObj restockStation = NonDuplicate_RandomStation(BuildArchiveCount_MaxStations());
            _stationStocks[i].Restock(new StationData(restockStation, restockStation.price));

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
