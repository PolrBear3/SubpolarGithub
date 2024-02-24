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

        // test function for demo
        Data_Controller data = _controller.vehicleController.mainController.dataController;

        Load_Data();

        if (Main_Controller.gameSaved) return;

        Add_StationItem(data.Station_ScrObj(0), 1);

        Add_StationItem(data.Station_ScrObj(3), 1);
        Add_StationItem(data.Station_ScrObj(301), 1);

        Add_StationItem(data.Station_ScrObj(5), 1);
        Add_StationItem(data.Station_ScrObj(4), 1);

        Add_StationItem(data.Station_ScrObj(1), 4);
        Add_StationItem(data.Station_ScrObj(2), 1);
    }

    private void OnEnable()
    {
        _controller.OnSelect_Input += Export_StationPrefab;
    }

    private void OnDisable()
    {
        _controller.OnSelect_Input -= Export_StationPrefab;
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
        List<ItemSlot_Data> loadSlots = ES3.Load("stationMenuSlots", new List<ItemSlot_Data>());

        for (int i = 0; i < loadSlots.Count; i++)
        {
            _itemSlots[i].data = loadSlots[i];

            _itemSlots[i].Assign_Item(_itemSlots[i].data.currentStation);
        }
    }



    // IVehicleMenu
    public List<ItemSlot> ItemBoxes()
    {
        return _itemSlots;
    }

    public bool MenuInteraction_Active()
    {
        return _interactionMode;
    }

    public void Exit_MenuInteraction()
    {
        Retrieve_Station_Toggle();
        Cancel_Export();
    }



    // All Start Functions are Here
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



    // Station Export System
    public void Add_StationItem(Station_ScrObj station, int amount)
    {
        List<ItemSlot> itemBoxes = _controller.itemBoxes;

        int repeatAmount = amount;

        for (int i = 0; i < itemBoxes.Count; i++)
        {
            if (itemBoxes[i].data.hasItem == true) continue;

            itemBoxes[i].Assign_Item(station);
            repeatAmount--;

            if (repeatAmount > 0) continue;
            break;
        }
    }

    private void Export_StationPrefab()
    {
        ItemSlot currentBox = _controller.currentItemBox;

        if (currentBox.data.hasItem == false)
        {
            // updates to retrieve mode if item box is empty
            Retrieve_Station_Toggle();
            return;
        }

        Vehicle_Controller vehicle = _controller.vehicleController;

        if (_interactionMode == true)
        {
            if (_interactStation.detection.onInteractArea == false)
            {
                // return back to spawn position
                _interactStation.transform.localPosition = vehicle.spawnPoint.position;

                return;
            }

            Main_Controller main = vehicle.mainController;

            Vector2 stationPosition = Main_Controller.SnapPosition(_interactStation.transform.position);

            if (main.Position_Claimed(stationPosition)) return;

            _interactionMode = false;
            _interactStation = null;

            currentBox.Update_Amount(-1);

            main.Sort_CurrentStation_fromClosest(vehicle.transform);

            return;
        }

        _interactionMode = true;

        int currentStationID = currentBox.data.currentStation.id;

        // instantiate selected station prefab at spawn position
        _interactStation = vehicle.mainController.Spawn_Station(currentStationID, vehicle.spawnPoint.position);

        Station_Movement movement = _interactStation.movement;

        _interactStation.Interact_Event += movement.Set_Position;
        _interactStation.detection.InteractArea_Event += movement.SetPosition_RestrictionToggle;
    }

    private void Cancel_Export()
    {
        if (_interactStation == null) return;

        _interactionMode = false;

        // destroy current exported station
        _interactStation.Destroy_Station();
        _interactStation = null;
    }



    // Station Retrieve System
    private void Retrieve_Station_Toggle()
    {
        ItemSlot currentBox = _controller.currentItemBox;

        if (currentBox.data.hasItem == true) return;

        List<Station_Controller> currentStations = _controller.vehicleController.mainController.currentStations;
        if (currentStations.Count <= 0) return;

        if (_interactionMode == true)
        {
            // retrieve target off
            _interactionMode = false;
            _interactStation.TransparentBlink_Toggle(false);
            _interactStation = null;

            _controller.OnSelect_Input += Export_StationPrefab;

            _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
            _controller.OnSelect_Input -= Retrieve_Station;

            return;
        }

        // retrieve target on
        _interactionMode = true;
        _targetNum = 0;

        _interactStation = currentStations[_targetNum];
        _interactStation.TransparentBlink_Toggle(true);

        _controller.OnSelect_Input -= Export_StationPrefab;

        _controller.OnCursorControl_Input += Station_TargetDirection_Control;
        _controller.OnSelect_Input += Retrieve_Station;
    }

    private void Station_TargetDirection_Control(float xInputDirection)
    {
        List<Station_Controller> currentStations = _controller.vehicleController.mainController.currentStations;

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

        _targetNum = nextStationNum;

        _interactStation = currentStations[_targetNum];
        _interactStation.TransparentBlink_Toggle(true);
    }

    private void Retrieve_Station()
    {
        // add current station to current empty item box
        _controller.currentItemBox.Assign_Item(_interactStation.stationScrObj);

        // retrieve food
        if (_interactStation.Food_Icon() != null)
        {
            FoodData_Controller foodIcon = _interactStation.Food_Icon();

            Food_ScrObj currentFood = foodIcon.currentFoodData.foodScrObj;

            // retrieve only raw foods
            if (_controller.vehicleController.mainController.dataController.CookedFood(currentFood) == null)
            {
                int currentAmount = foodIcon.currentFoodData.currentAmount;

                _controller.foodMenu.Add_FoodItem(currentFood, currentAmount);
            }
        }

        // destroy current selected station
        _interactStation.Destroy_Station();

        // retrieve target off
        _interactionMode = false;
        _interactStation = null;

        // update event subscription
        _controller.OnSelect_Input += Export_StationPrefab;

        _controller.OnCursorControl_Input -= Station_TargetDirection_Control;
        _controller.OnSelect_Input -= Retrieve_Station;
    }
}