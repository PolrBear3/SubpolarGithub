using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu_Controller : MonoBehaviour, IVehicleMenu
{
    [SerializeField] private VehicleMenu_Controller _controller;

    [Header("")]
    [SerializeField] private Vector2 _gridData;
    [SerializeField] private List<VechiclePanel_ItemBox> _itemBoxes = new();
    [SerializeField] private int _boxCapacity;

    private bool _fridgeTargetMode;

    private List<Fridge> _currentFridges = new();
    private Fridge _currentTargetFridge;

    private int _currentFridgeNum;

    // UnityEngine
    private void Start()
    {
        Set_ItemBoxes_GridNum();

        // test function for demo
        Data_Controller data = _controller.vehicleController.mainController.dataController;

        Add_FoodItem(data.RawFood(0), 98);
        Add_FoodItem(data.RawFood(1), 98);
        Add_FoodItem(data.RawFood(2), 98);
        Add_FoodItem(data.RawFood(3), 98);
        Add_FoodItem(data.RawFood(5), 98);
    }

    private void OnEnable()
    {
        _controller.OnSelect_Input += Export_FoodItem_toFridge;
        _controller.OnSelect_Input += Fridge_TargetSystem_Toggle;
    }

    private void OnDisable()
    {
        _controller.OnSelect_Input -= Export_FoodItem_toFridge;
        _controller.OnSelect_Input -= Fridge_TargetSystem_Toggle;
    }

    // IVehicleMenu
    public List<VechiclePanel_ItemBox> ItemBoxes()
    {
        return _itemBoxes;
    }

    public bool MenuInteraction_Active()
    {
        return _fridgeTargetMode;
    }

    public void Exit_MenuInteraction()
    {
        Fridge_TargetSystem_Toggle();
    }

    // All Start Functions are Here
    private void Set_ItemBoxes_GridNum()
    {
        Vector2 gridCount = Vector2.zero;

        for (int i = 0; i < _itemBoxes.Count; i++)
        {
            _itemBoxes[i].Assign_GridNum(gridCount);

            gridCount.x++;

            if (gridCount.x != _gridData.x) continue;

            gridCount.x = 0;
            gridCount.y++;
        }
    }

    // Food to Fridge Export System
    private List<Fridge> CurrentFridges()
    {
        Main_Controller mainContrller = _controller.vehicleController.mainController;
        List<Station_Controller> currentStations = mainContrller.currentStations;

        List<Fridge> currentFridges = new();

        for (int i = 0; i < currentStations.Count; i++)
        {
            if (currentStations[i].gameObject.TryGetComponent(out Fridge fridge) == false) continue;
            currentFridges.Add(fridge);
        }

        Transform playerTransForm = _controller.vehicleController.detection.player.gameObject.transform;

        // sort closest to farthest
        currentFridges.Sort((closestFridge, farthestFridge) =>
        Vector2.Distance(closestFridge.transform.position, playerTransForm.position)
        .CompareTo(Vector2.Distance(farthestFridge.transform.position, playerTransForm.position)));

        return currentFridges;
    }

    private void Fridge_TargetSystem_Toggle()
    {
        if (_fridgeTargetMode == true)
        {
            // toggle off
            _fridgeTargetMode = false;

            _currentTargetFridge.stationController.TransparentBlink_Toggle(false);
            _currentTargetFridge = null;

            _controller.OnCursorControl_Input -= Fridge_TargetDirection_Control;

            return;
        }

        _currentFridges = CurrentFridges();

        if (_currentFridges.Count <= 0) return;

        // toggle on
        _fridgeTargetMode = true;

        _currentFridgeNum = 0;
        _currentTargetFridge = _currentFridges[_currentFridgeNum];

        _currentTargetFridge.stationController.TransparentBlink_Toggle(true);

        _controller.OnCursorControl_Input += Fridge_TargetDirection_Control;
    }

    private void Fridge_TargetDirection_Control(float xInputDirection)
    {
        if (_fridgeTargetMode == false) return;

        int convertedDireciton = (int)xInputDirection;
        int nextFridgeNum = _currentFridgeNum + convertedDireciton;

        // less than min 
        if (nextFridgeNum < 0)
        {
            nextFridgeNum = _currentFridges.Count - 1;
        }

        // more than max
        if (nextFridgeNum > _currentFridges.Count - 1)
        {
            nextFridgeNum = 0;
        }

        _currentFridges[_currentFridgeNum].stationController.TransparentBlink_Toggle(false);

        _currentFridgeNum = nextFridgeNum;

        _currentTargetFridge = _currentFridges[_currentFridgeNum];
        _currentTargetFridge.stationController.TransparentBlink_Toggle(true);
    }

    private void Export_FoodItem_toFridge()
    {
        if (_fridgeTargetMode == false) return;

        Food_ScrObj prevFood = _controller.currentItemBox.currentFood;
        int prevAmount = _controller.currentItemBox.currentAmount;

        FoodData_Controller fridgeIcon = _currentTargetFridge.foodIcon;
        FoodData fridgeData = fridgeIcon.currentFoodData;

        VechiclePanel_ItemBox box = _controller.currentItemBox;

        // current item box food = fridge food
        if (prevFood == fridgeData.foodScrObj)
        {
            Add_FoodItem(prevFood, fridgeData.currentAmount);
            fridgeIcon.Clear_Food();
            return;
        }

        box.Assign_Item(fridgeData.foodScrObj);
        box.Assign_Amount(fridgeData.currentAmount);

        // current item box > fridge
        fridgeIcon.Assign_Food(prevFood);
        fridgeIcon.FoodIcon_Transparency(false);

        int leftOver = fridgeIcon.Assign_Amount(prevAmount);

        if (leftOver <= 0) return;
        Add_FoodItem(prevFood, leftOver);
    }

    public int Add_FoodItem(Food_ScrObj food, int amount)
    {
        for (int i = 0; i < _itemBoxes.Count; i++)
        {
            if (_itemBoxes[i].hasItem == true && _itemBoxes[i].currentFood != food) continue;
            if (_itemBoxes[i].currentAmount >= _boxCapacity) continue;

            int calculatedAmount = _itemBoxes[i].currentAmount + amount;
            int leftOver = calculatedAmount - _boxCapacity;

            _itemBoxes[i].Assign_Item(food);

            if (leftOver <= 0)
            {
                _itemBoxes[i].Update_Amount(amount);
                return 0;
            }

            _itemBoxes[i].Assign_Amount(_boxCapacity);

            if (i == _itemBoxes.Count - 1) return leftOver;

            return Add_FoodItem(food, leftOver);
        }

        return amount;
    }
}