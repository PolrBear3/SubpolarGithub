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
    private int _currentPageNum;

    private List<Food_ScrObj> _ingredientUnlocks = new();


    // UnityEngine
    private void OnEnable()
    {
        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);

        _controller.OnSelect_Input += Select_Slot;

        /*
        _controller.MenuOpen_Event += UpdateSlots_Data;
        _controller.MenuOpen_Event += UpdateNew_ArchivedFoods;
        _controller.MenuOpen_Event += UpdateSlots_Unlocks;
        _controller.MenuOpen_Event += Update_BookMarkFoods;

        _controller.AssignMain_ItemSlots(_slotsController.itemSlots);

        _controller.OnCursor_Input += IngredientBubble_UpdatePosition;

        _controller.OnOption1_Input += CurrentFood_BookmarkToggle;
        */
    }

    private void OnDisable()
    {
        // save current showing slots contents to _currentDatas
        Drag_Cancel();
        _currentDatas[_currentPageNum] = _controller.slotsController.Current_SlotDatas();

        _controller.OnSelect_Input -= Select_Slot;

        /*
        _controller.MenuOpen_Event -= UpdateSlots_Data;
        _controller.MenuOpen_Event -= UpdateNew_ArchivedFoods;
        _controller.MenuOpen_Event -= UpdateSlots_Unlocks;
        _controller.MenuOpen_Event -= Update_BookMarkFoods;

        _controller.OnCursor_Input -= IngredientBubble_UpdatePosition;

        _controller.OnOption1_Input -= CurrentFood_BookmarkToggle;
        */
    }

    private void OnDestroy()
    {
        OnDisable();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("ArchiveMenu_Controller/_currentDatas", _currentDatas);
        _currentDatas = ES3.Load("ArchiveMenu_Controller/_currentDatas", _currentDatas);
    }

    public void Load_Data()
    {
        // load saved slot datas
        if (ES3.KeyExists("ArchiveMenu_Controller/_currentDatas"))
        {
            _currentDatas = ES3.Load("ArchiveMenu_Controller/_currentDatas", _currentDatas);
            return;
        }

        // set new slot datas
        List<ItemSlot_Data> newDatas = new();
        for (int i = 0; i < _controller.slotsController.itemSlots.Count; i++)
        {
            newDatas.Add(new());
        }

        _currentDatas.Add(0, newDatas);
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

    //
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

    //
    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.Current_Data());

        cursor.Empty_Item();

        currentSlot.Assign_Data(cursorData);
        currentSlot.Assign_Item(cursorData.currentFood);
    }

    //
    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.Current_Data());
        ItemSlot_Data slotData = new(currentSlot.data);

        currentSlot.Assign_Data(cursorData);
        currentSlot.Assign_Item(cursorData.currentFood);

        cursor.Assign_Data(slotData);
        cursor.Assign_Item(slotData.currentFood);
    }


    // Data Control
    public bool Food_Archived(Food_ScrObj food)
    {
        List<ItemSlot_Data> data = _currentDatas[_currentPageNum];

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].hasItem == false) continue;
            if (food != data[i].currentFood) continue;
            return true;
        }

        return false;
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
            return data[i];
        }

        return null;
    }


    private void CurrentFood_BookmarkToggle()
    {
        //
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.Current_Data();

        // check if cursor is dragging item
        if (cursorData.hasItem == false) return;

        //
        ItemSlot currentSlot = cursor.currentSlot;

        // check if current hover slot has no item
        if (currentSlot.data.hasItem) return;

        //
        Drop_Food();

        // check if current food is bookmark unlocked
        if (currentSlot.data.isLocked == true) return;

        // toggle dropped food
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


    // Ingredient Data Control
    public bool Ingredient_Unlocked(Food_ScrObj checkFood)
    {
        return _ingredientUnlocks.Contains(checkFood);
    }

    public void UnLock_Ingredient(Food_ScrObj unlockFood)
    {
        if (_ingredientUnlocks.Contains(unlockFood)) return;

        _ingredientUnlocks.Add(unlockFood);
    }
}