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

    private List<Food_ScrObj> _ingredientUnlocks = new();

    [Header("")]
    [SerializeField] private GameObject _ingredientBubble;
    [SerializeField] private ItemSlot _ingredientIcon1;
    [SerializeField] private ItemSlot _ingredientIcon2;


    // UnityEngine
    private void OnEnable()
    {
        _controller.MenuOpen_Event += UpdateSlots_Data;
        _controller.MenuOpen_Event += UpdateNew_ArchivedFoods;
        _controller.MenuOpen_Event += UpdateSlots_Unlocks;
        _controller.MenuOpen_Event += Update_BookMarkFoods;

        _controller.AssignMain_ItemSlots(_slotsController.itemSlots);

        _controller.OnCursor_Input += IngredientBubble_UpdatePosition;
        _controller.OnSelect_Input += Select_Slot;

        _controller.OnOption1_Input += CurrentFood_BookmarkToggle;

    }

    private void OnDisable()
    {
        // empty current dragging food before menu close
        Drag_Cancel();

        _ingredientBubble.SetActive(false);

        _controller.MenuOpen_Event -= UpdateSlots_Data;
        _controller.MenuOpen_Event -= UpdateNew_ArchivedFoods;
        _controller.MenuOpen_Event -= UpdateSlots_Unlocks;
        _controller.MenuOpen_Event -= Update_BookMarkFoods;

        _controller.OnCursor_Input -= IngredientBubble_UpdatePosition;
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
        // slots
        List<ItemSlot> currentSlots = _slotsController.itemSlots;
        List<ItemSlot_Data> saveSlots = new();

        for (int i = 0; i < currentSlots.Count; i++)
        {
            saveSlots.Add(currentSlots[i].data);
        }

        ES3.Save("ArchiveMenu_Controller/_itemSlotDatas", saveSlots);

        // ingredient unlcoks
        List<int> foodIDs = new();

        foreach (var foods in _ingredientUnlocks)
        {
            foodIDs.Add(foods.id);
        }

        ES3.Save("ArchiveMenu_Controller/_ingredientUnlocks", foodIDs);

        // bookmark unlocks
        foodIDs.Clear();
    }

    public void Load_Data()
    {
        // slots
        List<ItemSlot> currentSlots = _slotsController.itemSlots;
        List<ItemSlot_Data> loadSlots = ES3.Load("ArchiveMenu_Controller/_itemSlotDatas", new List<ItemSlot_Data>());

        // ingredient unlcoks
        List<int> ingredientIDs = new();
        ingredientIDs = ES3.Load("ArchiveMenu_Controller/_ingredientUnlocks", ingredientIDs);

        // bookmark unlcoks
        List<int> bookmarkIDs = new();
        bookmarkIDs = ES3.Load("ArchiveMenu_Controller/_bookmarkUnlocks", bookmarkIDs);

        // load
        _slotsController.Add_Slot(15);

        for (int i = 0; i < loadSlots.Count; i++)
        {
            currentSlots[i].Assign_Data(loadSlots[i]);

            if (currentSlots[i].data.hasItem == false) continue;

            Food_ScrObj targetFood = currentSlots[i].data.currentFood;
            int targetID = targetFood.id;

            if (ingredientIDs.Contains(targetID))
            {
                _ingredientUnlocks.Add(targetFood);
            }
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

        ItemSlot_Data slotData = new(currentSlot.data);
        currentSlot.Empty_ItemBox();

        cursor.Assign_Item(slotData.currentFood);
        cursor.Assign_Data(slotData);

        IngredientBubble_Toggle(true);
    }

    private void Drag_Cancel()
    {
        ItemSlot_Cursor cursor = _controller.cursor;

        if (cursor.data.hasItem == false) return;

        ItemSlot_Data dragData = new(cursor.data);
        cursor.Empty_Item();

        AddFood(dragData.currentFood);

        IngredientBubble_Toggle(false);
    }

    //
    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.data);

        cursor.Empty_Item();

        currentSlot.Assign_Item(cursorData.currentFood);
        currentSlot.Assign_Data(cursorData);

        currentSlot.Toggle_BookMark(currentSlot.data.bookMarked);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);

        IngredientBubble_Toggle(false);
        UpdateSlot_Unlcoks(currentSlot);
    }

    //
    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.data);
        ItemSlot_Data slotData = new(currentSlot.data);

        currentSlot.Assign_Item(cursorData.currentFood);
        cursor.Assign_Item(slotData.currentFood);

        UpdateSlot_Unlcoks(currentSlot);
        IngredientBubble_Toggle(true);
    }


    // Slots Control
    /// <summary>
    /// Render sprites or amounts according to slot's current loaded data
    /// </summary>
    private void UpdateSlots_Data()
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            currentSlots[i].Assign_Item(currentSlots[i].data.currentFood);
        }
    }


    /// <summary>
    /// Single slot
    /// </summary>
    private void UpdateSlot_Unlcoks(ItemSlot currentSlot)
    {
        if (currentSlot.data.hasItem == false) return;

        Food_ScrObj slotFood = currentSlot.data.currentFood;

        currentSlot.Toggle_Icons(!currentSlot.data.isLocked, Ingredient_Unlocked(slotFood));
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);
    }

    /// <summary>
    /// All slots
    /// </summary>
    private void UpdateSlots_Unlocks()
    {
        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            UpdateSlot_Unlcoks(currentSlots[i]);
        }
    }


    // BookMark Control
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

    public void UnLock_BookMark(Food_ScrObj unlockFood)
    {
        Data_Controller data = _controller.vehicleController.mainController.dataController;
        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem == false) continue;
            if (currentSlots[i].data.currentFood != unlockFood) continue;

            // lock bookmark if it is not cooked food
            if (data.CookedFood(unlockFood) == null)
            {
                currentSlots[i].Toggle_Lock(true);
                return;
            }

            currentSlots[i].Toggle_Lock(false);
            return;
        }
    }


    private void CurrentFood_BookmarkToggle()
    {
        //
        ItemSlot_Cursor cursor = _controller.cursor;
        ItemSlot_Data cursorData = cursor.data;

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


    // Slots Update
    private void UpdateNew_ArchivedFoods()
    {
        Main_Controller mainController = _controller.vehicleController.mainController;
        List<Food_ScrObj> archivedFoods = mainController.archiveFoods;

        for (int i = 0; i < archivedFoods.Count; i++)
        {
            if (Food_InMenu(archivedFoods[i])) continue;

            AddFood(archivedFoods[i]);
        }
    }

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

    public ItemSlot AddFood(Food_ScrObj food)
    {
        // check if non duplicate food
        if (Food_InMenu(food)) return null;

        // check if food has ingredients
        if (food.ingredients.Count <= 0) return null;

        List<ItemSlot> currentSlots = _slotsController.itemSlots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (currentSlots[i].data.hasItem) continue;

            return currentSlots[i].Assign_Item(food);
        }

        return null;
    }


    // Food Ingredient Control
    public bool Ingredient_Unlocked(Food_ScrObj checkFood)
    {
        return _ingredientUnlocks.Contains(checkFood);
    }

    public void UnLock_Ingredient(Food_ScrObj unlockFood)
    {
        if (_ingredientUnlocks.Contains(unlockFood)) return;

        _ingredientUnlocks.Add(unlockFood);
    }


    // Ingredient Bubble Control
    private void IngredientBubble_Toggle(bool toggleOn)
    {
        ItemSlot_Data cursorData = _controller.cursor.data;

        if (toggleOn == false || cursorData.hasItem == false || Ingredient_Unlocked(cursorData.currentFood) == false)
        {
            _ingredientBubble.SetActive(false);
            return;
        }

        IngredientBubble_UpdatePosition();
        _ingredientBubble.SetActive(true);

        // update bubble
        Food_ScrObj currentFood = cursorData.currentFood;

        Food_ScrObj ingredient1 = currentFood.ingredients[0].foodScrObj;
        _ingredientIcon1.Assign_Data(new());
        _ingredientIcon1.Assign_Item(ingredient1);
        _ingredientIcon1.Assign_State(currentFood.ingredients[0].conditionDatas);

        Food_ScrObj ingredient2 = currentFood.ingredients[1].foodScrObj;
        _ingredientIcon2.Assign_Data(new());
        _ingredientIcon2.Assign_Item(ingredient2);
        _ingredientIcon2.Assign_State(currentFood.ingredients[1].conditionDatas);
    }

    private void IngredientBubble_UpdatePosition()
    {
        ItemSlot_Cursor cursor = _controller.cursor;

        Vector2 itemBoxPos = new(cursor.transform.position.x, cursor.transform.position.y + 0.65f);
        _ingredientBubble.transform.position = itemBoxPos;
    }
}