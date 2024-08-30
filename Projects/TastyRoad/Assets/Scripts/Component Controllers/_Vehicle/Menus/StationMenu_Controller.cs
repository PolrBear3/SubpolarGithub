using System.Collections.Generic;
using UnityEngine;

public class StationMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;
    public VehicleMenu_Controller controller => _controller;

    private Dictionary<int, List<ItemSlot_Data>> _currentDatas = new();
    public Dictionary<int, List<ItemSlot_Data>> currentDatas => _currentDatas;

    private int _currentPageNum;
    public int currentPageNum => _currentPageNum;

    [Header("")]
    private bool _interactionMode;
    private Station_Controller _interactStation;

    private int _targetNum;


    // UnityEngine
    private void OnEnable()
    {
        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += Export_StationPrefab;
        _controller.OnHoldSelect_Input += Toggle_RetrieveStations;

        _controller.OnOption1_Input += CurrentStation_BookmarkToggle;
    }

    private void OnDisable()
    {
        // save current dragging item before menu close
        Drag_Cancel();
        _currentDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= Export_StationPrefab;
        _controller.OnHoldSelect_Input -= Toggle_RetrieveStations;

        _controller.OnOption1_Input -= CurrentStation_BookmarkToggle;
    }

    private void OnDestroy()
    {
        OnDisable();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("StationMenu_Controller/_currentDatas", _currentDatas);
        _currentDatas = ES3.Load("StationMenu_Controller/_currentDatas", _currentDatas);
    }

    public void Load_Data()
    {

        // load saved slot datas
        if (ES3.KeyExists("StationMenu_Controller/_currentDatas"))
        {
            _currentDatas = ES3.Load("StationMenu_Controller/_currentDatas", _currentDatas);
            return;
        }

        // set new slot datas
        List<ItemSlot_Data> newDatas = new();
        for (int i = 0; i < _controller.slotsController.itemSlots.Count; i++)
        {
            newDatas.Add(new());
        }

        _currentDatas.Add(0, newDatas);
    }


    // IVehicleMenu
    public bool MenuInteraction_Active()
    {
        return _interactionMode;
    }


    // Menu Control
    public ItemSlot_Data Add_StationItem_Data(Station_ScrObj station, int amount)
    {
        if (amount <= 0) return null;

        List<ItemSlot_Data> datas = _currentDatas[_currentPageNum];
        int repeatAmount = amount;

        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].hasItem == true) continue;

            datas[i] = new(station, 1);
            repeatAmount--;

            if (repeatAmount > 0) continue;
            return datas[i];
        }

        return null;
    }
    public ItemSlot Add_StationItem_Slot(Station_ScrObj station, int amount)
    {
        if (amount <= 0) return null;

        List<ItemSlot> currentSlots = _controller.slotsController.itemSlots;
        int repeatAmount = amount;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem == true) continue;

            ItemSlot addSlot = currentSlots[i].Assign_Item(station);
            repeatAmount--;

            if (repeatAmount > 0) continue;
            return addSlot;
        }

        return null;
    }


    /// <summary>
    /// Removes all
    /// </summary>
    public void Remove_StationItem(Station_ScrObj station)
    {
        List<ItemSlot_Data> datas = _currentDatas[_currentPageNum];

        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].hasItem == false) continue;
            if (station != datas[i].currentStation) continue;

            datas[i] = new();
        }
    }

    /// <summary>
    /// Removes target amount
    /// </summary>
    public void Remove_StationItem(Station_ScrObj station, int amount)
    {
        List<ItemSlot_Data> datas = _currentDatas[_currentPageNum];
        int repeatAmount = amount;

        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].hasItem == false) continue;
            if (station != datas[i].currentStation) continue;

            datas[i] = new();
            repeatAmount--;

            if (repeatAmount <= 0) return;
        }
    }


    // Cursor Control
    private void Select_Slot()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.Current_Data().hasItem == false)
        {
            Drag_Station();
            return;
        }

        if (cursor.currentSlot.data.hasItem)
        {
            Swap_Station();
            return;
        }

        Drop_Station();
    }


    private void Drag_Station()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data slotData = currentSlot.data;
        Station_ScrObj slotStation = slotData.currentStation;

        currentSlot.Toggle_BookMark(false);
        ItemSlot_Data currentSlotData = new(currentSlot.data);

        cursor.Assign_Data(currentSlotData);
        cursor.Assign_Item(slotStation);

        currentSlot.Empty_ItemBox();
    }

    private void Drag_Cancel()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.Current_Data().hasItem == false) return;

        ItemSlot_Data cursorData = new(cursor.Current_Data());
        cursor.Empty_Item();

        ItemSlot targetSlot = Add_StationItem_Slot(cursorData.currentStation, 1);
        targetSlot.Assign_Data(cursorData);

        targetSlot.Toggle_Lock(targetSlot.data.isLocked);
    }


    private void Drop_Station()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.Current_Data());

        currentSlot.Assign_Item(cursor.Current_Data().currentStation).Assign_Data(cursorData);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);

        cursor.Empty_Item();
    }

    private void Swap_Station()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data currentSlotData = new(currentSlot.data);
        ItemSlot_Data cursorData = new(cursor.Current_Data());

        currentSlot.Toggle_BookMark(false);

        cursor.Assign_Data(currentSlotData);
        cursor.Assign_Item(currentSlot.data.currentStation);

        currentSlot.Assign_Item(cursorData.currentStation).Assign_Data(cursorData);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);
    }


    private void CurrentStation_BookmarkToggle()
    {
        // check if it is not interaction mode
        if (_interactionMode == true) return;

        //
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.Current_Data();

        // check if cursor is dragging item
        if (cursorData.hasItem == false) return;

        //
        ItemSlot currentSlot = cursor.currentSlot;

        // check if current hover slot has no item
        if (currentSlot.data.hasItem) return;

        // drop current item
        Drop_Station();

        // toggle
        currentSlot.Toggle_BookMark(true);
    }


    // Data Control
    public bool Slots_Full()
    {
        List<ItemSlot_Data> data = _currentDatas[_currentPageNum];

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].hasItem == true) continue;
            return false;
        }

        return true;
    }

    public int Station_Amount(Station_ScrObj station)
    {
        List<ItemSlot_Data> datas = _currentDatas[_currentPageNum];
        int count = 0;

        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].hasItem == false) continue;
            if (datas[i].currentStation != station) continue;
            count++;
        }

        return count;
    }


    // Station Export System
    private void Export_StationPrefab()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        if (cursor.Current_Data().hasItem == false) return;

        if (cursor.Current_Data().isLocked == true)
        {
            Drag_Cancel();
            return;
        }

        _interactionMode = true;

        Vehicle_Controller vehicle = _controller.vehicleController;
        Station_ScrObj cursorStation = cursor.Current_Data().currentStation;

        _interactStation = vehicle.mainController.Spawn_Station(cursorStation, vehicle.stationSpawnPoint.position);

        Station_Movement movement = _interactStation.movement;
        _interactStation.Action1_Event += movement.Set_Position;

        _controller.OnOption1_Input += Place_StationPrefab;
        _controller.OnExit_Input += Cancel_Export;
    }

    private void Place_StationPrefab()
    {
        if (_interactionMode == false) return;

        Vehicle_Controller vehicle = _controller.vehicleController;

        if (_interactStation.movement.PositionSet_Available() == false)
        {
            // return back to spawn position
            _interactStation.transform.localPosition = vehicle.stationSpawnPoint.position;
            return;
        }

        Complete_StationPlace();
    }
    
    private void Complete_StationPlace()
    {
        _controller.slotsController.cursor.Empty_Item();

        _interactionMode = false;
        _interactStation = null;

        Vehicle_Controller vehicle = _controller.vehicleController;
        Main_Controller main = vehicle.mainController;

        main.Sort_CurrentStation_fromClosest(vehicle.transform);

        _controller.OnOption1_Input -= Place_StationPrefab;
        _controller.OnExit_Input -= Cancel_Export;
    }

    private void Cancel_Export()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        if (cursor.Current_Data().hasItem == false) return;

        _interactionMode = false;

        // return exported station back to current slot
        Add_StationItem_Slot(_interactStation.stationScrObj, 1);
        cursor.Empty_Item();

        // destroy current exported station
        _interactStation.Destroy_Station();
        _interactStation = null;

        _controller.OnOption1_Input -= Place_StationPrefab;
        _controller.OnExit_Input -= Cancel_Export;
    }


    // Station Retrieve System
    private void Station_TargetDirection_Control(float xInputDirection)
    {
        List<Station_Controller> currentStations = _controller.vehicleController.mainController.CurrentStations(false);

        int convertedDireciton = (int)xInputDirection;
        int nextStationNum = _targetNum + convertedDireciton;

        // less than min 
        if (nextStationNum < 0)
        {
            nextStationNum = currentStations.Count - 1;
        }

        // more than max
        if (nextStationNum > currentStations.Count - 1)
        {
            nextStationNum = 0;
        }

        _interactStation.TransparentBlink_Toggle(false);
        _interactStation.movement.movementArrows.SetActive(false);

        _targetNum = nextStationNum;
        _interactStation = currentStations[_targetNum];

        _interactStation.TransparentBlink_Toggle(true);
        _interactStation.movement.movementArrows.SetActive(true);
    }

    private void Toggle_RetrieveStations()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.Current_Data().hasItem) return;
        if (cursor.currentSlot.data.isLocked) return;

        List<Station_Controller> currentStations = _controller.vehicleController.mainController.CurrentStations(false);

        if (currentStations.Count <= 0) return;

        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        cursor.Current_Data().hasItem = true;

        _interactionMode = true;
        _controller.OnCursorControl_Input += Station_TargetDirection_Control;
        _controller.OnOption1_Input += Retrieve_StationPrefab;
        _controller.OnExit_Input += Cancel_Retrieve;

        _interactStation = currentStations[0];

        _interactStation.TransparentBlink_Toggle(true);
        _interactStation.movement.movementArrows.SetActive(true);
    }

    private void Retrieve_StationPrefab()
    {
        if (_interactionMode == false) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        Station_ScrObj interactStation = _interactStation.stationScrObj;

        // destroy prefab for retrieve to slot
        _interactStation.Destroy_Station();

        // retrieve
        if (currentSlot.data.hasItem == false)
        {
            currentSlot.Assign_Data(new(interactStation, 1));
            currentSlot.Assign_Item();
        }

        // swap
        else
        {
            Main_Controller main = _controller.vehicleController.mainController;

            // exporting station
            Station_Controller exportStation = main.Spawn_Station(currentSlot.data.currentStation, _interactStation.transform.position);
            exportStation.movement.Load_Position();

            // retrieving station
            currentSlot.Assign_Data(new(interactStation, 1));
            currentSlot.Assign_Item();
        }

        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        cursor.Assign_Data(new());
        currentSlot.Assign_Item();

        _interactionMode = false;
        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnOption1_Input -= Retrieve_StationPrefab;
        _controller.OnExit_Input -= Cancel_Retrieve;

        _interactStation = null;
    }

    private void Cancel_Retrieve()
    {
        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        _controller.slotsController.cursor.Current_Data().hasItem = false;

        _interactionMode = false;

        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnOption1_Input -= Retrieve_StationPrefab;
        _controller.OnExit_Input -= Cancel_Retrieve;

        _interactStation.TransparentBlink_Toggle(false);
        _interactStation.movement.movementArrows.SetActive(false);

        _interactStation = null;
    }
}