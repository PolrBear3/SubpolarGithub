using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

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
    [SerializeField] private Image[] _pageDots;
    [SerializeField] private Sprite[] _dotSprites;


    [Header("")]
    [SerializeField] private InformationBox _infoBox;
    public InformationBox infoBox => _infoBox;

    [SerializeField] private ItemSlot _flipUpdateSlot;


    [Header("")]
    [SerializeField] private List<GameObject> _menus = new();

    [SerializeField] private Sprite[] _menuCursorSprites;


    [Header("")]
    [SerializeField] private FoodMenu_Controller _foodMenu;
    public FoodMenu_Controller foodMenu => _foodMenu;

    [SerializeField] private StationMenu_Controller _stationMenu;
    public StationMenu_Controller stationMenu => _stationMenu;

    [SerializeField] private ArchiveMenu_Controller _archiveMenu;
    public ArchiveMenu_Controller archiveMenu => _archiveMenu;


    private int _currentMenuNum;
    public int currentMenuNum => _currentMenuNum;


    public delegate void Menu_Event();
    public delegate void Cursor_Event(float value);

    public event Menu_Event MenuOpen_Event;

    public event Menu_Event OnCursor_Input;
    public event Menu_Event OnCursor_Outer;
    public event Cursor_Event OnCursorControl_Input;

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

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        int prevSlotNum = (int)_slotsController.cursor.currentSlot.gridNum.x;

        ItemSlot_Cursor cursor = _slotsController.cursor;
        ItemSlot nextSlot = cursor.Navigated_NextSlot(input);

        if (nextSlot == null)
        {
            OnCursor_Outer?.Invoke();
            OnCursor_Input?.Invoke();

            InfoBox_FlipUpdate(prevSlotNum);
            return;
        }

        cursor.Navigate_toSlot(nextSlot);
        OnCursor_Input?.Invoke();

        InfoBox_FlipUpdate(prevSlotNum);
    }

    private void OnSelect()
    {
        ItemSlot_Cursor cursor = _slotsController.cursor;
        UI_ClockTimer timer = cursor.holdTimer;

        if (timer.holdFinished == true)
        {
            timer.Stop_ClockSpriteRun();
            return;
        }

        timer.Stop_ClockSpriteRun();
        if (timer.onHold == true) return;

        OnSelect_Input?.Invoke();

        _infoBox.gameObject.SetActive(_slotsController.cursor.Current_Data().hasItem);
        infoBox.Update_RectLayout();
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
        ItemSlot_Cursor cursor = _slotsController.cursor;

        if (cursor.holdTimer.onHold) return;

        if (cursor.Current_Data().hasItem)
        {
            OnOption1_Input?.Invoke();

            _infoBox.gameObject.SetActive(_slotsController.cursor.Current_Data().hasItem);
            infoBox.Update_RectLayout();

            return;
        }

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        _menus[_currentMenuNum].SetActive(false);
        Menu_Navigate(false);
        Toggle_NavigatedMenu();

        // change cursor sprite to current menu type cursor
        cursor.Update_DefaultCursor(_menuCursorSprites[_currentMenuNum]);
        cursor.cursorImage.sprite = cursor.defaultCursor;

        // current menu update dialog
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(_currentMenuNum);
    }

    private void OnOption2()
    {
        ItemSlot_Cursor cursor = _slotsController.cursor;

        if (cursor.holdTimer.onHold) return;

        if (cursor.Current_Data().hasItem)
        {
            OnOption2_Input?.Invoke();

            _infoBox.gameObject.SetActive(_slotsController.cursor.Current_Data().hasItem);
            infoBox.Update_RectLayout();

            return;
        }

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;
        if (currentMenu.MenuInteraction_Active() == true) return;

        _menus[_currentMenuNum].SetActive(false);
        Menu_Navigate(true);
        Toggle_NavigatedMenu();

        // change cursor sprite to current menu type cursor
        cursor.Update_DefaultCursor(_menuCursorSprites[_currentMenuNum]);
        cursor.cursorImage.sprite = cursor.defaultCursor;

        // current menu update dialog
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(_currentMenuNum);
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
        _infoBox.gameObject.SetActive(false);

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
        _infoBox.Flip_toDefault();
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


    // UI Control
    private void InfoBox_FlipUpdate(int prevSlotNumX)
    {
        ItemSlot currentSlot = _slotsController.cursor.currentSlot;
        float flipUpdateSlotX = _flipUpdateSlot.gridNum.x;

        bool navigateRight = prevSlotNumX < flipUpdateSlotX && currentSlot.gridNum.x >= flipUpdateSlotX;
        bool navigateLeft = prevSlotNumX >= flipUpdateSlotX && currentSlot.gridNum.x < flipUpdateSlotX;

        if (navigateRight == false && navigateLeft == false) return;

        _infoBox.Flip();
    }

    public void Update_PageDots(int pageAmount, int currentPageNum)
    {
        int pageCount = pageAmount;

        for (int i = 0; i < _pageDots.Length; i++)
        {
            // lock
            if (pageCount <= 0)
            {
                _pageDots[i].sprite = _dotSprites[0];
                continue;
            }

            // unlock
            _pageDots[i].sprite = _dotSprites[1];
            pageCount--;
        }

        // indicate
        _pageDots[currentPageNum].sprite = _dotSprites[2];
    }
}