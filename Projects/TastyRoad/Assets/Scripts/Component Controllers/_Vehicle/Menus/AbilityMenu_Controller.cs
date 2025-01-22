using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _menuController;

    [Header("")]
    [SerializeField] private Sprite _panelSprite;

    [Header("")]
    [SerializeField] private Sprite[] _cursorFillSprites;


    private Dictionary<int, List<ItemSlot_Data>> _currentDatas = new();
    public Dictionary<int, List<ItemSlot_Data>> currentDatas => _currentDatas;

    private int _currentPageNum;


    // MonoBehaviour
    private void Start()
    {
        // subscriptions
        AbilityManager.OnPointIncrease += Update_CursorFill;
        AbilityManager.OnPointIncrease += Show_AbilityDiscription;
    }

    private void OnDestroy()
    {
        // subscriptions
        AbilityManager.OnPointIncrease -= Update_CursorFill;
        AbilityManager.OnPointIncrease -= Show_AbilityDiscription;
    }

    private void OnEnable()
    {
        _menuController.Update_PanelSprite(_panelSprite);

        _menuController.slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        _menuController.Update_PageDots(_currentDatas.Count, _currentPageNum);

        // subscriptions
        _menuController.MenuOpen_Event += Update_CursorFill;
        _menuController.MenuOpen_Event += Update_AbilityIcons;
        _menuController.MenuOpen_Event += Show_AbilityDiscription;

        _menuController.OnCursor_Input += Show_AbilityDiscription;
        _menuController.OnCursor_Outer += CurrentSlots_PageUpdate;

        _menuController.OnHoldEmptySelect_Input += ActivateAbility_onSelect;
        _menuController.OnHoldEmptySelect_Input += Show_AbilityDiscription;
    }

    private void OnDisable()
    {
        _menuController.Update_PanelSprite(null);

        // subscriptions
        _menuController.MenuOpen_Event -= Update_CursorFill;
        _menuController.MenuOpen_Event -= Update_AbilityIcons;
        _menuController.MenuOpen_Event -= Show_AbilityDiscription;

        _menuController.OnCursor_Input -= Show_AbilityDiscription;
        _menuController.OnCursor_Outer -= CurrentSlots_PageUpdate;

        _menuController.OnHoldEmptySelect_Input -= ActivateAbility_onSelect;
        _menuController.OnHoldEmptySelect_Input -= Show_AbilityDiscription;
    }


    // IVehicleMenu
    public bool MenuInteraction_Active()
    {
        return false;
    }

    public Dictionary<int, List<ItemSlot_Data>> ItemSlot_Datas()
    {
        return _currentDatas;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("AbilityMenu_Controller/_currentDatas", _currentDatas);
    }

    public void Load_Data()
    {
        // load saved slot datas
        if (ES3.KeyExists("AbilityMenu_Controller/_currentDatas"))
        {
            _currentDatas = ES3.Load("AbilityMenu_Controller/_currentDatas", _currentDatas);
            return;
        }

        // set new slot datas
        _menuController.slotsController.AddNewPage_ItemSlotDatas(_currentDatas);
    }


    // Data Set Functions
    private void CurrentSlots_PageUpdate()
    {
        ItemSlots_Controller slotsController = _menuController.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        // save current slots data to current page data, before moving on to next page
        _currentDatas[_currentPageNum] = new(slotsController.CurrentSlots_toDatas());

        int lastSlotNum = slotsController.itemSlots.Count - 1;

        // previous slots
        if (cursor.currentSlot.gridNum.x <= 0)
        {
            _currentPageNum = (_currentPageNum - 1 + _currentDatas.Count) % _currentDatas.Count;
            cursor.Navigate_toSlot(slotsController.ItemSlot(new(lastSlotNum, 0f)));
        }
        // next slots
        else if (cursor.currentSlot.gridNum.x >= lastSlotNum)
        {
            _currentPageNum = (_currentPageNum + 1) % _currentDatas.Count;
            cursor.Navigate_toSlot(slotsController.ItemSlot(new(0f, 0f)));
        }

        // load data to slots
        slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();

        Update_AbilityIcons();

        // indicator
        _menuController.Update_PageDots(_currentDatas.Count, _currentPageNum);
    }

    private void Update_AbilityIcons()
    {
        AbilityManager manager = _menuController.vehicleController.mainController.Player().abilityManager;
        ItemSlots_Controller slotsController = _menuController.slotsController;

        for (int i = 0; i < manager.allAbilities.Length; i++)
        {
            Ability_ScrObj targetAbility = manager.allAbilities[i].abilityScrObj;
            int activateCount = manager.Ability_ActivateCount(targetAbility);

            slotsController.itemSlots[i].Update_SlotIcon(targetAbility.ProgressIcon(activateCount));
        }
    }


    // Indications
    private string HoldSelect_InfoText()
    {
        AbilityManager manager = _menuController.vehicleController.mainController.Player().abilityManager;

        if (manager.AbilityPoint_Maxed()) return "Hold <sprite=15> to level up";
        return manager.currentAbilityPoint + "/" + manager.maxAbilityPoint + "\nNot enough <sprite=79> points!";
    }

    private void Show_AbilityDiscription()
    {
        InformationBox infoBox = _menuController.infoBox;
        Ability_ScrObj currentAbility = CurrentSlot_Ability();

        if (currentAbility == null)
        {
            infoBox.gameObject.SetActive(false);
            return;
        }

        infoBox.Update_InfoText(currentAbility.description + "\n\n" + HoldSelect_InfoText());

        infoBox.gameObject.SetActive(true);
        infoBox.Update_RectLayout();
    }


    private void Update_CursorFill()
    {
        if (gameObject.activeSelf == false) return;

        AbilityManager manager = _menuController.vehicleController.mainController.Player().abilityManager;

        if (manager.AbilityPoint_Maxed())
        {
            _menuController.Update_MenuCursorSprite(_cursorFillSprites[_cursorFillSprites.Length - 1]);
            return;
        }

        int currentIndex = manager.currentAbilityPoint * (_cursorFillSprites.Length - 1) / manager.maxAbilityPoint;
        int spriteIndex = Mathf.Clamp(currentIndex, 0, _cursorFillSprites.Length - 2);

        _menuController.Update_MenuCursorSprite(_cursorFillSprites[spriteIndex]);
    }


    // Activation
    private Ability_ScrObj CurrentSlot_Ability()
    {
        AbilityManager manager = _menuController.vehicleController.mainController.Player().abilityManager;
        ItemSlot_Cursor cursor = _menuController.slotsController.cursor;

        int currentSlotNum = (int)cursor.currentSlot.gridNum.x;

        if (currentSlotNum > manager.allAbilities.Length - 1) return null;
        return manager.allAbilities[currentSlotNum].abilityScrObj;
    }

    private void ActivateAbility_onSelect()
    {
        if (CurrentSlot_Ability() == null) return;

        AbilityManager manager = _menuController.vehicleController.mainController.Player().abilityManager;

        if (manager.AbilityPoint_Maxed() == false) return;

        manager.Activate_Ability(CurrentSlot_Ability());

        Update_CursorFill();
        Update_AbilityIcons();
    }
}
