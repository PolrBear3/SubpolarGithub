using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArchiveMenu_Controller : MonoBehaviour, IVehicleMenu, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private VehicleMenu_Controller _controller;
    public VehicleMenu_Controller controller => _controller;

    [Space(20)]
    [SerializeField] private Sprite _panelSprite;

    [Space(20)]
    [SerializeField] private RectTransform _ingredientBox;
    [SerializeField] private FoodCondition_Indicator[] _indicators;

    [Space(20)]
    [SerializeField] private GameObject _foodOrderIndicator;
    [SerializeField] private Clock_Timer _foodOrderTimer;

    [Space(20)] 
    [SerializeField] private Sprite _maxUnlockIcon;
    [SerializeField][Range(0, 500)] private int _maxUnlockAmount;

    [Space(20)] 
    [SerializeField] private Sprite _maxTransferIcon;
    [SerializeField][Range(0, 500)] private int _maxTransferAmount;
    
    [Space(80)]
    [SerializeField] private Guide_ScrObj _guideScrObj;
    

    private ArchiveMenu_Data _data;
    private int _currentPageNum;

    private List<NPC_FoodInteractionData> _foodInteractionDatas = new();
    public List<NPC_FoodInteractionData> foodInteractionDatas => _foodInteractionDatas;
    

    // UnityEngine
    private void OnEnable()
    {
        VideoGuide_Controller.instance.Trigger_Guide(_guideScrObj);
        
        _controller.slotsController.Set_Datas(_data.slotDatas[_currentPageNum]);

        _controller.Update_PanelSprite(_panelSprite);

        _controller.Update_PageDots(_data.slotDatas.Count, _currentPageNum);
        _controller.Update_PageArrows();

        // subscriptions
        _controller.On_MenuToggle += Update_CurrentDatas;
        _controller.On_MenuToggle += Update_MaterialShineSlots;

        _controller.OnCursor_OuterInput += Clamp_CursorPosition;
        _controller.OnCursor_YInput += Update_CurrentPage;

        _controller.OnSelect_Input += Select_Slot;
        _controller.OnHoldSelect_Input += CurrentFood_BookmarkToggle;
        _controller.OnOption1_Input += CurrentFood_BookmarkToggle;

        _controller.OnCursor_Input += Update_InfoBox;
        _controller.OnSelect_Input += Update_InfoBox;
        _controller.OnOption1_Input += Update_InfoBox;
        _controller.OnOption2_Input += Update_InfoBox;

        _controller.OnOption2_Input += Toggle_IngredientBox;
        _controller.OnSelect_Input += Hide_IngredientBox;
        _controller.OnCursor_DirectionInput += Hide_IngredientBox;
        _controller.OnExit_Input += Hide_IngredientBox;

        Localization_Controller.instance.OnLanguageChanged += Update_InfoBox;
    }

    private void OnDisable()
    {
        _controller.Update_PanelSprite(null);

        // save current showing slots contents to _currentDatas
        Drag_Cancel();
        _data.slotDatas[_currentPageNum] = _controller.slotsController.CurrentSlots_toDatas();

        // subscriptions
        _controller.On_MenuToggle -= Update_CurrentDatas;
        _controller.On_MenuToggle -= Update_MaterialShineSlots;

        _controller.OnCursor_OuterInput -= Clamp_CursorPosition;
        _controller.OnCursor_YInput -= Update_CurrentPage;

        _controller.OnSelect_Input -= Select_Slot;
        _controller.OnHoldSelect_Input -= CurrentFood_BookmarkToggle;
        _controller.OnOption1_Input -= CurrentFood_BookmarkToggle;

        _controller.OnCursor_Input -= Update_InfoBox;
        _controller.OnSelect_Input -= Update_InfoBox;
        _controller.OnOption1_Input -= Update_InfoBox;
        _controller.OnOption2_Input -= Update_InfoBox;

        _controller.OnOption2_Input -= Toggle_IngredientBox;
        _controller.OnSelect_Input -= Hide_IngredientBox;
        _controller.OnCursor_DirectionInput -= Hide_IngredientBox;
        _controller.OnExit_Input -= Hide_IngredientBox;
        
        Localization_Controller.instance.OnLanguageChanged -= Update_InfoBox;
    }

    private void OnDestroy()
    {
        OnDisable();
        
        // subscriptions
        ActionBubble_Interactable interactable = _controller.vehicleController;

        interactable.OnInteract -= Toggle_OrderIndicator;
        interactable.OnUnInteract -= Toggle_OrderIndicator;
        
        Main_Controller.instance.OnFoodBookmark -= Toggle_OrderIndicator;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("ArchiveMenu_Controller/ArchiveMenu_Data", _data);
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("ArchiveMenu_Controller/ArchiveMenu_Data") == false)
        {
            _data = new();
            _controller.slotsController.AddNewPage_ItemSlotDatas(_data.slotDatas);

            return;
        }
        _data = ES3.Load("ArchiveMenu_Controller/ArchiveMenu_Data", new ArchiveMenu_Data());
    }
    
    
    // IBackupLoadable
    public bool Has_Conflict()
    {
        if (_data.slotDatas != null) return false;
        return true;
    }

    public void Load_Backup()
    {
        _controller.slotsController.AddNewPage_ItemSlotDatas(_data.slotDatas);
    }


    // IVehicleMenu
    public void Start_Menu()
    {
        Toggle_OrderIndicator();
        
        // subscriptions
        ActionBubble_Interactable interactable = _controller.vehicleController;

        interactable.OnInteract += Toggle_OrderIndicator;
        interactable.OnUnInteract += Toggle_OrderIndicator;
        
        Main_Controller.instance.OnFoodBookmark += Toggle_OrderIndicator;
    }


    public bool MenuInteraction_Active()
    {
        return _ingredientBox.gameObject.activeSelf;
    }

    public Dictionary<int, List<ItemSlot_Data>> ItemSlot_Datas()
    {
        return _data.slotDatas;
    }


    // Slot and Cursor Control
    private void Select_Slot()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;

        if (cursor.data.hasItem == false)
        {
            Drag_Food();
            return;
        }

        if (cursor.currentSlot.data.hasItem)
        {
            Swap_Food();
            return;
        }

        Drop_Food();
    }
    
    public void Update_InfoBox()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;
        ItemSlot_Data slotData = cursor.currentSlot.data;

        if (!cursorData.hasItem) return;

        InformationBox info = _controller.infoBox;
        InfoTemplate_Trigger infoTrigger = info.templateTrigger;
        InfoTemplate_Trigger archiveInfoTrigger = gameObject.GetComponent<InfoTemplate_Trigger>();

        // Bookmark Lock Status
        string bookmarkStatus = infoTrigger.TemplateString(4);
        string lockStatus = null;

        if (slotData.hasItem)
        {
            bookmarkStatus = infoTrigger.TemplateString(6);
        }

        if (cursorData.isLocked)
        {
            lockStatus = archiveInfoTrigger.TemplateString(0) + "\n\n";
        }
        else if (!slotData.hasItem)
        {
            if (cursorData.bookMarked)
            {
                bookmarkStatus = "<sprite=68> " + infoTrigger.TemplateString(8);
            }
            else
            {
                bookmarkStatus = "<sprite=68>";
            }
        }

        Food_ScrObj dragFood = cursorData.currentFood;

        // ingredient available status
        string ingredientStatus = archiveInfoTrigger.TemplateString(1);
        if (FoodIngredient_Unlocked(dragFood) == false)
        {
            ingredientStatus = infoTrigger.TemplateString(4);
        }
        
        // recent npc food interaction data
        NPC_FoodInteractionData recentData = FoodInteraction_Data(dragFood);
        string orderCountString = recentData != null ? "<sprite=107> " + recentData.orderFoodData.currentAmount.ToString() + "  " : null;
        string serveCountString = recentData != null ? "<sprite=108> " + recentData.serveCount.ToString() + "  " : null;
        string paymentString = recentData != null ? "<sprite=56> " + recentData.goldCount.ToString() + "\n\n" : null;
        string recentDataString = recentData != null ? orderCountString + serveCountString + paymentString : null;

        string dragInfo = "<sprite=69> " + dragFood.LocalizedName() + "\n\n";
        string unlockCount = "<sprite=88> " + FoodIngredient_UnlockCount(dragFood) + "/" + _maxUnlockAmount + "\n";
        string transferCount = "<sprite=106> " + IngredientUnlocked_FoodData(dragFood).tikCount + "/" + _maxTransferAmount + "\n\n";
        string controlInfo = infoTrigger.KeyControl_Template(bookmarkStatus, ingredientStatus, bookmarkStatus);

        info.Update_InfoText(dragInfo + unlockCount + transferCount + recentDataString + lockStatus + controlInfo);
    }


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

        if (_data.slotDatas.Count <= 1) return;

        int direction = nextSlots ? 1 : -1;
        Update_CurrentPage(direction);
    }

    private void Update_PageNum(float direction)
    {
        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        
        if (direction == 1)
        {
            // next slots
            _currentPageNum = (_currentPageNum + 1) % slotDatas.Count;
            return;
        }

        // previous slots
        _currentPageNum = (_currentPageNum - 1 + slotDatas.Count) % slotDatas.Count;
    }

    private void Update_CurrentPage(float yInputValue) // y input
    {
        if (_data.slotDatas.Count <= 1) return;

        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;
        
        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;

        // save current slots data to current page data, before moving on to next page
        slotDatas[_currentPageNum] = new(slotsController.CurrentSlots_toDatas());

        Update_PageNum(yInputValue);

        // load data to slots
        slotsController.Set_Datas(slotDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();

        // indicator
        _controller.Update_PageDots(slotDatas.Count, _currentPageNum);
    }


    private void Drag_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data slotData = new(currentSlot.data);
        currentSlot.Empty_ItemBox();

        cursor.Assign_Data(slotData);
        cursor.Update_SlotIcon();

        Update_MaterialShineSlot(currentSlot);
        
        Audio_Controller.instance.Play_OneShot(_controller.gameObject, 1);
    }

    private void Drag_Cancel()
    {
        ItemSlots_Controller slotsController = _controller.slotsController;
        ItemSlot_Cursor cursor = slotsController.cursor;

        if (cursor.data.hasItem == false) return;

        // save drag data
        ItemSlot_Data cursorData = new(cursor.data);
        cursor.Empty_Item();

        ItemSlot emptySlot = slotsController.EmptySlot();

        // drop on empty slot
        if (emptySlot != null)
        {
            emptySlot.Assign_Data(cursorData);
            emptySlot.Update_SlotIcon();

            emptySlot.Toggle_BookMark(emptySlot.data.bookMarked);
            emptySlot.Toggle_Lock(emptySlot.data.isLocked);

            return;
        }

        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        
        // drop on empty data
        for (int i = 0; i < slotDatas.Count; i++)
        {
            for (int j = 0; j < slotDatas[i].Count; j++)
            {
                if (slotDatas[i][j].hasItem == true) continue;

                slotDatas[i][j] = new(cursorData);
                return;
            }
        }

        slotsController.Set_Datas(slotDatas[_currentPageNum]);
        slotsController.SlotsAssign_Update();
    }


    private void Drop_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;

        ItemSlot_Data cursorData = new(cursor.data);
        cursor.Empty_Item();

        currentSlot.Assign_Data(cursorData);
        currentSlot.Update_SlotIcon();

        currentSlot.Toggle_BookMark(currentSlot.data.bookMarked);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);

        Update_MaterialShineSlot(currentSlot);
        
        Audio_Controller.instance.Play_OneShot(_controller.gameObject, 2);
    }

    private void Swap_Food()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot currentSlot = cursor.currentSlot;
        
        ItemSlot_Data slotData = new(cursor.currentSlot.data);
        ItemSlot_Data cursorData = new(cursor.data);

        currentSlot.Assign_Data(cursorData);
        currentSlot.Update_SlotIcon();
        
        currentSlot.Toggle_BookMark(currentSlot.data.bookMarked);
        currentSlot.Toggle_Lock(currentSlot.data.isLocked);

        Update_MaterialShineSlot(currentSlot);
        
        cursor.Assign_Data(slotData);
        cursor.Update_SlotIcon();
        
        Audio_Controller.instance.Play_OneShot(_controller.gameObject, 1);
    }


    private void CurrentFood_BookmarkToggle()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;
        
        if (cursorData.hasItem == false) return;
        Hide_IngredientBox();
        
        ItemSlot currentSlot = cursor.currentSlot;
        if (currentSlot.data.hasItem)
        {
            Swap_Food();
            return;
        }

        Drop_Food();

        if (currentSlot.data.isLocked == true)
        {
            currentSlot.Toggle_BookMark(false);
            _controller.infoBox.gameObject.SetActive(false);
            return;
        }
        
        currentSlot.Toggle_BookMark(!currentSlot.data.bookMarked);

        // main data update
        Main_Controller main = Main_Controller.instance;

        if (currentSlot.data.bookMarked == false)
        {
            main.Remove_Bookmark(currentSlot.data.currentFood);
            return;
        }

        main.Add_Bookmark(currentSlot.data.currentFood);
        
        // reset recent data
        _foodInteractionDatas.Remove(FoodInteraction_Data(currentSlot.data.currentFood));
    }

    private void Toggle_OrderIndicator()
    {
        bool bubbleOn = _controller.vehicleController.bubble.bubbleOn;
        bool foodBookMarked = Main_Controller.instance.data.bookmarkedFoods.Count > 0;
        
        _foodOrderIndicator.SetActive(foodBookMarked && bubbleOn == false);
        _foodOrderTimer.Toggle_RunAnimation(foodBookMarked&& bubbleOn == false);
    }
    

    // Archive Data
    public ItemSlot_Data Archived_FoodData(Food_ScrObj targetFood)
    {
        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        
        for (int i = 0; i < slotDatas.Count; i++)
        {
            for (int j = 0; j < slotDatas[i].Count; j++)
            {
                if (slotDatas[i][j].hasItem == false) continue;
                if (targetFood != slotDatas[i][j].currentFood) continue;

                return slotDatas[i][j];
            }
        }
        return null;
    }

    public bool Food_Archived(Food_ScrObj food)
    {
        return _controller.slotsController.FoodAmount(_data.slotDatas, food) > 0;
    }


    private void RemoveDuplicate_ArchivedFood(Food_ScrObj food)
    {
        if (Food_Archived(food) == false) return;
        
        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        int amountCount = 0;
        
        for (int i = 0; i < slotDatas.Count; i++)
        {
            for (int j = 0; j < slotDatas[i].Count; j++)
            {
                if (slotDatas[i][j].hasItem == false) continue;
                if (slotDatas[i][j].currentFood != food) continue;
                amountCount++;

                if (amountCount <= 1) continue;
                slotDatas[i][j].Empty_Item();
            }
        }
    }

    private void Update_CurrentDatas(bool menuToggle)
    {
        if (!menuToggle) return;
        
        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        
        for (int i = 0; i < slotDatas.Count; i++)
        {
            for (int j = 0; j < slotDatas[i].Count; j++)
            {
                if (!slotDatas[i][j].hasItem) continue;
                
                Food_ScrObj dataFood = slotDatas[i][j].foodData.foodScrObj;
                RemoveDuplicate_ArchivedFood(dataFood);
            }
        }
        
        _controller.Update_ItemSlots(gameObject, slotDatas[_currentPageNum]);
    }


    private void AddNewPage_onFull()
    {
        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        
        for (int i = 0; i < slotDatas.Count; i++)
        {
            for (int j = 0; j < slotDatas[i].Count; j++)
            {
                if (slotDatas[i][j].hasItem) continue;
                return;
            }
        }

        _controller.slotsController.AddNewPage_ItemSlotDatas(slotDatas);
    }


    public ItemSlot_Data Archive_Food(Food_ScrObj food)
    {
        if (food == null) return null;

        // check if food has ingredients
        if (food.ingredients.Count <= 0) return null;

        // check if non duplicate food
        if (Food_Archived(food)) return Archived_FoodData(food);

        AddNewPage_onFull();

        Dictionary<int, List<ItemSlot_Data>> slotDatas = _data.slotDatas;
        
        for (int i = 0; i < slotDatas.Count; i++)
        {
            for (int j = 0; j < slotDatas[i].Count; j++)
            {
                if (slotDatas[i][j].hasItem == true) continue;

                FoodData archiveData = new(food);
                slotDatas[i][j] = new(archiveData);

                // lock toggle according to cooked food
                Data_Controller dataController = Main_Controller.instance.dataController;
                slotDatas[i][j].isLocked = !dataController.CookedFood(food);

                _controller.Update_ItemSlots(gameObject, slotDatas[_currentPageNum]);

                return slotDatas[i][j];
            }
        }

        return null;
    }

    public ItemSlot_Data Unlock_BookmarkToggle(ItemSlot_Data toggleData, bool toggle)
    {
        if (toggleData == null) return null;

        toggleData.isLocked = !toggle;
        _controller.Update_ItemSlots(gameObject, _data.slotDatas[_currentPageNum]);

        return toggleData;
    }

    
    // NPC Food Interaction
    public NPC_FoodInteractionData FoodInteraction_Data(Food_ScrObj interactFood)
    {
        for (int i = 0; i < _foodInteractionDatas.Count; i++)
        {
            if (interactFood != _foodInteractionDatas[i].orderFoodData.foodScrObj) continue;
            return _foodInteractionDatas[i];
        }
        return null;
    }

    public void Update_OrderCount(Food_ScrObj orderFood)
    {
        NPC_FoodInteractionData orderFoodData = FoodInteraction_Data(orderFood);
        
        if (orderFoodData == null)
        {
            _foodInteractionDatas.Add(new(orderFood));
            return;
        }
        orderFoodData.orderFoodData.Update_Amount(1);
    }
    
    
    // Ingredient Data
    private int MaxUnlock_IngredientCount()
    {
        List<FoodData> unlockedIngredients = _data.unlockedIngredients;
        int maxCount = 0;

        for (int i = 0; i < unlockedIngredients.Count; i++)
        {
            if (unlockedIngredients[i].currentAmount < _maxUnlockAmount) continue;
            maxCount++;
        }
        
        return maxCount;
    }
    
    private FoodData IngredientUnlocked_FoodData(Food_ScrObj food)
    {
        List<FoodData> unlockedIngredients = _data.unlockedIngredients;
        
        for (int i = 0; i < unlockedIngredients.Count; i++)
        {
            if (food != unlockedIngredients[i].foodScrObj) continue;
            return unlockedIngredients[i];
        }
        return null;
    }

    public bool FoodIngredient_Unlocked(Food_ScrObj food)
    {
        List<FoodData> unlockedIngredients = _data.unlockedIngredients;
        
        for (int i = 0; i < unlockedIngredients.Count; i++)
        {
            if (food != unlockedIngredients[i].foodScrObj) continue;
            return true;
        }
        return false;
    }

    public int FoodIngredient_UnlockCount(Food_ScrObj food)
    {
        FoodData targetData = IngredientUnlocked_FoodData(food);

        if (targetData == null) return 0;
        return targetData.currentAmount;
    }


    public void Unlock_FoodIngredient(Food_ScrObj food, int unlockAmount)
    {
        if (food == null) return;

        List<FoodData> unlockedIngredients = _data.unlockedIngredients;
        
        if (FoodIngredient_Unlocked(food) == false)
        {
            _data.unlockedIngredients.Add(new(food, unlockAmount));
            _controller.Update_ItemSlots(gameObject, _data.slotDatas[_currentPageNum]);

            Update_CurrentDatas(true);
            return;
        }

        if (unlockAmount <= 0) return;

        int unlockCount = FoodIngredient_UnlockCount(food);
        int setAmount = Mathf.Clamp(unlockCount + unlockAmount, 1, _maxUnlockAmount);

        IngredientUnlocked_FoodData(food).Set_Amount(setAmount);
        
        // tutorial quest
        TutorialQuest_Controller tutorialQuest = TutorialQuest_Controller.instance;
        TutorialQuest targetQuest = tutorialQuest.CurrentQuest("CookCountMax");
        
        int questCount = targetQuest.currentCompleteCount;
        tutorialQuest.Complete_Quest(targetQuest, MaxUnlock_IngredientCount() - questCount);
    }

    public void Update_FoodTransferCount(Food_ScrObj food, int updateValue)
    {
        if (FoodIngredient_Unlocked(food) == false) return;
        FoodData targetData = IngredientUnlocked_FoodData(food);

        if (targetData.tikCount >= _maxTransferAmount) return;
        IngredientUnlocked_FoodData(food).Update_TikCount(updateValue);
    }

    
    // Slot Item Indication Effects
    private void Update_MaterialShineSlot(ItemSlot targetSlot)
    {
        ItemSlot_Data data = targetSlot.data;
        bool unlocked = FoodIngredient_UnlockCount(data.currentFood) >= _maxUnlockAmount;

        targetSlot.Toggle_MaterialShine(data.hasItem && unlocked);
    }

    private void Update_MaterialShineSlots(bool menuToggle)
    {
        if (menuToggle == false) return;

        ItemSlots_Controller slotsController = _controller.slotsController;
        List<ItemSlot> itemSlots = slotsController.itemSlots;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            Update_MaterialShineSlot(itemSlots[i]);
        }
    }


    // Ingredient Box Control
    private void Toggle_IngredientBox()
    {
        ItemSlot_Data cursorData = _controller.slotsController.cursor.data;

        if (cursorData.hasItem == false) return;
        if (FoodIngredient_Unlocked(cursorData.currentFood) == false)
        {
            Drag_Cancel();
            return;
        }

        if (_ingredientBox.gameObject.activeSelf == false)
        {
            Show_IngredientBox();
            return;
        }

        Hide_IngredientBox();
    }


    private void Show_IngredientBox()
    {
        if (_ingredientBox.gameObject.activeSelf) return;

        InformationBox infoBox = _controller.infoBox;
        _controller.infoBox.gameObject.SetActive(false);
        
        _ingredientBox.transform.SetParent(infoBox.transform);
        _ingredientBox.anchorMin = _ingredientBox.anchorMax = infoBox.flipped ? new(1f, 1f) : new(0f, 1f);
        _ingredientBox.pivot = infoBox.flipped ? new(1f, 1f) : new(0f, 1f);
        _ingredientBox.anchoredPosition = Vector2.zero;
        
        _ingredientBox.transform.SetParent(transform);
        _ingredientBox.gameObject.SetActive(true);

        Update_IngredientBox();
    }

    private void Update_IngredientBox()
    {
        ItemSlot_Cursor cursor = _controller.slotsController.cursor;
        ItemSlot_Data cursorData = cursor.data;

        if (cursorData.hasItem == false) return;

        Food_ScrObj cursorFood = cursorData.currentFood;
        int ingredientAmount = cursorData.currentFood.ingredients.Count;

        for (int i = 0; i < _indicators.Length; i++)
        {
            if (ingredientAmount <= 0)
            {
                _indicators[i].Clear();
                continue;
            }

            _indicators[i].Indicate(cursorFood.ingredients[i]);
            ingredientAmount--;
        }
    }


    private void Hide_IngredientBox()
    {
        if (_ingredientBox.gameObject.activeSelf == false) return;

        _controller.infoBox.gameObject.SetActive(true);
        _ingredientBox.gameObject.SetActive(false);
    }
    private void Hide_IngredientBox(Vector2 nullDirection)
    {
        Hide_IngredientBox();
    }
}