using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu
{
    [SerializeField] private VehiclePanel_Controller _controller;

    [Header("")]
    [SerializeField] private Vector2 _layoutCount;
    [SerializeField] private List<VechiclePanel_ItemBox> _itemBoxes = new();

    [Header("")]
    public List<Food_ScrObj> availableFoodOrders = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out VehiclePanel_Controller controller)) { _controller = controller; }
    }

    // IVehicleMenu
    public List<VechiclePanel_ItemBox> ItemBoxes()
    {
        // add functions that needs to be run whenever menu is opened
        Update_Archived_CookedFoods();
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

    // Archive Cooked Foods to Available Orders Export System
    private void Update_Archived_CookedFoods()
    {
        Main_Controller mainController = _controller.vehicleController.mainController;
        List<Food_ScrObj> archivedFoods = mainController.archiveFoods;

        for (int i = 0; i < archivedFoods.Count; i++)
        {
            if (_itemBoxes[i].hasItem) continue;

            _itemBoxes[i].Assign_Item(archivedFoods[i]);
        }
    }
}