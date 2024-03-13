using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [SerializeField] private VehicleMenu_Controller _controller;

    [Header("")]
    [SerializeField] private Vector2 _gridData;

    [SerializeField] private List<ItemSlot> _itemSlots = new();
    public List<ItemSlot> itemSlots => _itemSlots;

    private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;

    [Header("")]
    [SerializeField] private GameObject _ingredientBubble;
    [SerializeField] private ItemSlot _ingredientIcon1;
    [SerializeField] private ItemSlot _ingredientIcon2;

    private Coroutine _bubbleCoroutine;



    // UnityEngine
    private void Start()
    {
        Set_ItemBoxes_GridNum();
        Load_Data();
    }

    private void OnEnable()
    {
        _controller.MenuOpen_Event += Update_Archived_Foods;
        _controller.MenuOpen_Event += Show_IngredientBubble;
    }

    private void OnDisable()
    {
        _ingredientBubble.SetActive(false);

        _controller.MenuOpen_Event -= Update_Archived_Foods;
        _controller.MenuOpen_Event -= Show_IngredientBubble;
    }



    // ISaveLoadable
    public void Save_Data()
    {
        List<ItemSlot_Data> saveSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            saveSlots.Add(_itemSlots[i].data);
        }

        ES3.Save("archiveMenuSlots", saveSlots);
    }

    public void Load_Data()
    {
        List<ItemSlot_Data> loadSlots = ES3.Load("archiveMenuSlots", new List<ItemSlot_Data>());

        for (int i = 0; i < loadSlots.Count; i++)
        {
            _itemSlots[i].data = loadSlots[i];

            if (_itemSlots[i].data.hasItem == false) continue;

            _itemSlots[i].Assign_Item(_itemSlots[i].data.currentFood);
            _itemSlots[i].Toggle_BookMark(_itemSlots[i].data.bookMarked);
        }
    }



    // InputSystem
    private void OnSelect()
    {
        Select_AvailableFood();
    }

    private void OnCursorControl()
    {
        _ingredientBubble.SetActive(false);
        Show_IngredientBubble();
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

    public void Exit_MenuInteraction()
    {

    }



    // All Start Functions are Here
    private void Set_ItemBoxes_GridNum()
    {
        Vector2 gridCount = Vector2.zero;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].Assign_GridNum(gridCount);

            gridCount.x ++;

            if (gridCount.x != _gridData.x) continue;

            gridCount.x = 0;
            gridCount.y ++;
        }
    }

    // Archive Cooked Foods to Available Orders Export System
    private void Update_Archived_Foods()
    {
        Main_Controller mainController = _controller.vehicleController.mainController;
        List<Food_ScrObj> archivedFoods = mainController.archiveFoods;

        for (int i = 0; i < archivedFoods.Count; i++)
        {
            if (_itemSlots[i].data.hasItem) continue;

            _itemSlots[i].Assign_Item(archivedFoods[i]);
        }
    }

    private void Select_AvailableFood()
    {
        ItemSlot currentBox = _controller.currentItemBox;

        if (currentBox.data.hasItem == false) return;

        currentBox.Toggle_BookMark();

        Food_ScrObj currentFood = currentBox.data.currentFood;
        Main_Controller main = _controller.vehicleController.mainController;

        if (currentBox.data.bookMarked == false)
        {
            main.RemoveFood_fromBookmark(currentFood);
            return;
        }

        main.AddFood_toBookmark(currentFood);
    }

    //
    private IEnumerator Show_IngredientBubble_Coroutine()
    {
        yield return new WaitForSeconds(1f);

        Update_IngredientBubble();
    }
    private void Show_IngredientBubble()
    {
        if (_bubbleCoroutine != null) StopCoroutine(_bubbleCoroutine);

        _bubbleCoroutine = StartCoroutine(Show_IngredientBubble_Coroutine());
    }

    private void Update_IngredientBubble()
    {
        ItemSlot currentBox = _controller.currentItemBox;

        if (currentBox.data.hasItem == false) return;

        Vector2 itemBoxPos = new(currentBox.transform.localPosition.x, currentBox.transform.localPosition.y + 75f);

        _ingredientBubble.transform.localPosition = itemBoxPos;
        _ingredientBubble.SetActive(true);

        Food_ScrObj currentFood = currentBox.data.currentFood;

        Food_ScrObj ingredient1 = currentFood.ingredients[0].foodScrObj;
        _ingredientIcon1.Assign_Item(ingredient1);
        _ingredientIcon1.Assign_State(currentFood.ingredients[0].stateData);

        if (currentFood.ingredients.Count <= 1) return;

        Food_ScrObj ingredient2 = currentFood.ingredients[1].foodScrObj;
        _ingredientIcon2.Assign_Item(ingredient2);
        _ingredientIcon2.Assign_State(currentFood.ingredients[1].stateData);
    }
}