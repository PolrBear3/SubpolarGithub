using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public interface IVehicleMenu
{
    public bool MenuInteraction_Active();
}

public class VehicleMenu_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _playerInput;


    [Header("")]
    [SerializeField] private Vehicle_Controller _vehicleController;
    public Vehicle_Controller vehicleController => _vehicleController;

    [SerializeField] private ItemSlots_Controller _slotsController;
    public ItemSlots_Controller slotsController => _slotsController;


    [Header("")]
    [SerializeField] private GameObject _menuPanel;
    public GameObject menuPanel => _menuPanel;


    [Header("")]
    [SerializeField] private List<GameObject> _menus = new();

    [SerializeField] private FoodMenu_Controller _foodMenu;
    public FoodMenu_Controller foodMenu => _foodMenu;

    [SerializeField] private StationMenu_Controller _stationMenu;
    public StationMenu_Controller stationMenu => _stationMenu;

    [SerializeField] private ArchiveMenu_Controller _archiveMenu;
    public ArchiveMenu_Controller archiveMenu => _archiveMenu;


    private int _currentMenuNum;
    public int currentMenuNum => _currentMenuNum;

    public delegate void Cursor_Event(float value);
    public event Cursor_Event OnCursorControl_Input;

    public delegate void Menu_Event();
    public event Menu_Event MenuOpen_Event;

    public event Menu_Event OnCursor_Input;

    public event Menu_Event OnSelect_Input;
    public event Menu_Event OnHoldSelect_Input;

    public event Menu_Event OnOption1_Input;
    public event Menu_Event OnOption2_Input;

    public event Menu_Event OnExit_Input;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out PlayerInput input)) { _playerInput = input; }
    }

    private void Start()
    {
        VehicleMenu_Toggle(false);
    }


    // ISaveLoadable
    public void Save_Data()
    {
        OnExit();

        for (int i = 0; i < _menus.Count; i++)
        {
            if (!_menus[i].TryGetComponent(out ISaveLoadable saveLoad)) continue;
            saveLoad.Save_Data();
        }
    }

    public void Load_Data()
    {
        for (int i = 0; i < _menus.Count; i++)
        {
            if (!_menus[i].TryGetComponent(out ISaveLoadable saveLoad)) continue;
            saveLoad.Load_Data();
        }
    }


    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        if (_slotsController.cursor.holdTimer.onHold) return;

        Vector2 input = value.Get<Vector2>();

        OnCursorControl_Input?.Invoke(input.x);
        OnCursor_Input?.Invoke();

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        _slotsController.cursor.Navigate_toSlot(input);
    }

    private void OnSelect()
    {
        ItemSlot_Cursor cursor = _slotsController.cursor;

        if (cursor.holdTimer.onHold == false)
        {
            OnSelect_Input?.Invoke();
        }

        cursor.holdTimer.Stop_ClockSpriteRun();
    }

    private void OnSelectDown()
    {
        _slotsController.cursor.holdTimer.Run_ClockSprite();
    }

    private void OnHoldSelect()
    {
        OnHoldSelect_Input?.Invoke();
    }

    private void OnOption1()
    {
        ItemSlot_Cursor _cursor = _slotsController.cursor;

        if (_cursor.holdTimer.onHold) return;

        if (_cursor.Current_Data().hasItem)
        {
            OnOption1_Input?.Invoke();
            return;
        }

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        _menus[_currentMenuNum].SetActive(false);
        Menu_Navigate(false);
        Toggle_NavigatedMenu();
    }

    private void OnOption2()
    {
        ItemSlot_Cursor _cursor = _slotsController.cursor;

        if (_cursor.holdTimer.onHold) return;

        if (_cursor.Current_Data().hasItem)
        {
            OnOption2_Input?.Invoke();
            return;
        }

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        _menus[_currentMenuNum].SetActive(false);
        Menu_Navigate(true);
        Toggle_NavigatedMenu();
    }

    private void OnExit()
    {
        if (_slotsController.cursor.holdTimer.onHold == true) return;

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;

        if (currentMenu.MenuInteraction_Active() == true)
        {
            OnExit_Input?.Invoke();
            return;
        }

        OnExit_Input?.Invoke();

        VehicleMenu_Toggle(false);

        if (_vehicleController.detection.player == null) return;

        _vehicleController.detection.player.Player_Input().enabled = true;
    }


    // Main Control
    public void VehicleMenu_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _menuPanel.SetActive(false);
            _playerInput.enabled = false;

            for (int i = 0; i < _menus.Count; i++)
            {
                _menus[i].SetActive(false);
            }
            
            return;
        }

        _menuPanel.SetActive(true);
        _playerInput.enabled = true;

        Toggle_NavigatedMenu();

        ItemSlot firstSlot = _slotsController.ItemSlot(Vector2.zero);
        _slotsController.cursor.Navigate_toSlot(firstSlot);
    }


    private void Menu_Navigate(bool moveNext)
    {
        if (moveNext == true)
        {
            _currentMenuNum = (_currentMenuNum + 1) % _menus.Count;
            return;
        }

        _currentMenuNum = (_currentMenuNum - 1 + _menus.Count) % _menus.Count;
    }

    public void Toggle_NavigatedMenu()
    {
        _menus[_currentMenuNum].SetActive(true);
        _slotsController.SlotsAssign_Update();

        MenuOpen_Event?.Invoke();
    }
}