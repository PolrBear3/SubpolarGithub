using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;
    public VehicleMenu_Controller controller => _controller;

    private Dictionary<int, List<ItemSlot_Data>> _currentDatas = new();
    public Dictionary<int, List<ItemSlot_Data>> currentDatas => _currentDatas;

    private int _currentPageNum;
    public int currentPageNum => _currentPageNum;

    [Header("")]
    [SerializeField] private Station_ScrObj _foodBox;
    [SerializeField] private Transform[] _exportIndicators;


    // UnityEngine
    private void OnEnable()
    {
        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        _controller.Update_PageDots(_currentDatas.Count, _currentPageNum);

        // subscriptions
        _controller.OnCursor_Outer += CurrentSlots_PageUpdate;

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += Export_Food;

        _controller.OnOption1_Input += CurrentFood_BookmarkToggle;
        _controller.OnOption1_Input += DropSingle_Food;
        _controller.OnOption2_Input += DragSingle_Food;

        Toggle_ExportIndicators(true);
    }

    private void OnDisable()
    {
        // save current showing slots contents to _currentDatas
        _currentDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();
        Drag_Cancel();

        // subscriptions
        _controller.OnCursor_Outer -= CurrentSlots_PageUpdate;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= Export_Food;

        _controller.OnOption1_Input -= CurrentFood_BookmarkToggle;
        _controller.OnOption1_Input -= DropSingle_Food;
        _controller.OnOption2_Input -= DragSingle_Food;

        Toggle_ExportIndicators(false);
    }

    private void OnDestroy()
    {
        OnDisable();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("FoodMenu_Controller/_currentDatas", _currentDatas);
    }

    public void Load_Data()
    {
        // load saved slot datas
        if (ES3.KeyExists("FoodMenu_Controller/_currentDatas"))
        {
            _currentDatas = ES3.Load("FoodMenu_Controller/_currentDatas", _currentDatas);
            return;
        }

        // set new slot datas
        _controller.slotsController.AddNewPage_ItemSlotDatas(_currentDatas);
    }


    // IVehicleMenu
    public bool MenuInteraction_Active()
    {
        return false;
    }


    // Menu Control
    public int Add_FoodItem(Food_ScrObj food, int amount)
    {
        if (amount <= 0) return 0;

        int slotCapacity = _controller.slotsController.singleSlotCapacity;

        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (currentDatas[i][j].hasItem == true && currentDatas[i][j].currentFood != food) continue;
                if (currentDatas[i][j].currentAmount >= slotCapacity) continue;

                int calculatedAmount = currentDatas[i][j].currentAmount + amount;
                int leftOver = calculatedAmount - slotCapacity;

                currentDatas[i][j] = new(food, calculatedAmount);

                // check if there is leftover
                if (leftOver <= 0) return 0;
                currentDatas[i][j].currentAmount = slotCapacity;

                // no slots available
                if (i == currentDatas.Count - 1) return leftOver;

                // add to next available slot
                return Add_FoodItem(food, leftOver);
            }
        }

        return amount;
    }

    public void Remove_FoodItem(Food_ScrObj food, int amount)
    {
        List<ItemSlot_Data> currentDatas = _currentDatas[_currentPageNum];
        int removeAmount = amount;

        for (int i = 0; i < currentDatas.Count; i++)
        {
            if (currentDatas[i].hasItem == false) continue;
            if (currentDatas[i].currentFood != food) continue;

            if (currentDatas[i].currentAmount <= removeAmount)
            {
                removeAmount -= currentDatas[i].currentAmount;
                currentDatas[i] = new();
                continue;
            }

            currentDatas[i].currentAmount -= removeAmount;
            return;
        }
    }


    // Slot and Cursor Control
    private void Select_Slot()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.Current_Data().hasItem == false)
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


    private void Drag_Food()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        currentSlot.Toggle_BookMark(false);
        ItemSlot_Data slotData = new(currentSlot.data);

        cursor.Assign_Item(slotData.currentFood);
        cursor.Assign_Amount(1);

        currentSlot.Update_Amount(-1);
    }

    private void DragSingle_Food()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        if (cursor.Current_Data().hasItem == false) return;

        ItemSlot currentSlot = cursor.currentSlot;

        if (currentSlot.data.hasItem == false) return;
        if (cursor.Current_Data().currentFood != currentSlot.data.currentFood) return;
        if (cursor.Current_Data().currentAmount >= slotsController.singleSlotCapacity) return;

        cursor.Assign_Amount(cursor.Current_Data().currentAmount + 1);
        currentSlot.Update_Amount(-1);
    }

    private void Drag_Cancel()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.Current_Data().hasItem == false) return;

        ItemSlot_Data cursorData = new(cursor.Current_Data());
        cursor.Empty_Item();

        Add_FoodItem(cursorData.currentFood, cursorData.currentAmount);
    }


    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = new(cursor.Current_Data());

        ItemSlot currentSlot = cursor.currentSlot;

        currentSlot.Assign_Item(cursorData.currentFood);
        currentSlot.Assign_Amount(cursorData.currentAmount);

        cursor.Empty_Item();
    }

    private void DropSingle_Food()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        if (cursor.Current_Data().hasItem == false) return;

        ItemSlot currentSlot = cursor.currentSlot;

        if (currentSlot.data.hasItem == false) return;
        if (cursor.Current_Data().currentFood != currentSlot.data.currentFood) return;
        if (currentSlot.data.currentAmount >= slotsController.singleSlotCapacity) return;

        cursor.Assign_Amount(cursor.Current_Data().currentAmount - 1);
        currentSlot.Update_Amount(1);
    }


    private void Swap_Food()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        // cursor max drag amount
        int maxCapacity = slotsController.singleSlotCapacity;
        if (cursor.Current_Data().currentAmount >= maxCapacity) return;

        currentSlot.Toggle_BookMark(false);

        // same food combine drag
        if (cursor.Current_Data().currentFood == currentSlot.data.currentFood)
        {
            int calculatedAmount = cursor.Current_Data().currentAmount + currentSlot.data.currentAmount;

            if (calculatedAmount > maxCapacity)
            {
                cursor.Assign_Amount(maxCapacity);
                currentSlot.Assign_Amount(calculatedAmount - maxCapacity);
                return;
            }

            cursor.Assign_Amount(cursor.Current_Data().currentAmount + currentSlot.data.currentAmount);
            currentSlot.Empty_ItemBox();
            return;
        }

        // different food
        Food_ScrObj cursorFood = cursor.Current_Data().currentFood;
        int cursorAmount = cursor.Current_Data().currentAmount;

        cursor.Assign_Item(currentSlot.data.currentFood);
        cursor.Assign_Amount(currentSlot.data.currentAmount);

        currentSlot.Assign_Item(cursorFood);
        currentSlot.Assign_Amount(cursorAmount);
    }


    private void CurrentFood_BookmarkToggle()
    {
        //
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.Current_Data();

        // check if cursor has item
        if (cursorData.hasItem == false) return;

        //
        ItemSlot currentSlot = cursor.currentSlot;

        // check if current hover slot has no item
        if (currentSlot.data.hasItem) return;

        // drop current item
        Drop_Food();

        // toggle
        currentSlot.Toggle_BookMark(true);
    }


    // FoodBox Export System
    private void Toggle_ExportIndicators(bool toggleOn)
    {
        foreach (var indicator in _exportIndicators)
        {
            indicator.gameObject.SetActive(toggleOn);
        }
    }


    private List<Transform> Available_ExportPositions()
    {
        Main_Controller main = _controller.vehicleController.mainController;
        List<Transform> exportPositions = new();

        for (int i = 0; i < _exportIndicators.Length; i++)
        {
            if (main.Position_Claimed(_exportIndicators[i].position)) continue;
            exportPositions.Add(_exportIndicators[i]);
        }

        return exportPositions;
    }

    private Vector2 Available_ExportPosition()
    {
        foreach (var transform in Available_ExportPositions())
        {
            return transform.position;
        }
        return Vector2.zero;
    }


    private void Export_Food()
    {
        // if no food to export on cursor
        ItemSlot_Data currentCursorData = _controller.slotsController.cursor.Current_Data();
        if (currentCursorData.hasItem == false) return;

        // if there are enough space to spawn food box
        if (Available_ExportPositions().Count <= 0) return;

        // get vehicle 
        Vehicle_Controller vehicle = _controller.vehicleController;

        // spawn and track food box
        Station_Controller station = vehicle.mainController.Spawn_Station(_foodBox, Available_ExportPosition());
        vehicle.mainController.Claim_Position(station.transform.position);

        // assign exported food to food box
        FoodData cursorFood = new(currentCursorData.currentFood);
        station.Food_Icon().Set_CurrentData(cursorFood);

        // show food icon
        station.Food_Icon().Show_Icon();

        // assign exported food amount according to dragging amount
        if (currentCursorData.currentAmount >= 6)
        {
            // max amount
            station.Food_Icon().currentData.Set_Amount(6);
            _controller.slotsController.cursor.Assign_Amount(currentCursorData.currentAmount - 6);
        }
        else
        {
            // bellow max amount
            station.Food_Icon().currentData.Set_Amount(currentCursorData.currentAmount);
            _controller.slotsController.cursor.Empty_Item();
        }
    }
}