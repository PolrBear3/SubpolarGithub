using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehiclePanel_Controller : MonoBehaviour
{
    [SerializeField] private List<VechiclePanel_ItemBox> _itemBoxes = new();
    private VechiclePanel_ItemBox _currentItemBox;

    private Vector2 _cursorDirection;

    [Header("Insert from Vehicle Prefab")]
    [SerializeField] private Vehicle_Controller _vehicleController;

    [Header("Fridge Target Data")]
    private bool _isTargetMode;

    private List<Fridge> _currentFridges = new();
    private Fridge _currentTargetFridge;

    private int _currentFridgeNum;

    // UnityEngine
    private void Awake()
    {
        Assign_All_BoxNum();

        // set cursor starting box
        _currentItemBox = _itemBoxes[0];
        _currentItemBox.BoxSelect_Toggle(true);
    }

    private void Start()
    {
        _itemBoxes[0].Assign_Item(_vehicleController.mainController.dataController.RawFood(1));
        _itemBoxes[0].Assign_Amount(23);
    }

    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        CursorDirection_Control(input);
        TargetDirection_Control(input.x);
    }

    private void OnSelect()
    {
        Export_CurrentItem_toFridge();
        TargetSystem_Toggle(true);
    }

    private void OnOption1()
    {
        Food_ScrObj apple = _vehicleController.mainController.dataController.RawFood(0);
        Add_Item(apple, 50);
    }

    private void OnOption2()
    {
        Food_ScrObj steak = _vehicleController.mainController.dataController.RawFood(2);
        Add_Item(steak, 50);
    }

    private void OnExit()
    {
        if (_isTargetMode == true)
        {
            TargetSystem_Toggle(false);
            return;
        }

        _vehicleController.VehiclePanel_Toggle(false);

        _vehicleController.Player_InputSystem_Toggle(true);
    }

    // Main Vehicle Panel Control
    private void Assign_All_BoxNum()
    {
        int numCount = 0;

        for (int i = 0; i < _itemBoxes.Count; i++)
        {
            _itemBoxes[i].boxNum = numCount;

            numCount++;
        }
    }

    private void CursorDirection_Control(Vector2 inputDireciton)
    {
        if (_isTargetMode == true) return;

        float calculatedDirection = inputDireciton.x + -(inputDireciton.y * 5);
        int convertedDirection = (int)calculatedDirection;

        int nextBoxNum = _currentItemBox.boxNum + convertedDirection;

        // less than min box num
        if (nextBoxNum < 0)
        {
            nextBoxNum = _itemBoxes.Count - 1;
        }

        // more than max box num
        if (nextBoxNum > _itemBoxes.Count - 1)
        {
            nextBoxNum = 0;
        }

        _currentItemBox.BoxSelect_Toggle(false);

        _currentItemBox = _itemBoxes[nextBoxNum];

        _currentItemBox.BoxSelect_Toggle(true);
    }

    private void Add_Item(Food_ScrObj food, int amount)
    {
        for (int i = 0; i < _itemBoxes.Count; i++)
        {
            if (_itemBoxes[i].hasItem == true)
            {
                if (food != _itemBoxes[i].currentFood) continue;

                _itemBoxes[i].Update_Amount(amount);

                return;
            }

            _itemBoxes[i].Assign_Item(food);
            _itemBoxes[i].Assign_Amount(amount);

            return;
        }
    }

    // Export Food to Fridge System Control
    private List<Fridge> CurrentFridges()
    {
        Main_Controller mainContrller = _vehicleController.mainController;
        List<GameObject> currentStations = mainContrller.currentStations;

        List<Fridge> currentFridges = new();

        for (int i = 0; i < currentStations.Count; i++)
        {
            if (currentStations[i].TryGetComponent(out Fridge fridge) == false) continue;

            currentFridges.Add(fridge);
        }

        Transform playerTransForm = _vehicleController.detection.player.gameObject.transform;

        // sort closest to farthest
        currentFridges.Sort((closestFridge, farthestFridge) =>
        Vector2.Distance(closestFridge.transform.position, playerTransForm.position)
        .CompareTo(Vector2.Distance(farthestFridge.transform.position, playerTransForm.position)));

        return currentFridges;
    }

    private void TargetSystem_Toggle(bool toggleOn)
    {
        _currentFridges = CurrentFridges();

        if (toggleOn == true && _isTargetMode == false)
        {
            _isTargetMode = true;
            _currentFridgeNum = 0;

            _currentTargetFridge = _currentFridges[_currentFridgeNum];
            _currentTargetFridge.TargetIndicator_Toggle(true);

            return;
        }

        _isTargetMode = false;

        _currentTargetFridge.TargetIndicator_Toggle(false);
        _currentTargetFridge = null;
    }

    private void TargetDirection_Control(float xInputDirection)
    {
        if (_isTargetMode == false) return;

        _currentFridges[_currentFridgeNum].TargetIndicator_Toggle(false);

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

        _currentFridgeNum = nextFridgeNum;

        _currentTargetFridge = _currentFridges[_currentFridgeNum];
        _currentTargetFridge.TargetIndicator_Toggle(true);
    }

    private void Export_CurrentItem_toFridge()
    {
        if (_isTargetMode == false) return;

        Food_ScrObj prevFood = _currentItemBox.currentFood;
        int prevAmount = _currentItemBox.currentAmount;

        FoodData_Controller fridgeIcon = _currentTargetFridge.FoodIcon();
        FoodData fridgeData = fridgeIcon.currentFoodData;

        // fridge > current item box
        _currentItemBox.Assign_Item(fridgeData.foodScrObj);
        _currentItemBox.Assign_Amount(fridgeData.currentAmount);

        // current item box > fridge
        fridgeIcon.Assign_Food(prevFood);
        fridgeIcon.Assign_Amount(prevAmount);
        fridgeIcon.FoodIcon_Transparency(false);
    }
}