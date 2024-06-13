using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;

    [Header("")]
    [SerializeField] private ItemSlots_Controller _slotsController;
    public ItemSlots_Controller slotsController => _slotsController;

    private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;

    [Header("")]
    [SerializeField] private GameObject _ingredientBubble;
    [SerializeField] private ItemSlot _ingredientIcon1;
    [SerializeField] private ItemSlot _ingredientIcon2;



    // UnityEngine
    private void OnEnable()
    {
        _controller.MenuOpen_Event += Update_Slots_Data;
        _controller.MenuOpen_Event += UpdateNew_ArchivedFoods;
        _controller.MenuOpen_Event += Update_BookMarkFoods;

        _controller.AssignMain_ItemSlots(_slotsController.itemSlots);

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnCursor_Input += IngredientBubble_UpdatePosition;

        _controller.OnHoldSelect_Input += CurrentFood_BookmarkToggle;

    }

    private void OnDisable()
    {
        // empty current dragging food before menu close
        Drag_Cancel();

        _ingredientBubble.SetActive(false);

        _controller.MenuOpen_Event -= Update_Slots_Data;
        _controller.MenuOpen_Event -= UpdateNew_ArchivedFoods;
        _controller.MenuOpen_Event -= Update_BookMarkFoods;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnCursor_Input -= IngredientBubble_UpdatePosition;

        _controller.OnHoldSelect_Input -= CurrentFood_BookmarkToggle;
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

        ES3.Save("ArchiveMenu_Controller/_itemSlotDatas", saveSlots);
    }

    public void Load_Data()
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;
        List<ItemSlot_Data> loadSlots = ES3.Load("ArchiveMenu_Controller/_itemSlotDatas", new List<ItemSlot_Data>());

        _slotsController.Add_Slot(15);

        for (int i = 0; i < loadSlots.Count; i++)
        {
            currentSlots[i].data = loadSlots[i];
        }
    }


    // IVehicleMenu
    public bool MenuInteraction_Active()
    {
        return false;
    }


    // Slot and Cursor Control
    private void Select_Slot()
    {
        _ingredientBubble.SetActive(false);

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

    //
    private void Drag_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;
        Food_ScrObj slotFood = currentSlot.data.currentFood;

        cursor.Assign_Item(slotFood);
        cursor.data.bookMarked = currentSlot.data.bookMarked;

        currentSlot.Empty_ItemBox();

        IngredientBubble_Toggle(true);
    }

    private void Drag_Cancel()
    {
        ItemSlot_Cursor cursor = _controller.cursor;

        if (cursor.data.hasItem == false) return;

        AddFoodto_EmptySlot(cursor.data.currentFood);
        cursor.Empty_Item();

        IngredientBubble_Toggle(false);
    }

    //
    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        currentSlot.Assign_Item(cursor.data.currentFood);
        currentSlot.Toggle_BookMark(cursor.data.bookMarked);

        cursor.Empty_Item();

        IngredientBubble_Toggle(false);
    }

    //
    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        Food_ScrObj cursorFood = cursor.data.currentFood;
        bool cursorBookMarked = cursor.data.bookMarked;

        cursor.Assign_Item(currentSlot.data.currentFood);
        cursor.data.bookMarked = currentSlot.data.bookMarked;

        currentSlot.Assign_Item(cursorFood);
        currentSlot.Toggle_BookMark(cursorBookMarked);

        IngredientBubble_Toggle(true);
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
        }
    }

    // BookMark Toggle Update
    private void Update_BookMarkFoods()
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;
        Main_Controller main = _controller.vehicleController.mainController;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem == false) continue;
            if (!main.Is_BookmarkedFood(currentSlots[i].data.currentFood)) continue;

            currentSlots[i].Toggle_BookMark(true);
        }
    }

    // Archive Cooked Foods to Available Orders Export System
    private void CurrentFood_BookmarkToggle()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        if (cursor.data.hasItem == false) return;

        ItemSlot currentSlot = cursor.currentSlot;

        cursor.data.bookMarked = !cursor.data.bookMarked;

        currentSlot.Toggle_BookMark(currentSlot.data.bookMarked);

        Food_ScrObj currentFood = currentSlot.data.currentFood;
        Main_Controller main = _controller.vehicleController.mainController;

        if (currentSlot.data.bookMarked == false)
        {
            main.RemoveFood_fromBookmark(currentFood);
            return;
        }

        main.AddFood_toBookmark(currentFood);
    }


    // Slots Update
    private void UpdateNew_ArchivedFoods()
    {
        Main_Controller mainController = _controller.vehicleController.mainController;
        List<Food_ScrObj> archivedFoods = mainController.archiveFoods;

        for (int i = 0; i < archivedFoods.Count; i++)
        {
            if (Food_InMenu(archivedFoods[i])) continue;

            AddFoodto_EmptySlot(archivedFoods[i]);
        }
    }

    //
    private bool Food_InMenu(Food_ScrObj food)
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem == false) continue;
            if (currentSlots[i].data.currentFood != food) continue;
            return true;
        }

        return false;
    }

    //
    private void AddFoodto_EmptySlot(Food_ScrObj food)
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem) continue;

            currentSlots[i].Assign_Item(food);
            break;
        }
    }


    // Ingredient Bubble Control
    private void IngredientBubble_Toggle(bool toggleOn)
    {
        ItemSlot_Data cursorData = _controller.cursor.data;

        if (toggleOn == false || cursorData.hasItem == false)
        {
            _ingredientBubble.SetActive(false);
            return;
        }

        IngredientBubble_UpdatePosition();
        _ingredientBubble.SetActive(true);

        // update bubble
        Food_ScrObj currentFood = cursorData.currentFood;

        Food_ScrObj ingredient1 = currentFood.ingredients[0].foodScrObj;
        _ingredientIcon1.Assign_Item(ingredient1);
        _ingredientIcon1.Assign_State(currentFood.ingredients[0].conditionDatas);

        Food_ScrObj ingredient2 = currentFood.ingredients[1].foodScrObj;
        _ingredientIcon2.Assign_Item(ingredient2);
        _ingredientIcon2.Assign_State(currentFood.ingredients[1].conditionDatas);
    }

    //
    private void IngredientBubble_UpdatePosition()
    {
        ItemSlot_Cursor cursor = _controller.cursor;

        Vector2 itemBoxPos = new(cursor.transform.position.x, cursor.transform.position.y + 0.65f);
        _ingredientBubble.transform.position = itemBoxPos;
    }
}