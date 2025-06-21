using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public interface IVehicleMenu
{
    public void Start_Menu();

    public bool MenuInteraction_Active();
    public Dictionary<int, List<ItemSlot_Data>> ItemSlot_Datas();
}

public class VehicleMenu_Controller : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private Vehicle_Controller _vehicleController;
    public Vehicle_Controller vehicleController => _vehicleController;

    [SerializeField] private ItemSlots_Controller _slotsController;
    public ItemSlots_Controller slotsController => _slotsController;
    
    [Space(20)]
    [SerializeField] private Image _menuPanel;
    public Image menuPanel => _menuPanel;
    
    [SerializeField] private UI_EffectController _uiEffectController;

    private Sprite _defaultPanelSprite;
    
    [Space(20)]
    [SerializeField] private GameObject _pageArrowDirections;
    [SerializeField] private Image[] _pageArrows;
    
    [Space(20)]
    [SerializeField] private Image[] _pageDots;
    [SerializeField] private Sprite[] _dotSprites;

    [Space(20)]
    [SerializeField] private InformationBox _infoBox;
    public InformationBox infoBox => _infoBox;

    [SerializeField] private ItemSlot _flipUpdateSlot;
    
    [Space(20)]
    [SerializeField] private List<GameObject> _menus = new();
    public List<GameObject> menus => _menus;

    [SerializeField] private Sprite[] _menuCursorSprites;
    public Sprite[] menuCursorSprites => _menuCursorSprites;
    
    [Space(20)]
    [SerializeField] private FoodMenu_Controller _foodMenu;
    public FoodMenu_Controller foodMenu => _foodMenu;

    [SerializeField] private StationMenu_Controller _stationMenu;
    public StationMenu_Controller stationMenu => _stationMenu;

    [SerializeField] private ArchiveMenu_Controller _archiveMenu;
    public ArchiveMenu_Controller archiveMenu => _archiveMenu;
    
    [Space(80)]
    [SerializeField] private Input_Manager _inputManager;
    
    
    private int _currentMenuNum;
    public int currentMenuNum => _currentMenuNum;

    public Action<bool> On_MenuToggle;

    public Action OnCursor_Input;
    public Action OnCursor_OuterInput;

    public Action<float> OnCursorControl_Input;
    public Action<float> OnCursor_YInput;

    public Action OnSelect_Input;
    public Action OnHoldSelect_Input;

    public Action OnOption1_Input;
    public Action OnOption2_Input;

    public Action OnExit_Input;


    // UnityEngine
    private void Start()
    {
        _defaultPanelSprite = _menuPanel.sprite;
        _pageArrowDirections.SetActive(false);

        Start_AllMenus();
        VehicleMenu_Toggle(false);
        
        // subscriptions
        _inputManager.OnCursorControl += CursorControl;
        _inputManager.OnSelect += Select;
        _inputManager.OnHoldSelect += HoldSelect;
        _inputManager.OnOption1 += Option1;
        _inputManager.OnOption2 += Option2;
        _inputManager.OnExit += Exit;

        UI_ClockTimer timer = _slotsController.cursor.holdTimer;

        _inputManager.OnSelectStart += timer.Run_ClockSprite;
        _inputManager.OnSelect += timer.Stop_ClockSpriteRun;
        _inputManager.OnHoldSelect += timer.Stop_ClockSpriteRun;

        Localization_Controller.instance.OnLanguageChanged += Update_NavigateText;
    }

    private void OnDestroy()
    { 
        // subscriptions
        _inputManager.OnCursorControl -= CursorControl;
        _inputManager.OnSelect -= Select;
        _inputManager.OnHoldSelect -= HoldSelect;
        _inputManager.OnOption1 -= Option1;
        _inputManager.OnOption2 -= Option2;
        _inputManager.OnExit -= Exit;

        UI_ClockTimer timer = _slotsController.cursor.holdTimer;

        _inputManager.OnSelectStart -= timer.Run_ClockSprite;
        _inputManager.OnSelect -= timer.Stop_ClockSpriteRun;
        _inputManager.OnHoldSelect -= timer.Stop_ClockSpriteRun;
        
        Localization_Controller.instance.OnLanguageChanged -= Update_NavigateText;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        Exit();

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

    private void Start_AllMenus()
    {
        for (int i = 0; i < _menus.Count; i++)
        {
            if (!_menus[i].TryGetComponent(out IVehicleMenu menu)) continue;

            menu.Start_Menu();
        }
    }


    // Input_Controller
    private void CursorControl(Vector2 inputDirection)
    {
        if (Input_Controller.instance.isHolding) return;

        OnCursorControl_Input?.Invoke(inputDirection.x);
        
        if (MenuInteraction_Active()) return;
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        if (inputDirection.x == 0)
        {
            OnCursor_YInput?.Invoke(inputDirection.y);
            return;
        }

        int prevSlotNum = (int)_slotsController.cursor.currentSlot.gridNum.x;

        ItemSlot_Cursor cursor = _slotsController.cursor;
        ItemSlot nextSlot = cursor.Navigated_NextSlot(inputDirection);

        if (nextSlot == null)
        {
            OnCursor_OuterInput?.Invoke();
        }
        else
        {
            cursor.Navigate_toSlot(nextSlot);
        }

        Update_PageArrows();

        OnCursor_Input?.Invoke();
        FlipUpdate_InfoBox(prevSlotNum);
    }

    private void Select()
    {
        OnSelect_Input?.Invoke();

        if (MenuInteraction_Active()) return;

        bool hasItem = _slotsController.cursor.data.hasItem;
        
        _infoBox.gameObject.SetActive(_slotsController.cursor.data.hasItem);
        _infoBox.Update_RectLayout();
    }

    private void HoldSelect()
    {
        OnHoldSelect_Input?.Invoke();
    }


    private void Option1()
    {
        if (Input_Controller.instance.isHolding) return;

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
        
        Audio_Controller.instance.Play_OneShot(gameObject, 3);
    }

    private void Option2()
    {
        if (Input_Controller.instance.isHolding) return;

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
        
        Audio_Controller.instance.Play_OneShot(gameObject, 3);
    }

    private void Exit()
    {
        if (Input_Controller.instance.isHolding) return;

        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return;

        _infoBox.gameObject.SetActive(false);

        if (currentMenu.MenuInteraction_Active() == true)
        {
            OnExit_Input?.Invoke();
            return;
        }

        OnExit_Input?.Invoke();

        VehicleMenu_Toggle(false);
        Main_Controller.instance.Player().Toggle_Controllers(true);
    }


    // Main Control
    private IVehicleMenu Current_IVehicleMenu()
    {
        if (_menus[_currentMenuNum].TryGetComponent(out IVehicleMenu currentMenu) == false) return null;
        return currentMenu;
    }
    
    public void VehicleMenu_Toggle(bool toggleOn)
    {
        Main_Controller.instance.Player().Toggle_Controllers(!toggleOn);

        if (toggleOn == false)
        {
            _menuPanel.gameObject.SetActive(false);
            _inputManager.Toggle_Input(false);

            for (int i = 0; i < _menus.Count; i++)
            {
                _menus[i].SetActive(false);
            }

            On_MenuToggle?.Invoke(false);

            return;
        }

        _menuPanel.gameObject.SetActive(true);
        _inputManager.Toggle_Input(true);

        Toggle_NavigatedMenu();
        _infoBox.Flip_toDefault();

        Update_NavigateText();
        
        _uiEffectController.Update_Scale(_menuPanel.rectTransform);
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
        // toggle menu
        _menus[_currentMenuNum].SetActive(true);
        _slotsController.SlotsAssign_Update();

        // update cursor
        ItemSlot_Cursor cursor = _slotsController.cursor;
        cursor.Update_CursorSprite(_menuCursorSprites[_currentMenuNum]);

        ItemSlot firstSlot = _slotsController.ItemSlot(Vector2.zero);
        _slotsController.cursor.Navigate_toSlot(firstSlot);

        Update_PageArrows();

        // event
        On_MenuToggle?.Invoke(true);

        // info box
        _infoBox.Flip_toDefault();
    }


    public void Update_ItemSlots(GameObject menu, List<ItemSlot_Data> currentPageSlots)
    {
        if (menu.activeSelf == false) return;

        ItemSlots_Controller controller = _slotsController;

        controller.Set_Datas(currentPageSlots);
        controller.SlotsAssign_Update();
    }

    private void FlipUpdate_InfoBox(int prevSlotNumX)
    {
        ItemSlot currentSlot = _slotsController.cursor.currentSlot;
        float flipUpdateSlotX = _flipUpdateSlot.gridNum.x;

        bool navigateRight = prevSlotNumX < flipUpdateSlotX && currentSlot.gridNum.x >= flipUpdateSlotX;
        bool navigateLeft = prevSlotNumX >= flipUpdateSlotX && currentSlot.gridNum.x < flipUpdateSlotX;

        if (navigateRight == false && navigateLeft == false) return;

        _infoBox.Flip();
    }


    // UI Control
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

    public void Update_PageArrows()
    {
        _pageArrowDirections.SetActive(Current_IVehicleMenu().ItemSlot_Datas().Count > 1);
        
        ItemSlot cursorSlot = _slotsController.cursor.currentSlot;
        List<ItemSlot> itemSlots = _slotsController.itemSlots;

        foreach (Image arrow in _pageArrows)
        {
            arrow.gameObject.SetActive(false);
        }

        if (cursorSlot == itemSlots[0])
        {
            _pageArrows[0].gameObject.SetActive(true);
            return;
        }

        if (cursorSlot != itemSlots[itemSlots.Count - 1]) return;

        _pageArrows[1].gameObject.SetActive(true);
    }


    private void Update_NavigateText()
    {
        InfoTemplate_Trigger template = gameObject.GetComponent<InfoTemplate_Trigger>();
        
        string navigateString = template.TemplateString(0) + "    " + template.TemplateString(1);
        template.setText.text = navigateString;
    }
}