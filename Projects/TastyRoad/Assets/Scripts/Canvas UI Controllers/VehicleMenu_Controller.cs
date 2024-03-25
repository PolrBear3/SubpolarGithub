using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public interface IVehicleMenu
{
    public List<ItemSlot> ItemSlots();

    public bool MenuInteraction_Active();

    public void Exit_MenuInteraction();
}

public class VehicleMenu_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _playerInput;

    [SerializeField] private GameObject _canvas;

    private List<ItemSlot> _itemBoxes = new();
    public List<ItemSlot> itemBoxes => _itemBoxes;

    [HideInInspector] public ItemSlot currentItemBox;

    [Header("Coin Amounts")]
    [SerializeField] private TextMeshProUGUI _goldCoinText;
    [SerializeField] private TextMeshProUGUI _stationCoinText;
    [SerializeField] private TextMeshProUGUI _gasCoinText;

    [Header("Insert Vehicle Prefab")]
    [SerializeField] private Vehicle_Controller _vehicleController;
    public Vehicle_Controller vehicleController => _vehicleController;

    [Header("Menu Controllers")]
    [SerializeField] private FoodMenu_Controller _foodMenu;
    public FoodMenu_Controller foodMenu => _foodMenu;

    [SerializeField] private StationMenu_Controller _stationMenu;
    public StationMenu_Controller stationMenu => _stationMenu;

    [SerializeField] private ArchiveMenu_Controller _arhiveMenu;
    public ArchiveMenu_Controller arhiveMenu => _arhiveMenu;

    [Header("Menu Control")]
    [SerializeField] private List<GameObject> _menus = new();
    [SerializeField] private List<GameObject> _menuIcons = new();

    private int _currentMenuNum;
    public int currentMenuNum => _currentMenuNum;

    public delegate void Cursor_Event(float value);
    public event Cursor_Event OnCursorControl_Input;

    public delegate void Menu_Event();
    public event Menu_Event MenuOpen_Event;
    public event Menu_Event OnSelect_Input;



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
        Vector2 input = value.Get<Vector2>();

        // only gets left and right input
        OnCursorControl_Input?.Invoke(input.x);

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu))
        {
            if (currentMenu.MenuInteraction_Active() == true) return;
        }

        CursorDirection_Control(input);
    }

    private void OnSelect()
    {
        OnSelect_Input?.Invoke();
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

        VehicleMenu_Toggle(false);
        _vehicleController.detection.player.Player_Input().enabled = true;
    }



    // Canvas Toggle on off
    public void VehicleMenu_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _canvas.gameObject.SetActive(false);
            _playerInput.enabled = false;

            return;
        }

        _canvas.gameObject.SetActive(true);
        _playerInput.enabled = true;

        CoinText_Update();
        Menu_Control(currentMenuNum);
    }



    /// <summary>
    /// Coin amount text update
    /// </summary>
    private void CoinText_Update()
    {
        _goldCoinText.text = Main_Controller.currentGoldCoin.ToString();
        _stationCoinText.text = Main_Controller.currentStationCoin.ToString();
        _gasCoinText.text = Main_Controller.currentGasCoin.ToString();
    }



    // Item Box Main Control
    private void Assign_All_BoxNum()
    {
        int numCount = 0;

        for (int i = 0; i < itemBoxes.Count; i++)
        {
            itemBoxes[i].Assign_BoxNum(numCount);

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

        if (nextBoxNum < 0) return;
        if (nextBoxNum >= itemBoxes.Count) return;

        Vector2 currentNum = currentItemBox.gridNum;
        Vector2 nextNum = itemBoxes[nextBoxNum].gridNum;

        if (currentNum.x != nextNum.x && currentNum.y != nextNum.y) return;

        currentItemBox.BoxSelect_Toggle(false);

        currentItemBox = itemBoxes[nextBoxNum];
        currentItemBox.BoxSelect_Toggle(true);
    }

    public void Menu_Control(int controlNum)
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
        _itemBoxes = newMenu.ItemSlots();

        Assign_All_BoxNum();

        // all actions for current menu when it is opened
        MenuOpen_Event?.Invoke();

        // set starting cursor at first box
        currentItemBox = itemBoxes[0];
        currentItemBox.BoxSelect_Toggle(true);
    }
}