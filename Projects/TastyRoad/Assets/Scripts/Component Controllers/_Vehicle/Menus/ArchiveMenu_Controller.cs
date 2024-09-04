using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;
    public VehicleMenu_Controller controller => _controller;

    private Dictionary<int, List<ItemSlot_Data>> _currentDatas = new();
    public Dictionary<int, List<ItemSlot_Data>> currentDatas => _currentDatas;

    private int _currentPageNum;
    public int currentPageNum;

    private List<Food_ScrObj> _ingredientUnlocks = new();


    // UnityEngine
    private void OnEnable()
    {
        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);

        // subscriptions
        _controller.OnCursor_Outer += CurrentSlots_PageUpdate;

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnOption1_Input += CurrentFood_BookmarkToggle;

    }

    private void OnDisable()
    {
        // save current showing slots contents to _currentDatas
        Drag_Cancel();
        _currentDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();

        // subscriptions
        _controller.OnCursor_Outer -= CurrentSlots_PageUpdate;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnOption1_Input -= CurrentFood_BookmarkToggle;

    }

    private void OnDestroy()
    {
        OnDisable();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("ArchiveMenu_Controller/_currentDatas", _currentDatas);
        ES3.Save("ArchiveMenu_Controller/_ingredientUnlocks", _ingredientUnlocks);
    }

    public void Load_Data()
    {
        // load saved slot datas
        if (ES3.KeyExists("ArchiveMenu_Controller/_currentDatas"))
        {
            _currentDatas = ES3.Load("ArchiveMenu_Controller/_currentDatas", _currentDatas);
            _ingredientUnlocks = ES3.Load("ArchiveMenu_Controller/_ingredientUnlocks", _ingredientUnlocks);
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
    }


    private void Drag_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data slotData = new(currentSlot.data);
        currentSlot.Empty_ItemBox();

        cursor.Assign_Data(slotData);
        cursor.Assign_Item(slotData.currentFood);
    }

    private void Drag_Cancel()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.Current_Data().hasItem == false) return;

        ItemSlot_Data cursorData = new(cursor.Current_Data());
        cursor.Empty_Item();

        ItemSlot returnSlot = _controller.slotsController.EmptySlot();
        returnSlot.Assign_Data(cursorData);
        returnSlot.Assign_Item();
    }


    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.Current_Data());
        cursor.Empty_Item();

        currentSlot.Assign_Data(cursorData);
        currentSlot.Assign_Item(cursorData.currentFood);

        currentSlot.Toggle_BookMark(currentSlot.data.bookMarked);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);
    }

    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data slotData = new(cursor.currentSlot.data);

        Drop_Food();

        cursor.Assign_Data(slotData);
        cursor.Assign_Item(slotData.currentFood);
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

        if (currentSlot.data.isLocked == true) return;

        // toggle
        currentSlot.Toggle_BookMark(!currentSlot.data.bookMarked);

        // main data update
        Main_Controller main = _controller.vehicleController.mainController;

        if (currentSlot.data.bookMarked == true)
        {
            main.AddFood_toBookmark(currentSlot.data.currentFood);
            return;
        }

        main.RemoveFood_fromBookmark(currentSlot.data.currentFood);
    }


    // Data Control
    public bool Food_Archived(Food_ScrObj food)
    {
        return _controller.slotsController.FoodAmount(_currentDatas[currentPageNum], food) > 0;
    }

    public ItemSlot_Data Archive_Food(Food_ScrObj food)
    {
        // check if non duplicate food
        if (Food_Archived(food)) return null;

        // check if food has ingredients
        if (food.ingredients.Count <= 0) return null;

        List<ItemSlot_Data> data = _currentDatas[_currentPageNum];

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].hasItem == true) continue;
            data[i] = new(food, 1);

            // lock toggle according to cooked food
            Data_Controller dataController = _controller.vehicleController.mainController.dataController;
            data[i].isLocked = !dataController.CookedFood(food);

            return data[i];
        }

        return null;
    }


    public void Unlock_FoodIngredient(Food_ScrObj food)
    {
        if (_ingredientUnlocks.Contains(food) == true) return;
        _ingredientUnlocks.Add(food);
    }
}