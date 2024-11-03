using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;
    public VehicleMenu_Controller controller => _controller;

    private Dictionary<int, List<ItemSlot_Data>> _currentDatas = new();
    public Dictionary<int, List<ItemSlot_Data>> currentDatas => _currentDatas;

    private int _currentPageNum;
    public int currentPageNum;

    private List<FoodData> _ingredientUnlocks = new();

    [Header("")]
    [SerializeField] private RectTransform _ingredientBox;
    [SerializeField] private FoodCondition_Indicator[] _indicators;


    // Editor
    public Food_ScrObj archiveFood;


    // UnityEngine
    private void OnEnable()
    {
        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        _controller.Update_PageDots(_currentDatas.Count, _currentPageNum);

        // subscriptions
        _controller.OnCursor_Outer += CurrentSlots_PageUpdate;

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += CurrentFood_BookmarkToggle;
        _controller.OnOption1_Input += CurrentFood_BookmarkToggle;

        _controller.OnCursor_Input += InfoBox_Update;
        _controller.OnSelect_Input += InfoBox_Update;
        _controller.OnOption1_Input += InfoBox_Update;
        _controller.OnOption2_Input += InfoBox_Update;

        _controller.OnOption2_Input += IngredientBox_Toggle;
        _controller.OnSelect_Input += Hide_IngredientBox;
        _controller.OnCursor_Input += Hide_IngredientBox;
        _controller.OnExit_Input += Hide_IngredientBox;
    }

    private void OnDisable()
    {
        // save current showing slots contents to _currentDatas
        Drag_Cancel();
        _currentDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();

        // subscriptions
        _controller.OnCursor_Outer -= CurrentSlots_PageUpdate;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= CurrentFood_BookmarkToggle;
        _controller.OnOption1_Input -= CurrentFood_BookmarkToggle;

        _controller.OnCursor_Input -= InfoBox_Update;
        _controller.OnSelect_Input -= InfoBox_Update;
        _controller.OnOption1_Input -= InfoBox_Update;
        _controller.OnOption2_Input -= InfoBox_Update;

        _controller.OnOption2_Input -= IngredientBox_Toggle;
        _controller.OnSelect_Input -= Hide_IngredientBox;
        _controller.OnCursor_Input -= Hide_IngredientBox;
        _controller.OnExit_Input -= Hide_IngredientBox;
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

    private void InfoBox_Update()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;
        ItemSlot_Data slotData = cursor.currentSlot.data;

        if (!cursorData.hasItem) return;

        InformationBox info = _controller.infoBox;

        // Bookmark Lock Status
        string lockStatus = null;
        string bookmarkStatus = "Drop";

        if (slotData.hasItem)
        {
            bookmarkStatus = "Swap";
        }

        if (cursorData.isLocked)
        {
            lockStatus = "Bookmark Unavailable\n\n";
        }
        else if (!slotData.hasItem)
        {
            if (cursorData.bookMarked)
            {
                bookmarkStatus = "UnBookmark";
            }
            else
            {
                bookmarkStatus = "Bookmark";
            }
        }

        // ingredient available status
        string ingredientStatus = "Toggle ingredients";
        if (FoodIngredient_Unlocked(cursorData.currentFood) == false)
        {
            ingredientStatus = "Drop";
        }

        string controlInfo = info.UIControl_Template(bookmarkStatus, ingredientStatus, bookmarkStatus);
        info.Update_InfoText(lockStatus + controlInfo);
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
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data slotData = new(currentSlot.data);
        currentSlot.Empty_ItemBox();

        cursor.Assign_Data(slotData);
        cursor.Update_SlotIcon();
    }

    private void Drag_Cancel()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        if (cursor.data.hasItem == false) return;

        ItemSlot_Data cursorData = new(cursor.data);
        cursor.Empty_Item();

        _currentDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();

        ItemSlot_Data emptySlot = slotsController.Empty_SlotData(_currentDatas);
        emptySlot = new(cursorData);

        slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();
    }


    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.data);
        cursor.Empty_Item();

        currentSlot.Assign_Data(cursorData);
        currentSlot.Update_SlotIcon();

        currentSlot.Toggle_BookMark(currentSlot.data.bookMarked);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);
    }

    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data slotData = new(cursor.currentSlot.data);

        Drop_Food();

        cursor.Assign_Data(slotData);
        cursor.Update_SlotIcon();
    }


    private void CurrentFood_BookmarkToggle()
    {
        //
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;

        // check if cursor has item
        if (cursorData.hasItem == false) return;

        Hide_IngredientBox();

        //
        ItemSlot currentSlot = cursor.currentSlot;

        // check if current hover slot has no item
        if (currentSlot.data.hasItem)
        {
            Swap_Food();
            return;
        }

        // drop current item
        Drop_Food();

        if (currentSlot.data.isLocked == true)
        {
            currentSlot.Toggle_BookMark(false);
            return;
        }

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
    private List<Food_ScrObj> Archived_Foods()
    {
        List<Food_ScrObj> archivedFoods = new();

        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem == false) continue;
                if (archivedFoods.Contains(_currentDatas[i][j].currentFood)) continue;

                archivedFoods.Add(_currentDatas[i][j].currentFood);
            }
        }

        return archivedFoods;
    }

    public bool Food_Archived(Food_ScrObj food)
    {
        return _controller.slotsController.FoodAmount(_currentDatas, food) > 0;
    }


    private void RemoveDuplicate_ArchivedFood(Food_ScrObj food)
    {
        if (Food_Archived(food) == false) return;

        int amountCount = 0;

        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem == false) continue;
                if (_currentDatas[i][j].currentFood != food) continue;
                amountCount++;

                if (amountCount <= 1) continue;
                _currentDatas[i][j].Empty_Item();
            }
        }
    }

    private void RemoveDuplicate_ArchivedFoods()
    {
        foreach (Food_ScrObj food in Archived_Foods())
        {
            RemoveDuplicate_ArchivedFood(food);
        }
    }


    private void AddNewPage_onFull()
    {
        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem) continue;
                return;
            }
        }

        _controller.slotsController.AddNewPage_ItemSlotDatas(_currentDatas);
    }

    public ItemSlot_Data Archive_Food(Food_ScrObj food)
    {
        // check if non duplicate food
        if (Food_Archived(food)) return null;

        // check if food has ingredients
        if (food.ingredients.Count <= 0) return null;

        AddNewPage_onFull();

        for (int i = 0; i < _currentDatas.Count; i++)
        {
            for (int j = 0; j < _currentDatas[i].Count; j++)
            {
                if (_currentDatas[i][j].hasItem == true) continue;

                FoodData archiveData = new(food);
                _currentDatas[i][j] = new(archiveData);

                // lock toggle according to cooked food
                Data_Controller dataController = _controller.vehicleController.mainController.dataController;
                _currentDatas[i][j].isLocked = !dataController.CookedFood(food);

                return _currentDatas[i][j];
            }
        }

        return null;
    }


    private bool FoodIngredient_Unlocked(Food_ScrObj food)
    {
        for (int i = 0; i < _ingredientUnlocks.Count; i++)
        {
            if (food != _ingredientUnlocks[i].foodScrObj) continue;
            return true;
        }
        return false;
    }

    public void Unlock_FoodIngredient(Food_ScrObj food)
    {
        for (int i = 0; i < _ingredientUnlocks.Count; i++)
        {
            if (food != _ingredientUnlocks[i].foodScrObj) continue;
            return;
        }

        _ingredientUnlocks.Add(new(food));
    }


    // Ingredient Box Control
    private void IngredientBox_Toggle()
    {
        ItemSlot_Data cursorData = _controller.slotsController.cursor.data;

        if (cursorData.hasItem == false) return;
        if (FoodIngredient_Unlocked(cursorData.currentFood) == false)
        {
            Drag_Cancel();
            return;
        }

        if (_ingredientBox.gameObject.activeSelf == false)
        {
            Show_IngredientBox();
            return;
        }

        Hide_IngredientBox();
    }

    private void Show_IngredientBox()
    {
        if (_ingredientBox.gameObject.activeSelf) return;

        // set active
        _ingredientBox.gameObject.SetActive(true);

        // update position to info box
        float infoBoxX = _controller.infoBox.transform.position.x - 21f;
        _ingredientBox.transform.position = new Vector2(infoBoxX, _ingredientBox.transform.position.y);

        Update_IngredientBox();
    }

    private void Hide_IngredientBox()
    {
        if (_ingredientBox.gameObject.activeSelf == false) return;

        _ingredientBox.gameObject.SetActive(false);
    }


    private void Update_IngredientBox()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;

        if (cursorData.hasItem == false) return;

        for (int i = 0; i < _indicators.Length; i++)
        {
            _indicators[i].Indicate(cursorData.currentFood.ingredients[i]);
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ArchiveMenu_Controller))]
public class ArchiveMenu_Controller_Controller_Inspector : Editor
{
    //
    private SerializedProperty _archiveFoodProp;

    private void OnEnable()
    {
        _archiveFoodProp = serializedObject.FindProperty("archiveFood");
    }


    //
    public override void OnInspectorGUI()
    {
        ArchiveMenu_Controller menu = (ArchiveMenu_Controller)target;

        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);

        //
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(_archiveFoodProp, GUIContent.none);
        Food_ScrObj archiveFood = (Food_ScrObj)_archiveFoodProp.objectReferenceValue;

        if (GUILayout.Button("Archive Food"))
        {
            menu.Archive_Food(archiveFood);
        }

        EditorGUILayout.EndHorizontal();
        //

        serializedObject.ApplyModifiedProperties();
    }
}
#endif