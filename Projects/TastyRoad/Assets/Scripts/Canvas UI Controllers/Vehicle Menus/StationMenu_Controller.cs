using System.Collections.Generic;
using UnityEngine;

public class StationMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [SerializeField] private VehicleMenu_Controller _controller;

    [SerializeField] private Vector2 _gridData;
    [SerializeField] private List<ItemSlot> _itemSlots = new();

    private bool _interactionMode;
    private Station_Controller _interactStation;

    private int _targetNum;



    // UnityEngine
    private void Start()
    {
        Set_ItemBoxes_GridNum();
        Update_Slots();
    }

    private void OnEnable()
    {
        _controller.OnSelect_Input += Select_Slot;

        _controller.OnHoldSelect_Input += Export_StationPrefab;
        _controller.OnHoldSelect_Input += Toggle_RetrieveStations;
    }

    private void OnDisable()
    {
        _controller.OnSelect_Input -= Select_Slot;

        _controller.OnHoldSelect_Input -= Export_StationPrefab;
        _controller.OnHoldSelect_Input -= Toggle_RetrieveStations;
    }



    // ISaveLoadable
    public void Save_Data()
    {
        List<ItemSlot_Data> saveSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            saveSlots.Add(_itemSlots[i].data);
        }

        ES3.Save("stationMenuSlots", saveSlots);
    }

    public void Load_Data()
    {
        if (!ES3.KeyExists("stationMenuSlots")) return;

        List<ItemSlot_Data> loadSlots = ES3.Load("stationMenuSlots", new List<ItemSlot_Data>());

        for (int i = 0; i < loadSlots.Count; i++)
        {
            _itemSlots[i].data = loadSlots[i];
        }
    }



    // IVehicleMenu
    public List<ItemSlot> ItemSlots()
    {
        return _itemSlots;
    }

    public bool MenuInteraction_Active()
    {
        return _interactionMode;
    }

    public void Exit_MenuInteraction()
    {
        Cancel_Export();
        Cancel_Retrieve();
    }



    // All Starting Functions are Here
    private void Set_ItemBoxes_GridNum()
    {
        Vector2 gridCount = Vector2.zero;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].Assign_GridNum(gridCount);

            gridCount.x++;

            if (gridCount.x != _gridData.x) continue;

            gridCount.x = 0;
            gridCount.y++;
        }
    }

    /// <summary>
    /// Render sprites or amounts according to slot's current loaded data
    /// </summary>
    private void Update_Slots()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].Assign_Item(_itemSlots[i].data.currentStation);
        }
    }



    // Check
    public bool Slots_Full()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem) continue;
            return false;
        }

        return true;
    }

    public int Station_Amount(Station_ScrObj station)
    {
        int count = 0;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == false) continue;
            if (_itemSlots[i].data.currentStation != station) continue;
            count++;
        }

        return count;
    }



    // Menu Control
    public void Add_StationItem(Station_ScrObj station, int amount)
    {
        int repeatAmount = amount;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == true) continue;

            _itemSlots[i].Assign_Item(station);
            repeatAmount--;

            if (repeatAmount > 0) continue;
            break;
        }
    }



    // Slot and Cursor Control
    private void Select_Slot()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.cursor;

        if (cursor.data.hasItem == false)
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

    //
    private void Drag_Station()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;
        Station_ScrObj slotStation = currentSlot.data.currentStation;

        cursor.Assign_Item(slotStation);
        currentSlot.Empty_ItemBox();
    }

    //
    private void Drop_Station()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        currentSlot.Assign_Item(cursor.data.currentStation);
        cursor.Empty_Item();
    }

    //
    private void Swap_Station()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        Station_ScrObj cursorStation = cursor.data.currentStation;

        cursor.Assign_Item(currentSlot.data.currentStation);
        currentSlot.Assign_Item(cursorStation);
    }



    // Station Export System
    private void Export_StationPrefab()
    {
        ItemSlot_Cursor cursor = _controller.cursor;

        if (cursor.data.hasItem == false) return;

        _interactionMode = true;

        Vehicle_Controller vehicle = _controller.vehicleController;
        int cursorStationID = cursor.data.currentStation.id;

        _interactStation = vehicle.mainController.Spawn_Station(cursorStationID, vehicle.spawnPoint.position);

        Station_Movement movement = _interactStation.movement;

        _interactStation.Action1_Event += movement.Set_Position;
        _interactStation.detection.InteractArea_Event += movement.SetPosition_RestrictionToggle;

        _controller.OnOption1_Input += Place_StationPrefab;
    }

    private void Place_StationPrefab()
    {
        if (_interactionMode == false) return;

        Vehicle_Controller vehicle = _controller.vehicleController;

        if (_interactStation.detection.onInteractArea == false)
        {
            // return back to spawn position
            _interactStation.transform.localPosition = vehicle.spawnPoint.position;
            return;
        }

        Main_Controller main = vehicle.mainController;

        Vector2 stationPosition = Main_Controller.SnapPosition(_interactStation.transform.position);

        if (main.Position_Claimed(stationPosition)) return;

        _controller.cursor.Empty_Item();

        _interactionMode = false;
        _interactStation = null;

        main.Sort_CurrentStation_fromClosest(vehicle.transform);

        _controller.OnOption1_Input -= Place_StationPrefab;
    }

    private void Cancel_Export()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        if (cursor.data.hasItem == false) return;

        if (_interactStation == null) return;

        _interactionMode = false;

        // return exported station back to current slot
        cursor.currentSlot.Assign_Item(_interactStation.stationScrObj);
        cursor.Empty_Item();

        // destroy current exported station
        _interactStation.Destroy_Station();
        _interactStation = null;

        _controller.OnOption1_Input -= Place_StationPrefab;
    }



    // Station Retrieve System
    private void Station_TargetDirection_Control(float xInputDirection)
    {
        List<Station_Controller> retrievableStations = _controller.vehicleController.mainController.Retrievable_Stations();

        int convertedDireciton = (int)xInputDirection;
        int nextStationNum = _targetNum + convertedDireciton;

        // less than min 
        if (nextStationNum < 0)
        {
            nextStationNum = retrievableStations.Count - 1;
        }

        // more than max
        if (nextStationNum > retrievableStations.Count - 1)
        {
            nextStationNum = 0;
        }

        _interactStation.TransparentBlink_Toggle(false);

        _targetNum = nextStationNum;

        _interactStation = retrievableStations[_targetNum];
        _interactStation.TransparentBlink_Toggle(true);
    }

    private void Toggle_RetrieveStations()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.cursor;
        if (cursor.data.hasItem) return;

        List<Station_Controller> retrievableStations = _controller.vehicleController.mainController.Retrievable_Stations();
        if (retrievableStations.Count <= 0) return;

        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        cursor.data.hasItem = true;

        _interactionMode = true;
        _controller.OnCursorControl_Input += Station_TargetDirection_Control;
        _controller.OnOption1_Input += Retrieve_StationPrefab;

        _interactStation = retrievableStations[0];
        _interactStation.TransparentBlink_Toggle(true);
    }

    private void Retrieve_StationPrefab()
    {
        if (_interactionMode == false) return;

        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        // retrieve
        if (currentSlot.data.hasItem == false)
        {
            currentSlot.Assign_Item(_interactStation.stationScrObj);
        }

        // swap
        else
        {
            Add_StationItem(_interactStation.stationScrObj, 1);
        }

        // destroy selected station prefab after action 
        _interactStation.Destroy_Station();

        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        cursor.data.hasItem = false;

        _interactionMode = false;
        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnOption1_Input -= Retrieve_StationPrefab;

        _interactStation = null;
    }

    private void Cancel_Retrieve()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        if (cursor.data.hasItem) return;

        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        cursor.data.hasItem = false;

        _interactionMode = false;
        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnOption1_Input -= Retrieve_StationPrefab;

        _interactStation.TransparentBlink_Toggle(false);
        _interactStation = null;
    }
}