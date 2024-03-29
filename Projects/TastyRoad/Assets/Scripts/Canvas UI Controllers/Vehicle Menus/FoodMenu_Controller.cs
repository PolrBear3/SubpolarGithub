using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [SerializeField] private VehicleMenu_Controller _controller;

    [Header("")]
    [SerializeField] private Vector2 _gridData;
    [SerializeField] private List<ItemSlot> _itemSlots = new();
    [SerializeField] private int _boxCapacity;



    // UnityEngine
    private void Start()
    {
        Set_Slots_GridNum();
        Update_Slots();
    }

    private void OnEnable()
    {
        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += Export_Food;

        _controller.OnOption1_Input += DropSingle_Food;
        _controller.OnOption2_Input += DragSingle_Food;
    }

    private void OnDisable()
    {
        // save current dragging item before menu close
        ItemSlot_Data cursorData = _controller.cursor.data;

        if (cursorData.hasItem)
        {
            Add_FoodItem(cursorData.currentFood, cursorData.currentAmount);
            _controller.cursor.Empty_Item();
        }

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= Export_Food;

        _controller.OnOption1_Input -= DropSingle_Food;
        _controller.OnOption2_Input -= DragSingle_Food;
    }



    // ISaveLoadable
    public void Save_Data()
    {
        // save current dragging item before game close
        ItemSlot_Data cursorData = _controller.cursor.data;

        if (cursorData.hasItem)
        {
            Add_FoodItem(cursorData.currentFood, cursorData.currentAmount);
        }

        List<ItemSlot_Data> saveSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            saveSlots.Add(_itemSlots[i].data);
        }

        ES3.Save("foodMenuSlots", saveSlots);
    }

    public void Load_Data()
    {
        if (!ES3.KeyExists("foodMenuSlots")) return;

        List<ItemSlot_Data> loadSlots = ES3.Load("foodMenuSlots", new List<ItemSlot_Data>());

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
        return false;
    }



    // All Start Functions are Here
    private void Set_Slots_GridNum()
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
            _itemSlots[i].Assign_Item(_itemSlots[i].data.currentFood);
            _itemSlots[i].Assign_Amount(_itemSlots[i].data.currentAmount);
        }
    }

    /*
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

        Food_ScrObj prevFood = _controller.currentItemBox.data.currentFood;
        int prevAmount = _controller.currentItemBox.data.currentAmount;

        FoodData_Controller fridgeIcon = _currentTargetFridge.foodIcon;
        FoodData fridgeData = fridgeIcon.currentFoodData;

        ItemSlot box = _controller.currentItemBox;

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
    */



    // Menu Control
    public int Add_FoodItem(Food_ScrObj food, int amount)
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == true && _itemSlots[i].data.currentFood != food) continue;
            if (_itemSlots[i].data.currentAmount >= _boxCapacity) continue;

            int calculatedAmount = _itemSlots[i].data.currentAmount + amount;
            int leftOver = calculatedAmount - _boxCapacity;

            _itemSlots[i].Assign_Item(food);

            if (leftOver <= 0)
            {
                _itemSlots[i].Update_Amount(amount);
                return 0;
            }

            _itemSlots[i].Assign_Amount(_boxCapacity);

            if (i == _itemSlots.Count - 1) return leftOver;

            return Add_FoodItem(food, leftOver);
        }

        return amount;
    }



    // Slot and Cursor Control
    private void Select_Slot()
    {
        ItemSlot_Cursor cursor = _controller.cursor;

        if (cursor.data.hasItem == false)
        {
            Drag_Food();
            return;
        }

        if (cursor.currentSlot.data.hasItem)
        {
            Swap_Food();
            return;
        }

        Drop_Food();
    }

    // Drag
    private void Drag_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;
        Food_ScrObj slotFood = currentSlot.data.currentFood;

        cursor.Assign_Item(slotFood);
        cursor.Assign_Amount(1);

        currentSlot.Update_Amount(-1);
    }

    private void DragSingle_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        if (cursor.data.hasItem == false) return;

        ItemSlot currentSlot = cursor.currentSlot;
        if (currentSlot.data.hasItem == false) return;

        if (cursor.data.currentFood != currentSlot.data.currentFood) return;

        cursor.Assign_Amount(cursor.data.currentAmount + 1);
        currentSlot.Update_Amount(-1);
    }

    // Drop
    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        currentSlot.Assign_Item(cursor.data.currentFood);
        currentSlot.Assign_Amount(cursor.data.currentAmount);

        cursor.Empty_Item();
    }

    private void DropSingle_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        if (cursor.data.hasItem == false) return;

        ItemSlot currentSlot = cursor.currentSlot;
        if (currentSlot.data.hasItem == false) return;
        if (cursor.data.currentFood != currentSlot.data.currentFood) return;

        cursor.Assign_Amount(cursor.data.currentAmount - 1);
        currentSlot.Update_Amount(1);
    }

    // Swap
    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        // same food
        if (cursor.data.currentFood == currentSlot.data.currentFood)
        {
            cursor.Assign_Amount(cursor.data.currentAmount + currentSlot.data.currentAmount);
            currentSlot.Empty_ItemBox();

            return;
        }

        // different food
        Food_ScrObj cursorFood = cursor.data.currentFood;
        int cursorAmount = cursor.data.currentAmount;

        cursor.Assign_Item(currentSlot.data.currentFood);
        cursor.Assign_Amount(currentSlot.data.currentAmount);

        currentSlot.Assign_Item(cursorFood);
        currentSlot.Assign_Amount(cursorAmount);
    }



    // FoodBox Export System
    private void Export_Food()
    {
        ItemSlot_Data currentCursorData = _controller.cursor.currentSlot.data;

        // spawn food box, nearest to vehicle

        // assign food and amount to food box

        // clear hold data
    }
}