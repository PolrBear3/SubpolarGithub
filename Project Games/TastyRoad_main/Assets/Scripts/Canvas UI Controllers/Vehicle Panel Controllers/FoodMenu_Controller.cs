using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FoodMenu_Controller : MonoBehaviour
{
    private VehiclePanel_Controller _controller;

    [SerializeField] private List<VechiclePanel_ItemBox> _itemBoxes = new();

    private List<Fridge> _currentFridges = new();
    private Fridge _currentTargetFridge;

    private bool _fridgeTargetMode;
    public bool fridgeTargetMode => _fridgeTargetMode;

    private int _currentFridgeNum;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out VehiclePanel_Controller controller)) { _controller = controller; }
    }

    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        Fridge_TargetDirection_Control(input.x);
    }

    private void OnSelect()
    {
        Export_FoodItem_toFridge();

        Fridge_TargetSystem_Toggle(true);
    }

    private void OnExit()
    {
        Fridge_TargetSystem_Toggle(false);
    }

    //
    public void Assign_ItemBoxes()
    {
        if (_controller.currentMenuNum != 0) return;

        _controller.itemBoxes = _itemBoxes;
        _controller.Assign_All_BoxNum();

        // set starting cursor at first box
        _controller.currentItemBox = _itemBoxes[0];
        _controller.currentItemBox.BoxSelect_Toggle(true);
    }

    // Food to Fridge Export System
    private List<Fridge> CurrentFridges()
    {
        Main_Controller mainContrller = _controller.vehicleController.mainController;
        List<GameObject> currentStations = mainContrller.currentStations;

        List<Fridge> currentFridges = new();

        for (int i = 0; i < currentStations.Count; i++)
        {
            if (currentStations[i].TryGetComponent(out Fridge fridge) == false) continue;

            currentFridges.Add(fridge);
        }

        Transform playerTransForm = _controller.vehicleController.detection.player.gameObject.transform;

        // sort closest to farthest
        currentFridges.Sort((closestFridge, farthestFridge) =>
        Vector2.Distance(closestFridge.transform.position, playerTransForm.position)
        .CompareTo(Vector2.Distance(farthestFridge.transform.position, playerTransForm.position)));

        return currentFridges;
    }

    private void Fridge_TargetSystem_Toggle(bool toggleOn)
    {
        if (_controller.currentMenuNum != 0) return;

        _currentFridges = CurrentFridges();

        if (toggleOn == true && _fridgeTargetMode == false)
        {
            _fridgeTargetMode = true;
            _currentFridgeNum = 0;

            _currentTargetFridge = _currentFridges[_currentFridgeNum];
            _currentTargetFridge.TargetIndicator_Toggle(true);

            return;
        }

        if (_fridgeTargetMode == false) return;

        _fridgeTargetMode = false;

        _currentTargetFridge.TargetIndicator_Toggle(false);
        _currentTargetFridge = null;
    }

    private void Fridge_TargetDirection_Control(float xInputDirection)
    {
        if (_fridgeTargetMode == false) return;

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

    public void Add_FoodItem(Food_ScrObj food, int amount)
    {
        List<VechiclePanel_ItemBox> itemBoxes = _controller.itemBoxes;

        for (int i = 0; i < itemBoxes.Count; i++)
        {
            if (itemBoxes[i].hasItem == true)
            {
                if (food != itemBoxes[i].currentFood) continue;

                itemBoxes[i].Update_Amount(amount);

                return;
            }

            itemBoxes[i].Assign_Item(food);
            itemBoxes[i].Assign_Amount(amount);

            return;
        }
    }

    private void Export_FoodItem_toFridge()
    {
        if (_fridgeTargetMode == false) return;

        Food_ScrObj prevFood = _controller.currentItemBox.currentFood;
        int prevAmount = _controller.currentItemBox.currentAmount;

        FoodData_Controller fridgeIcon = _currentTargetFridge.FoodIcon();
        FoodData fridgeData = fridgeIcon.currentFoodData;

        // fridge > current item box
        _controller.currentItemBox.Assign_Item(fridgeData.foodScrObj);
        _controller.currentItemBox.Assign_Amount(fridgeData.currentAmount);

        // current item box > fridge
        fridgeIcon.Assign_Food(prevFood);
        fridgeIcon.Assign_Amount(prevAmount);
        fridgeIcon.FoodIcon_Transparency(false);
    }
}
