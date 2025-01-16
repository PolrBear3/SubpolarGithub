using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public interface IVehicleMenu
{
    public bool MenuInteraction_Active();
    public Dictionary<int, List<ItemSlot_Data>> ItemSlot_Datas();
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
    [SerializeField] private Image _menuPanel;
    public Image menuPanel => _menuPanel;

    private Sprite _defaultPanelSprite;


    [Header("")]
    [SerializeField] private Image[] _pageDots;
    [SerializeField] private Sprite[] _dotSprites;


    [Header("")]
    [SerializeField] private InformationBox _infoBox;
    public InformationBox infoBox => _infoBox;

    [SerializeField] private ItemSlot _flipUpdateSlot;


    [Header("")]
    [SerializeField] private List<GameObject> _menus = new();
    public List<GameObject> menus => _menus;

    [SerializeField] private Sprite[] _menuCursorSprites;
    public Sprite[] menuCursorSprites => _menuCursorSprites;


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

    public event Cursor_Event OnCursorControl_Input;

    public event Menu_Event OnCursor_Input;
    public event Menu_Event OnCursor_Outer;

    public event Menu_Event OnSelect_Input;

    public event Menu_Event OnHoldSelect_Input;
    public event Menu_Event OnHoldEmptySelect_Input;

    private bool _onHold;
    private float _pressStartTime;

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
        _playerInput.actions["Select"].started += ctx => OnPressStart();
        _playerInput.actions["Select"].canceled += ctx => OnPressEnd();

        _defaultPanelSprite = _menuPanel.sprite;

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


    // IVehicleMenu
    private bool MenuInteraction_Active()
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return false;
        if (currentMenu.MenuInteraction_Active()) return true;
        return false;
    }


    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        if (_onHold) return;

        Vector2 input = value.Get<Vector2>();

        OnCursorControl_Input?.Invoke(input.x);

        if (MenuInteraction_Active()) return;

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


    private void OnPressStart()
    {
        UI_ClockTimer timer = _slotsController.cursor.holdTimer;

        _pressStartTime = Time.time;  // Record the time when the button is pressed
        _onHold = true;

        // Start the clock timer for visual feedback
        timer.Run_ClockSprite();
    }

    private void OnPressEnd()
    {
        UI_ClockTimer timer = _slotsController.cursor.holdTimer;

        float pressDuration = Time.time - _pressStartTime;
        float holdTime = 1;

        // Stop the clock timer when the button is released
        timer.Stop_ClockSpriteRun();

        if (pressDuration >= holdTime)
        {
            HoldSelect();
        }
        else
        {
            Select();
        }

        _onHold = false;
    }

    private void Select()
    {
        OnSelect_Input?.Invoke();

        if (MenuInteraction_Active()) return;

        _infoBox.gameObject.SetActive(_slotsController.cursor.data.hasItem);
        _infoBox.Update_RectLayout();
    }

    private void HoldSelect()
    {
        OnHoldSelect_Input?.Invoke();

        _infoBox.gameObject.SetActive(false);

        OnHoldEmptySelect_Input?.Invoke();
    }


    private void OnOption1()
    {
        if (_onHold) return;

        ItemSlot_Cursor cursor = _slotsController.cursor;

        if (cursor.data.hasItem)
        {
            OnOption1_Input?.Invoke();

            if (MenuInteraction_Active()) return;

            _infoBox.gameObject.SetActive(cursor.data.hasItem);
            infoBox.Update_RectLayout();

            return;
        }

        if (MenuInteraction_Active()) return;

        _menus[_currentMenuNum].SetActive(false);
        _infoBox.gameObject.SetActive(false);

        Menu_Navigate(false);
        Toggle_NavigatedMenu();

        cursor.Update_CursorSprite(_menuCursorSprites[_currentMenuNum]);
    }

    private void OnOption2()
    {
        if (_onHold) return;

        ItemSlot_Cursor cursor = _slotsController.cursor;

        if (cursor.data.hasItem)
        {
            OnOption2_Input?.Invoke();

            if (MenuInteraction_Active()) return;

            _infoBox.gameObject.SetActive(cursor.data.hasItem);
            infoBox.Update_RectLayout();

            return;
        }

        if (MenuInteraction_Active()) return;

        _menus[_currentMenuNum].SetActive(false);
        _infoBox.gameObject.SetActive(false);

        Menu_Navigate(true);
        Toggle_NavigatedMenu();

        cursor.Update_CursorSprite(_menuCursorSprites[_currentMenuNum]);
    }

    private void OnExit()
    {
        if (_onHold) return;

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;

        _infoBox.gameObject.SetActive(false);

        if (currentMenu.MenuInteraction_Active() == true)
        {
            OnExit_Input?.Invoke();
            return;
        }

        OnExit_Input?.Invoke();

        VehicleMenu_Toggle(false);

        if (_vehicleController.detection.player == null) return;

        _vehicleController.detection.player.Player_Input().enabled = true;
        _vehicleController.detection.player.detectionController.Toggle_BoxCollider(true);
    }


    // Main Control
    public void VehicleMenu_Toggle(bool toggleOn)
    {
        _vehicleController.mainController.Player().Toggle_Controllers(!toggleOn);

        if (toggleOn == false)
        {
            _menuPanel.gameObject.SetActive(false);
            _playerInput.enabled = false;

            for (int i = 0; i < _menus.Count; i++)
            {
                _menus[i].SetActive(false);
            }

            return;
        }

        _menuPanel.gameObject.SetActive(true);
        _playerInput.enabled = true;

        Toggle_NavigatedMenu();

        ItemSlot_Cursor cursor = _slotsController.cursor;
        cursor.Update_CursorSprite(_menuCursorSprites[_currentMenuNum]);

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


    public void Update_ItemSlots(GameObject menu, List<ItemSlot_Data> currentPageSlots)
    {
        if (menu.activeSelf == false) return;

        ItemSlots_Controller controller = _slotsController;

        controller.Set_Datas(currentPageSlots);
        controller.SlotsAssign_Update();
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


    public void Update_PanelSprite(Sprite updateSprite)
    {
        if (updateSprite == null)
        {
            _menuPanel.sprite = _defaultPanelSprite;
            return;
        }

        _menuPanel.sprite = updateSprite;
    }


    public void Update_MenuCursorSprite(Sprite updateSprite)
    {
        _menuCursorSprites[currentMenuNum] = updateSprite;
        _slotsController.cursor.cursorImage.sprite = updateSprite;
    }

    public void Update_PageDots(int pageAmount)
    {
        int pageCount = pageAmount;

        for (int i = 0; i < _pageDots.Length; i++)
        {
            if (_pageDots[i].sprite == _dotSprites[2])
            {
                pageCount--;
                continue;
            }

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
    }
    public void Update_PageDots(int pageAmount, int currentPageNum)
    {
        // reset
        foreach (Image dot in _pageDots)
        {
            dot.sprite = null;
        }

        // indicate
        _pageDots[currentPageNum].sprite = _dotSprites[2];

        Update_PageDots(pageAmount);
    }
}