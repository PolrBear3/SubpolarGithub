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
    [SerializeField] private GameObject _menuToggleFile;
    public GameObject menuToggleFile => _menuToggleFile;

    [Header("")]
    private List<ItemSlot> _itemSlots = new();
    public List<ItemSlot> itemSlots => _itemSlots;

    [SerializeField] private ItemSlot_Cursor _cursor;
    public ItemSlot_Cursor cursor => _cursor;

    [SerializeField] private GameObject _newItemSlot;
    public GameObject newItemSlot => _newItemSlot;



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

    [SerializeField] private ArchiveMenu_Controller _archiveMenu;
    public ArchiveMenu_Controller archiveMenu => _archiveMenu;



    [Header("Menu Control")]
    [SerializeField] private List<GameObject> _menus = new();
    [SerializeField] private List<GameObject> _menuIcons = new();



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
        // multiple save restriction
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
        Vector2 input = value.Get<Vector2>();

        // only gets left and right input
        OnCursorControl_Input?.Invoke(input.x);

        OnCursor_Input?.Invoke();

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu))
        {
            if (currentMenu.MenuInteraction_Active() == true) return;
        }

        CursorDirection_Control(input);
    }

    private void OnSelect()
    {
        _cursor.holdTimer.Stop_ClockSpriteRun();

        OnSelect_Input?.Invoke();
    }

    private void OnSelectDown()
    {
        _cursor.holdTimer.Run_ClockSprite();
    }

    private void OnHoldSelect()
    {
        OnHoldSelect_Input?.Invoke();
    }

    private void OnOption1()
    {
        if (_cursor.data.hasItem)
        {
            OnOption1_Input?.Invoke();
            return;
        }

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        Menu_Control(-1);
    }

    private void OnOption2()
    {
        if (_cursor.data.hasItem)
        {
            OnOption2_Input?.Invoke();
            return;
        }

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        Menu_Control(1);
    }

    private void OnExit()
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;

        if (currentMenu.MenuInteraction_Active() == true)
        {
            OnExit_Input?.Invoke();
            return;
        }

        VehicleMenu_Toggle(false);

        if (_vehicleController.detection.player == null) return;

        _vehicleController.detection.player.Player_Input().enabled = true;
    }

    private void OnTest()
    {
        if (!_menus[currentMenuNum].TryGetComponent(out ItemSlots_Controller slotsController)) return;
        slotsController.Add_Slot(1);
    }



    // Canvas Toggle on off
    public void VehicleMenu_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _menuToggleFile.gameObject.SetActive(false);
            _playerInput.enabled = false;

            return;
        }

        _menuToggleFile.gameObject.SetActive(true);
        _playerInput.enabled = true;

        CoinText_Update();

        Menu_Control();
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



    // Cursor Control
    private ItemSlot Item_Slot(Vector2 gridNum)
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].gridNum != gridNum) continue;
            return _itemSlots[i];
        }

        return null;
    }

    private void CursorDirection_Control(Vector2 inputDireciton)
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        Vector2 currentGridNum = _cursor.currentSlot.gridNum;
        Vector2 nextGridNum = new(currentGridNum.x + inputDireciton.x, currentGridNum.y + -inputDireciton.y);

        if (Item_Slot(nextGridNum) == null) return;

        // update to next slot
        _cursor.Assign_CurrentSlot(Item_Slot(nextGridNum));
    }



    // Main Control
    public void AssignMain_ItemSlots(List<ItemSlot> menuItemSlots)
    {
        _itemSlots = menuItemSlots;
    }



    private void Menu_Control()
    {
        _menus[_currentMenuNum].SetActive(true);
        _menuIcons[_currentMenuNum].SetActive(true);

        // all actions for current menu when it is opened
        MenuOpen_Event?.Invoke();

        // set starting cursor at first box
        _cursor.Assign_CurrentSlot(Item_Slot(new Vector2(0, 0)));
    }

    public void Menu_Control(int controlNum)
    {
        _menus[_currentMenuNum].SetActive(false);
        _menuIcons[_currentMenuNum].SetActive(false);

        _currentMenuNum += controlNum;

        if (_currentMenuNum > _menus.Count - 1) _currentMenuNum = 0;
        else if (_currentMenuNum < 0) _currentMenuNum = _menus.Count - 1;

        _menus[_currentMenuNum].SetActive(true);
        _menuIcons[_currentMenuNum].SetActive(true);

        // all actions for current menu when it is opened
        MenuOpen_Event?.Invoke();

        // set starting cursor at first box
        _cursor.Assign_CurrentSlot(Item_Slot(new Vector2(0, 0)));
    }
}