using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;

    [Header("")]
    [SerializeField] private ItemSlots_Controller _slotsController;
    public ItemSlots_Controller slotsController => _slotsController;

    [Header("Food Box Export")]
    [SerializeField] private GameObject _exportIndicators; 
    [SerializeField] private Vector2 _exportRange;



    // UnityEngine
    private void OnEnable()
    {
        _controller.MenuOpen_Event += Update_Slots_Data;
        _controller.AssignMain_ItemSlots(_slotsController.itemSlots);

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += Export_Food;

        _controller.OnOption1_Input += DropSingle_Food;
        _controller.OnOption2_Input += DragSingle_Food;

        _exportIndicators.SetActive(true);
    }

    private void OnDisable()
    {
        // save current dragging item before menu close
        Drag_Cancel();

        _controller.MenuOpen_Event -= Update_Slots_Data;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= Export_Food;

        _controller.OnOption1_Input -= DropSingle_Food;
        _controller.OnOption2_Input -= DragSingle_Food;

        _exportIndicators.SetActive(false);
    }

    private void OnDestroy()
    {
        OnDisable();
    }



    // ISaveLoadable
    public void Save_Data()
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;
        List<ItemSlot_Data> saveSlots = new();

        for (int i = 0; i < currentSlots.Count; i++)
        {
            saveSlots.Add(currentSlots[i].data);
        }

        ES3.Save("FoodMenu_Controller/_itemSlotDatas", saveSlots);
    }

    public void Load_Data()
    {
        List<ItemSlot_Data> loadSlots = ES3.Load("FoodMenu_Controller/_itemSlotDatas", new List<ItemSlot_Data>());

        _slotsController.Add_Slot(loadSlots.Count);

        for (int i = 0; i < loadSlots.Count; i++)
        {
            _slotsController.itemSlots[i].data = loadSlots[i];
        }

        // default slots amount
        if (ES3.KeyExists("FoodMenu_Controller/_itemSlotDatas")) return;

        _slotsController.Add_Slot(5);
    }



    // IVehicleMenu
    public bool MenuInteraction_Active()
    {
        return false;
    }


    /// <summary>
    /// Render sprites or amounts according to slot's current loaded data
    /// </summary>
    private void Update_Slots_Data()
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            currentSlots[i].Assign_Item(currentSlots[i].data.currentFood);
            currentSlots[i].Assign_Amount(currentSlots[i].data.currentAmount);
        }
    }



    // Menu Control
    public int Add_FoodItem(Food_ScrObj food, int amount)
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;
        int slotCapacity = _slotsController.singleSlotCapacity;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem == true && currentSlots[i].data.currentFood != food) continue;
            if (currentSlots[i].data.currentAmount >= slotCapacity) continue;

            int calculatedAmount = currentSlots[i].data.currentAmount + amount;
            int leftOver = calculatedAmount - slotCapacity;

            currentSlots[i].Assign_Item(food);

            if (leftOver <= 0)
            {
                currentSlots[i].Update_Amount(amount);
                return 0;
            }

            currentSlots[i].Assign_Amount(slotCapacity);

            if (i == currentSlots.Count - 1) return leftOver;

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
        /*
        // if no food to export on cursor
        ItemSlot_Data currentCursorData = _controller.cursor.data;
        if (currentCursorData.hasItem == false) return;

        // if there are enough space to spawn food box
        if (FoodExport_PositionAvailable() == false) return;

        //
        Vehicle_Controller vehicle = _controller.vehicleController;

        // spawn food box
        Station_ScrObj foodbox = vehicle.mainController.dataController.Station_ScrObj("Food Box");
        Station_Controller station = vehicle.mainController.Spawn_Station(foodbox, FoodExport_Position());
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
        */
    }

    private bool FoodExport_PositionAvailable()
    {
        Main_Controller main = _controller.vehicleController.mainController;
        Vector2 vehiclePos = _controller.vehicleController.transform.position;

        int loopAmount = (int)_exportRange.y - (int)_exportRange.x + 1;
        float currentX = vehiclePos.x + _exportRange.x;

        for (int i = 0; i < loopAmount; i++)
        {
            if (main.Position_Claimed(new Vector2(currentX, vehiclePos.y - 1)))
            {
                currentX++;
                continue;
            }

            return true;
        }

        return false;
    }

    private Vector2 FoodExport_Position()
    {
        Main_Controller main = _controller.vehicleController.mainController;
        Vector2 vehiclePos = _controller.vehicleController.transform.position;

        int loopAmount = (int)_exportRange.y - (int)_exportRange.x + 1;
        float currentX = vehiclePos.x + _exportRange.x;

        for (int i = 0; i < loopAmount; i++)
        {
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