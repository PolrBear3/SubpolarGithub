using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu
{
    [SerializeField] private VehiclePanel_Controller _controller;

    [Header("")]
    [SerializeField] private Vector2 _gridData;
    [SerializeField] private List<VechiclePanel_ItemBox> _itemBoxes = new();

    private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;

    [Header("")]
    [SerializeField] private GameObject _ingredientBubble;
    [SerializeField] private VechiclePanel_ItemBox _ingredientIcon1;
    [SerializeField] private VechiclePanel_ItemBox _ingredientIcon2;

    // UnityEngine
    private void Start()
    {
        Set_ItemBoxes_GridNum();
    }

    // InputSystem
    private void OnSelect()
    {
        Select_AvailableFood();
        IngredientBubble_Update();
    }
    private void OnCursorControl()
    {
        _ingredientBubble.SetActive(false);
    }

    // IVehicleMenu
    public List<VechiclePanel_ItemBox> ItemBoxes()
    {
        // add functions that needs to be run whenever menu is opened
        Update_Archived_Foods();
        //

        return _itemBoxes;
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

        for (int i = 0; i < _itemBoxes.Count; i++)
        {
            _itemBoxes[i].gridNum = gridCount;

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
            if (_itemBoxes[i].hasItem) continue;

            _itemBoxes[i].Assign_Item(archivedFoods[i]);
        }
    }

    private void Select_AvailableFood()
    {
        VechiclePanel_ItemBox currentBox = _controller.currentItemBox;

        if (currentBox.hasItem == false) return;

        currentBox.Toggle_BookMark();

        if (currentBox.bookMarked == false)
        {
            _bookmarkedFoods.Remove(currentBox.currentFood);
            return;
        }

        _bookmarkedFoods.Add(currentBox.currentFood);
    }

    //
    private void IngredientBubble_Update()
    {
        VechiclePanel_ItemBox currentBox = _controller.currentItemBox;

        if (currentBox.hasItem == false) return;

        if (currentBox.bookMarked == false)
        {
            _ingredientBubble.SetActive(false);
            return;
        }

        Vector2 itemBoxPos = new(currentBox.transform.localPosition.x, currentBox.transform.localPosition.y + 75f);

        _ingredientBubble.transform.localPosition = itemBoxPos;
        _ingredientBubble.SetActive(true);

        Food_ScrObj currentFood = currentBox.currentFood;

        Food_ScrObj ingredient1 = currentFood.ingredients[0].foodScrObj;
        _ingredientIcon1.Assign_Item(ingredient1);
        _ingredientIcon1.Assign_State(currentFood.ingredients[0].stateData);

        if (currentFood.ingredients.Count <= 1) return;

        Food_ScrObj ingredient2 = currentFood.ingredients[1].foodScrObj;
        _ingredientIcon2.Assign_Item(ingredient2);
        _ingredientIcon2.Assign_State(currentFood.ingredients[1].stateData);
    }
}