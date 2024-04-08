using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [SerializeField] private VehicleMenu_Controller _controller;

    [Header("")]
    [SerializeField] private Vector2 _gridData;
    [SerializeField] private List<ItemSlot> _itemSlots = new();
    [SerializeField] private int _slotCapacity;

    [Header("Food Box Export")]
    [SerializeField] private Vector2 _exportRange;


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
        Drag_Cancel();

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= Export_Food;

        _controller.OnOption1_Input -= DropSingle_Food;
        _controller.OnOption2_Input -= DragSingle_Food;
    }



    // ISaveLoadable
    public void Save_Data()
    {
        List<ItemSlot_Data> saveSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            saveSlots.Add(_itemSlots[i].data);
        }

        ES3.Save("foodMenuSlots", saveSlots);
    }

    public void Load_Data()
    {
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



    // Menu Control
    public int Add_FoodItem(Food_ScrObj food, int amount)
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == true && _itemSlots[i].data.currentFood != food) continue;
            if (_itemSlots[i].data.currentAmount >= _slotCapacity) continue;

            int calculatedAmount = _itemSlots[i].data.currentAmount + amount;
            int leftOver = calculatedAmount - _slotCapacity;

            _itemSlots[i].Assign_Item(food);

            if (leftOver <= 0)
            {
                _itemSlots[i].Update_Amount(amount);
                return 0;
            }

            _itemSlots[i].Assign_Amount(_slotCapacity);

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

    private void Drag_Cancel()
    {
        ItemSlot_Data cursorData = _controller.cursor.data;

        if (cursorData.hasItem == false) return;

        Add_FoodItem(cursorData.currentFood, cursorData.currentAmount);
        _controller.cursor.Empty_Item();
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
        // if no food to export on cursor
        ItemSlot_Data currentCursorData = _controller.cursor.data;
        if (currentCursorData.hasItem == false) return;

        //
        Vehicle_Controller vehicle = _controller.vehicleController;

        // spawn food box
        Station_Controller station = vehicle.mainController.Spawn_Station(7, FoodExport_Position());
        vehicle.mainController.Claim_Position(station.transform.position);

        // assign exported food to food box
        station.Food_Icon().Assign_Food(currentCursorData.currentFood);

        // assign exported food amount according to dragging amount
        if (currentCursorData.currentAmount >= 6)
        {
            station.Food_Icon().Assign_Amount(6);
            _controller.cursor.Assign_Amount(currentCursorData.currentAmount - 6);
        }
        else
        {
            station.Food_Icon().Assign_Amount(currentCursorData.currentAmount);
            _controller.cursor.Empty_Item();
        }
    }

    private bool FooedExport_PositionAvailable()
    {
        Main_Controller main = _controller.vehicleController.mainController;
        Transform vehiclePoint = _controller.vehicleController.transform;

        int loopAmount = (int)_exportRange.y - (int)_exportRange.x - 1;
        float currentX = _exportRange.x;

        for (int i = 0; i < loopAmount; i++)
        {
            if (main.Position_Claimed(new Vector2(vehiclePoint.position.x - currentX, vehiclePoint.position.y - 1))) continue;
            return true;
        }

        return false;
    }

    private Vector2 FoodExport_Position()
    {
        Main_Controller main = _controller.vehicleController.mainController;

        Vector2 vehiclePos = _controller.vehicleController.transform.position;

        int loopAmount = (int)_exportRange.y - (int)_exportRange.x - 1;
        float currentX = vehiclePos.x - _exportRange.x;

        for (int i = 0; i < loopAmount; i++)
        {
            Debug.Log(currentX);

            if (main.Position_Claimed(new Vector2(currentX, vehiclePos.y - 1)))
            {
                currentX++;
                continue;
            }

            return new Vector2(currentX, vehiclePos.y - 1);
        }

        return Vector2.zero;
    }
}