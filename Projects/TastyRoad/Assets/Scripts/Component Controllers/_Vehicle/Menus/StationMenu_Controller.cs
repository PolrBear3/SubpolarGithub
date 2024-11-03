using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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


    // Editor
    [HideInInspector] public Station_ScrObj editStation;
    [HideInInspector] public bool lockStation;


    // UnityEngine
    private void OnEnable()
    {
        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        _controller.Update_PageDots(_currentDatas.Count, _currentPageNum);

        _controller.vehicleController.interactArea.gameObject.SetActive(true);

        // subscriptions
        _controller.OnCursor_Outer += CurrentSlots_PageUpdate;

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += Toggle_RetrieveStations;
        _controller.OnHoldSelect_Input += Export_StationPrefab;

        _controller.OnOption1_Input += CurrentStation_BookmarkToggle;
        _controller.OnOption2_Input += CurrentStation_BookmarkToggle;

        _controller.OnCursor_Input += InfoBox_Update;
        _controller.OnSelect_Input += InfoBox_Update;
        _controller.OnOption1_Input += InfoBox_Update;
    }

    private void OnDisable()
    {
        // save current dragging item before menu close
        Drag_Cancel();
        _currentDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();

        _controller.vehicleController.interactArea.gameObject.SetActive(false);

        // subscriptions
        _controller.OnCursor_Outer -= CurrentSlots_PageUpdate;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= Toggle_RetrieveStations;
        _controller.OnHoldSelect_Input -= Export_StationPrefab;

        _controller.OnOption1_Input -= CurrentStation_BookmarkToggle;
        _controller.OnOption2_Input -= CurrentStation_BookmarkToggle;

        _controller.OnCursor_Input -= InfoBox_Update;
        _controller.OnSelect_Input -= InfoBox_Update;
        _controller.OnOption1_Input -= InfoBox_Update;
    }

    private void OnDestroy()
    {
        OnDisable();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("StationMenu_Controller/_currentDatas", _currentDatas);
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
        _controller.slotsController.AddNewPage_ItemSlotDatas(_currentDatas);
    }


    // IVehicleMenu
    public bool MenuInteraction_Active()
    {
        return _interactionMode;
    }


    // Menu Control
    public ItemSlot_Data Add_StationItem(Station_ScrObj station, int amount)
    {
        if (amount <= 0) return null;

        int repeatAmount = amount;

        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem == true) continue;

                StationData addStation = new(station);
                _currentDatas[i][j] = new(addStation);

                repeatAmount--;

                if (repeatAmount > 0) continue;
                return _currentDatas[i][j];
            }
        }

        return null;
    }
    public ItemSlot Add_StationItem_toSlot(Station_ScrObj station, int amount)
    {
        if (amount <= 0) return null;

        List<ItemSlot> currentSlots = _controller.slotsController.itemSlots;
        int repeatAmount = amount;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem == true) continue;

            StationData addStation = new(station);
            currentSlots[i].Assign_Data(new(addStation));

            repeatAmount--;

            if (repeatAmount > 0) continue;
            return currentSlots[i];
        }

        return null;
    }


    /// <summary>
    /// Removes all
    /// </summary>
    public void Remove_StationItem(Station_ScrObj station)
    {
        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem == false) continue;
                if (station != _currentDatas[i][j].currentStation) continue;

                _currentDatas[i][j] = new();
            }
        }
    }

    /// <summary>
    /// Removes target amount
    /// </summary>
    public void Remove_StationItem(Station_ScrObj station, int amount)
    {
        int repeatAmount = amount;

        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem == false) continue;
                if (station != _currentDatas[i][j].currentStation) continue;

                _currentDatas[i][j] = new();
                repeatAmount--;

                if (repeatAmount <= 0) return;
            }
        }
    }


    // Cursor Control
    private void Select_Slot()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

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

    private void InfoBox_Update()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;
        ItemSlot_Data slotData = cursor.currentSlot.data;

        if (!cursorData.hasItem) return;

        InformationBox info = _controller.infoBox;

        // Retrieve Status
        string retrieveStatus = "Hold <sprite=2> on empty to retrieve\n\n";

        // Action key 1
        string action1 = "Swap";
        if (slotData.hasItem == false)
        {
            action1 = "Bookmark";
        }

        // Hold Key
        string hold = "Remove";
        if (cursorData.isLocked == false)
        {
            hold = "Export";
        }

        // Set update
        string controlInfo = info.UIControl_Template(action1, action1, hold);
        info.Update_InfoText(retrieveStatus + controlInfo);
    }

    private void CurrentSlots_PageUpdate()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        // save current slots data to current page data, before moving on to next page
        _currentDatas[_currentPageNum] = new(slotsController.CurrentSlots_toDatas());

        int lastSlotNum = slotsController.itemSlots.Count - 1;

        // previous slots
        if (cursor.currentSlot.gridNum.x <= 0)
        {
            _currentPageNum = (_currentPageNum - 1 + _currentDatas.Count) % _currentDatas.Count;
            cursor.Navigate_toSlot(slotsController.ItemSlot(new(lastSlotNum, 0f)));
        }
        // next slots
        else if (cursor.currentSlot.gridNum.x >= lastSlotNum)
        {
            _currentPageNum = (_currentPageNum + 1) % _currentDatas.Count;
            cursor.Navigate_toSlot(slotsController.ItemSlot(new(0f, 0f)));
        }

        // load data to slots
        slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();

        // indicator
        _controller.Update_PageDots(_currentDatas.Count, _currentPageNum);
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
        cursor.Update_SlotIcon();

        currentSlot.Empty_ItemBox();
    }

    private void Drag_Cancel()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        if (cursor.data.hasItem == false) return;

        ItemSlot_Data cursorData = new(cursor.data);
        cursor.Empty_Item();

        _currentDatas[_currentPageNum] = slotsController.CurrentSlots_toDatas();
        Add_StationItem(cursorData.currentStation, 1).isLocked = cursorData.isLocked;

        slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();
    }


    private void Drop_Station()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.data);

        currentSlot.Assign_Data(cursorData);
        currentSlot.Update_SlotIcon();

        currentSlot.Toggle_Lock(currentSlot.data.isLocked);

        cursor.Empty_Item();
    }

    private void Swap_Station()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data currentSlotData = new(currentSlot.data);
        ItemSlot_Data cursorData = new(cursor.data);

        currentSlot.Toggle_BookMark(false);

        cursor.Assign_Data(currentSlotData);
        cursor.Update_SlotIcon();

        currentSlot.Assign_Data(cursorData);
        currentSlot.Update_SlotIcon();

        currentSlot.Toggle_Lock(currentSlot.data.isLocked);
    }


    private void CurrentStation_BookmarkToggle()
    {
        // check if it is not interaction mode
        if (_interactionMode == true) return;

        //
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;

        // check if cursor is dragging item
        if (cursorData.hasItem == false) return;

        //
        ItemSlot currentSlot = cursor.currentSlot;

        // check if current hover slot has no item
        if (currentSlot.data.hasItem)
        {
            Swap_Station();
            return;
        }

        // drop current item
        Drop_Station();

        // toggle
        currentSlot.Toggle_BookMark(true);
    }


    // Station Export System
    private void Export_StationPrefab()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        if (cursor.data.hasItem == false) return;

        if (cursor.data.isLocked == true)
        {
            cursor.Empty_Item();
            return;
        }

        _interactionMode = true;

        Vehicle_Controller vehicle = _controller.vehicleController;
        Station_ScrObj cursorStation = cursor.data.currentStation;

        _interactStation = vehicle.mainController.Spawn_Station(cursorStation, vehicle.stationSpawnPoint.position);
        _interactStation.Set_Data(new(cursor.data.stationData));

        Station_Movement movement = _interactStation.movement;

        _interactStation.Action1_Event += movement.Set_Position;

        _controller.OnOption1_Input += Place_StationPrefab;
        _controller.OnExit_Input += Cancel_Export;
    }

    private void Place_StationPrefab()
    {
        if (_interactionMode == false) return;

        Vehicle_Controller vehicle = _controller.vehicleController;

        if (_interactStation.movement.PositionSet_Available() == false) return;

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
        if (cursor.data.hasItem == false) return;

        _interactionMode = false;

        // return exported station back to current slot
        Drag_Cancel();

        // destroy current exported station
        _interactStation.Destroy_Station();
        _interactStation = null;

        _controller.OnOption1_Input -= Place_StationPrefab;
        _controller.OnExit_Input -= Cancel_Export;
    }


    // Station Retrieve System
    private List<Station_Controller> Retrieve_Stations()
    {
        Vehicle_Controller vehicle = _controller.vehicleController;
        List<Station_Controller> currentStations = new(vehicle.mainController.CurrentStations(false));

        for (int i = currentStations.Count - 1; i >= 0; i--)
        {
            if (vehicle.Is_InteractArea(currentStations[i].transform.position)) continue;
            currentStations.RemoveAt(i);
        }

        return currentStations;
    }


    private void Station_TargetDirection_Control(float xInputDirection)
    {
        List<Station_Controller> retrieveStations = new(Retrieve_Stations());

        int convertedDireciton = (int)xInputDirection;
        int nextStationNum = _targetNum + convertedDireciton;

        // less than min 
        if (nextStationNum < 0)
        {
            nextStationNum = retrieveStations.Count - 1;
        }

        // more than max
        if (nextStationNum > retrieveStations.Count - 1)
        {
            nextStationNum = 0;
        }

        _interactStation.TransparentBlink_Toggle(false);
        _interactStation.movement.movementArrows.SetActive(false);

        _targetNum = nextStationNum;
        _interactStation = retrieveStations[_targetNum];

        _interactStation.TransparentBlink_Toggle(true);
        _interactStation.movement.movementArrows.SetActive(true);
    }

    private void Toggle_RetrieveStations()
    {
        if (_interactionMode) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.data.hasItem) return;
        if (cursor.currentSlot.data.isLocked) return;

        List<Station_Controller> retrieveStations = new(Retrieve_Stations());

        if (retrieveStations.Count <= 0) return;

        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        cursor.data.hasItem = true;

        _interactionMode = true;
        _controller.OnCursorControl_Input += Station_TargetDirection_Control;
        _controller.OnOption1_Input += Retrieve_StationPrefab;
        _controller.OnExit_Input += Cancel_Retrieve;

        _interactStation = retrieveStations[0];

        _interactStation.TransparentBlink_Toggle(true);
        _interactStation.movement.movementArrows.SetActive(true);
    }


    private void Retrieve_StationPrefab()
    {
        if (_interactionMode == false) return;

        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        StationData interactStationData = new(_interactStation.data);

        // destroy prefab for retrieve to slot
        _interactStation.Destroy_Station();

        // retrieve
        if (currentSlot.data.hasItem == false)
        {
            currentSlot.Assign_Data(new(interactStationData));
            currentSlot.Update_SlotIcon();

            currentSlot.data.Update_StationData(interactStationData);
        }
        // swap
        else
        {
            Main_Controller main = _controller.vehicleController.mainController;

            // exporting station
            Station_Controller exportStation = main.Spawn_Station(currentSlot.data.currentStation, _interactStation.transform.position);
            exportStation.movement.Load_Position();

            // retrieving station
            currentSlot.Assign_Data(new(interactStationData));
            currentSlot.Update_SlotIcon();
        }

        // empty cursor
        cursor.Assign_Data(new());

        _interactionMode = false;
        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnOption1_Input -= Retrieve_StationPrefab;
        _controller.OnExit_Input -= Cancel_Retrieve;

        _interactStation = null;
    }

    private void Cancel_Retrieve()
    {
        // cursor actually doesn't have item but need this for OnOption1_Input?.Invoke(); to work on VehicleMenu_Controller
        _controller.slotsController.cursor.data.hasItem = false;

        _interactionMode = false;

        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnOption1_Input -= Retrieve_StationPrefab;
        _controller.OnExit_Input -= Cancel_Retrieve;

        _interactStation.TransparentBlink_Toggle(false);
        _interactStation.movement.movementArrows.SetActive(false);

        _interactStation = null;
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(StationMenu_Controller))]
public class StationMenu_Controller_Inspector : Editor
{
    //
    private SerializedProperty editStationProp;
    private SerializedProperty lockStationProp;

    private void OnEnable()
    {
        editStationProp = serializedObject.FindProperty("editStation");
        lockStationProp = serializedObject.FindProperty("lockStation");
    }


    //
    public override void OnInspectorGUI()
    {
        StationMenu_Controller menu = (StationMenu_Controller)target;

        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);

        if (GUILayout.Button("Add New Page of Slots"))
        {
            menu.controller.slotsController.AddNewPage_ItemSlotDatas(menu.currentDatas);
        }
        GUILayout.Space(20);

        //
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(editStationProp, GUIContent.none);
        Station_ScrObj editStation = (Station_ScrObj)editStationProp.objectReferenceValue;

        EditorGUILayout.PropertyField(lockStationProp, GUIContent.none);
        bool lockStation = lockStationProp.boolValue;

        if (GUILayout.Button("Add Station"))
        {
            menu.Add_StationItem(editStation, 1).isLocked = lockStation;
        }

        EditorGUILayout.EndHorizontal();
        //

        if (GUILayout.Button("Remove Station"))
        {
            menu.Remove_StationItem(editStation, 1);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif