using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IVehicleMenu
{
    public List<VechiclePanel_ItemBox> ItemBoxes();
    public bool MenuInteraction_Active();

    public void Exit_MenuInteraction();
}

public class VehiclePanel_Controller : MonoBehaviour
{
    [HideInInspector] public List<VechiclePanel_ItemBox> itemBoxes = new();
    [HideInInspector] public VechiclePanel_ItemBox currentItemBox;

    [Header("Insert Vehicle Prefab")]
    public Vehicle_Controller vehicleController;

    [Header("All Menu Controllers")]
    public FoodMenu_Controller foodMenu;
    public ArchiveMenu_Controller archiveMenu;

    [Header("Menu Control")]
    [SerializeField] private List<GameObject> _menus = new();
    [SerializeField] private List<GameObject> _menuIcons = new();

    private int _currentMenuNum;

    // UnityEngine
    private void Awake()
    {
        Menu_Control(0);
    }

    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        Vector2 input = value.Get<Vector2>();

        CursorDirection_Control(input);
    }

    private void OnOption1()
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        Menu_Control(-1);
    }

    private void OnOption2()
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        Menu_Control(1);
    }

    private void OnExit()
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;

        if (currentMenu.MenuInteraction_Active() == true)
        {
            currentMenu.Exit_MenuInteraction();
            return;
        }

        vehicleController.VehiclePanel_Toggle(false);
        vehicleController.Player_InputSystem_Toggle(true);
    }

    // Item Box Main Control
    private void Assign_All_BoxNum()
    {
        int numCount = 0;

        for (int i = 0; i < itemBoxes.Count; i++)
        {
            itemBoxes[i].boxNum = numCount;

            numCount++;
        }
    }

    private void CursorDirection_Control(Vector2 inputDireciton)
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        float calculatedDirection = inputDireciton.x + -(inputDireciton.y * 5);

        int convertedDirection = (int)calculatedDirection;
        int nextBoxNum = currentItemBox.boxNum + convertedDirection;

        // cursor moves up outside
        if (nextBoxNum < 0) return;

        // cursor moves down outside
        if (nextBoxNum > itemBoxes.Count - 1) return;

        currentItemBox.BoxSelect_Toggle(false);

        currentItemBox = itemBoxes[nextBoxNum];
        currentItemBox.BoxSelect_Toggle(true);
    }

    private void Menu_Control(int controlNum)
    {
        if (currentItemBox != null) currentItemBox.BoxSelect_Toggle(false);

        _menus[_currentMenuNum].SetActive(false);
        _menuIcons[_currentMenuNum].SetActive(false);

        _currentMenuNum += controlNum;

        if (_currentMenuNum > _menus.Count - 1) _currentMenuNum = 0;
        else if (_currentMenuNum < 0) _currentMenuNum = _menus.Count - 1;

        _menus[_currentMenuNum].SetActive(true);
        _menuIcons[_currentMenuNum].SetActive(true);

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu newMenu) == false) return;
        itemBoxes = newMenu.ItemBoxes();

        Assign_All_BoxNum();

        // set starting cursor at first box
        currentItemBox = itemBoxes[0];
        currentItemBox.BoxSelect_Toggle(true);
    }
}