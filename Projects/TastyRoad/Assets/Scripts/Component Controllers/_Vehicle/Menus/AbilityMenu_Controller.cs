using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _controller;

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
        _controller.Update_PanelSprite(_panelSprite);

        _controller.slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        _controller.Update_PageDots(_currentDatas.Count, _currentPageNum);

        // subscriptions
        _controller.On_MenuToggle += Update_CursorFill;
        _controller.On_MenuToggle += Update_AbilityIcons;
        _controller.On_MenuToggle += Show_AbilityDiscription;

        _controller.OnCursor_Input += Show_AbilityDiscription;

        _controller.OnCursor_OuterInput += Clamp_CursorPosition;
        _controller.OnCursor_YInput += Update_CurrentPage;

        _controller.OnHoldSelect_Input += ActivateAbility_onSelect;
        _controller.OnHoldSelect_Input += Show_AbilityDiscription;
    }

    private void OnDisable()
    {
        _controller.Update_PanelSprite(null);

        // subscriptions
        _controller.On_MenuToggle -= Update_CursorFill;
        _controller.On_MenuToggle -= Update_AbilityIcons;
        _controller.On_MenuToggle -= Show_AbilityDiscription;

        _controller.OnCursor_Input -= Show_AbilityDiscription;

        _controller.OnCursor_OuterInput -= Clamp_CursorPosition;
        _controller.OnCursor_YInput -= Update_CurrentPage;

        _controller.OnHoldSelect_Input -= ActivateAbility_onSelect;
        _controller.OnHoldSelect_Input -= Show_AbilityDiscription;
    }


    // IVehicleMenu
    public void Start_Menu()
    {

    }


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
        _controller.slotsController.AddNewPage_ItemSlotDatas(_currentDatas);
    }


    // Data Set Functions
    private void Clamp_CursorPosition() // outer input
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        int lastSlotNum = slotsController.itemSlots.Count - 1;
        float cursorGridNum = cursor.currentSlot.gridNum.x;

        bool nextSlots = false;

        if (cursorGridNum == 0)
        {
            cursor.Navigate_toSlot(slotsController.ItemSlot(new(lastSlotNum, 0f)));
        }
        else if (cursorGridNum == lastSlotNum)
        {
            nextSlots = true;
            cursor.Navigate_toSlot(slotsController.ItemSlot(new(0f, 0f)));
        }

        if (_currentDatas.Count <= 1) return;

        int direction = nextSlots ? 1 : -1;
        Update_CurrentPage(direction);
    }

    private void Update_PageNum(float direction)
    {
        if (direction == 1)
        {
            // next slots
            _currentPageNum = (_currentPageNum + 1) % _currentDatas.Count;
            return;
        }

        // previous slots
        _currentPageNum = (_currentPageNum - 1 + _currentDatas.Count) % _currentDatas.Count;
    }

    private void Update_CurrentPage(float yInputValue) // y input
    {
        if (_currentDatas.Count <= 1) return;

        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        // save current slots data to current page data, before moving on to next page
        _currentDatas[_currentPageNum] = new(slotsController.CurrentSlots_toDatas());

        Update_PageNum(yInputValue);

        // load data to slots
        slotsController.Set_Datas(_currentDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();

        // indicator
        _controller.Update_PageDots(_currentDatas.Count, _currentPageNum);
    }


    private void Update_AbilityIcons(bool menuToggle)
    {
        if (menuToggle == false) return;

        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        ItemSlots_Controller slotsController = _controller.slotsController;

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
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;

        if (manager.Ability_ActivateMaxed(CurrentSlot_Ability())) return null;

        if (manager.AbilityPoint_Maxed()) return "\n\nHold <sprite=15> to level up";
        return "\n\n" + manager.currentAbilityPoint + "/" + manager.maxAbilityPoint + "  <sprite=79> points";
    }


    private void Show_AbilityDiscription(bool menuToggle)
    {
        if (menuToggle == false) return;
        if (gameObject.activeSelf == false) return;

        InformationBox infoBox = _controller.infoBox;
        Ability_ScrObj currentAbility = CurrentSlot_Ability();

        if (currentAbility == null)
        {
            infoBox.gameObject.SetActive(false);
            return;
        }

        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        string activationCount = manager.Ability_ActivateCount(currentAbility) + "/" + currentAbility.maxActivationCount;

        infoBox.Update_InfoText(activationCount + "\n\n" + currentAbility.description + HoldSelect_InfoText());

        infoBox.gameObject.SetActive(true);
        infoBox.Update_RectLayout();
    }
    private void Show_AbilityDiscription()
    {
        Show_AbilityDiscription(gameObject.activeSelf);
    }


    private void Update_CursorFill(bool menuToggle)
    {
        if (menuToggle == false) return;
        if (gameObject.activeSelf == false) return;

        AbilityManager manager = Main_Controller.instance.Player().abilityManager;

        if (manager.AbilityPoint_Maxed())
        {
            _controller.Update_MenuCursorSprite(_cursorFillSprites[_cursorFillSprites.Length - 1]);
            return;
        }

        int currentIndex = manager.currentAbilityPoint * (_cursorFillSprites.Length - 1) / manager.maxAbilityPoint;
        int spriteIndex = Mathf.Clamp(currentIndex, 0, _cursorFillSprites.Length - 2);

        _controller.Update_MenuCursorSprite(_cursorFillSprites[spriteIndex]);
    }
    private void Update_CursorFill()
    {
        Update_CursorFill(gameObject.activeSelf);
    }


    // Activation
    private Ability_ScrObj CurrentSlot_Ability()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.currentSlot == null) return null;
        int currentSlotNum = (int)cursor.currentSlot.gridNum.x;

        if (currentSlotNum > manager.allAbilities.Length - 1) return null;
        return manager.allAbilities[currentSlotNum].abilityScrObj;
    }

    private void ActivateAbility_onSelect()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        if (manager.AbilityPoint_Maxed() == false) return;

        Ability_ScrObj currentAbility = CurrentSlot_Ability();
        if (currentAbility == null) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (manager.Ability_ActivateMaxed(currentAbility))
        {
            // current ability is fully leveled up!
            dialog.Update_Dialog(0);

            return;
        }

        manager.Activate_Ability(CurrentSlot_Ability());

        Update_CursorFill(true);
        Update_AbilityIcons(true);

        // dialog
        int activationCount = manager.Ability_ActivateCount(currentAbility);
        Sprite abilitySprite = currentAbility.ProgressIcon(activationCount);

        string abilityInfo = currentAbility.abilityName + "\n\n";
        string activationInfo = manager.Ability_ActivateCount(currentAbility) + "/" + currentAbility.maxActivationCount + " ";

        DialogData dialogData = new(abilitySprite, abilityInfo + activationInfo + "leveled up");
        dialog.Update_Dialog(dialogData);
    }
}
