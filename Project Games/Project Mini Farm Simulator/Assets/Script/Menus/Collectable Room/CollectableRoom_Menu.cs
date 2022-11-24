using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CollectableRoom_Menu_UI
{
    public GameObject[] menuButtonIcons;
    public RectTransform collectableFramesPanel, collectableRoomMenu;
    public Image findIcon;
}

[System.Serializable]
public class CollectableRoom_Menu_Data
{
    public bool menuOn = false;

    public bool placeMode = false;
    public Collectable_ScrObj selectedCollectable;

    public int sortNum;
    public Collectable_Rare_level sortMode;
}

[System.Serializable]
public class Collectable
{
    public Collectable_ScrObj collectable;
    public bool unLocked = false, isNew = true, goldModeAvailable = false;
    public int currentAmount, maxAmount;
}

[System.Serializable]
public class Collectable_Tier_Data
{
    public Collectable_Rare_level colorLevel;
    public float tierPercentage;
    public Sprite colorButtonFrameSprite;
    public Sprite colorFrameFrameSprite;
    public AnimatorOverrideController fireworks;
    public Sprite findIcon;
}

[System.Serializable]
public class CollectableRoom_Menu_ButtonPage
{
    public int pageNum;
    public List<Collectable_Button> buttons;
}

public class CollectableRoom_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public LeanTweenType tweenType;
    public Page_Controller pageController;
    public Button[] allAvailableButtons;

    public CollectableRoom_Menu_UI ui;
    public CollectableRoom_Menu_Data data;

    public Collectable_Frame[] allFrames;
    public CollectableRoom_Menu_ButtonPage[] allButtonPages;
    private List<Collectable_Button> currentButtons;

    public Collectable_Tier_Data[] allCollectableTierData;
    public Collectable[] allCollectables;

    private void Start()
    {
        Center_Position();
        Set_Start_CurrentButtonPage();
    }

    // basic functions
    private void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }
    private void Frames_CurrentButtons_Button_Shield(bool activate)
    {
        if (activate)
        {
            for (int i = 0; i < allFrames.Length; i++)
            {
                allFrames[i].Button_Shield(true);
            }
            for (int i = 0; i < currentButtons.Count; i++)
            {
                currentButtons[i].Button_Shield(true);
            }
        }
        else if (!activate)
        {
            for (int i = 0; i < allFrames.Length; i++)
            {
                allFrames[i].Button_Shield(false);
            }
            for (int i = 0; i < currentButtons.Count; i++)
            {
                currentButtons[i].Button_Shield(false);
            }
        }
    }
    private void Center_Position()
    {
        ui.collectableRoomMenu.anchoredPosition = new Vector2(0f, -125f);
    }

    private void Open()
    {
        // close all menus that are opened
        controller.Reset_All_Menu();

        data.menuOn = true;

        // default menu button icon
        ui.menuButtonIcons[0].SetActive(false);
        ui.menuButtonIcons[1].SetActive(true);

        // buttons available
        Button_Shield(false);
        Frames_CurrentButtons_Button_Shield(false);
        // lean tween collectableFramesPanel
        LeanTween.move(ui.collectableFramesPanel, new Vector2(0f, 62.50972f), 0.75f).setEase(tweenType);
        // lean tween collectableRoomMenu
        LeanTween.move(ui.collectableRoomMenu, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
        // button shield on for nextday button
        controller.defaultMenu.menuUI.nextDayButton.enabled = false;
        // turn off all farmtile status icons, button shield on for all farmtiles
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            controller.farmTiles[i].statusIconIndicator.gameObject.SetActive(false);
            controller.farmTiles[i].button.enabled = false;
        }
        // reset frame button availability
        AllFrame_PlaceMode_Off();
        // collectables unlock check
        AllButton_UnlockCheck();
        // all collectables isNew check
        AllButton_NewIcon_Check();
        // button available check from amount
        AllButton_Select_Available_Check();
        // update amount text
        AllButton_Amount_Text_Update();
        // button frame tier color set
        AllButton_Frame_Tier_Update();
        // frame tier color set
        AllFrame_Frame_Tier_Update();
        // reset find data
        Reset_Find();
    }
    public void Close()
    {
        data.menuOn = false;

        // default menu button icon
        ui.menuButtonIcons[0].SetActive(true);
        ui.menuButtonIcons[1].SetActive(false);

        // buttons unavailable
        Button_Shield(true);
        Frames_CurrentButtons_Button_Shield(true);
        // lean tween collectableFramesPanel
        LeanTween.move(ui.collectableFramesPanel, new Vector2(500f, 62.50972f), 0.75f).setEase(tweenType);
        // close shop menu
        controller.shopMenu.Close();
        // lean tween collectableRoomMenu
        LeanTween.move(ui.collectableRoomMenu, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
        // button shield off for nextday button
        controller.defaultMenu.menuUI.nextDayButton.enabled = true;
        // turn on all farmtile status icons, button shield off for all farmtiles
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            controller.farmTiles[i].statusIconIndicator.gameObject.SetActive(true);
            controller.farmTiles[i].button.enabled = true;
        }
        // all buttons set to unpressed
        AllButton_UnSelect();
        // reset frame button availability
        AllFrame_PlaceMode_Off();
    }
    public void Open_Close()
    {
        // close if menu is open, open if menu is closed
        if (!data.menuOn) { Open(); }
        else { Close(); }
    }

    // current button loop functions
    private void Set_Start_CurrentButtonPage()
    {
        currentButtons = allButtonPages[0].buttons;
    }
    public void Set_New_CurrentButtonPage()
    {
        AllButton_UnSelect();

        // set new current button page
        currentButtons = allButtonPages[pageController.currentPageNum - 1].buttons;
        // collectables unlock check
        AllButton_UnlockCheck();
        // all collectables isNew check
        AllButton_NewIcon_Check();
        // amount check
        AllButton_Select_Available_Check();
        // update amount text
        AllButton_Amount_Text_Update();
        // button frame tier color set
        AllButton_Frame_Tier_Update();
    }

    // all frame functions
    public void AllFrame_PlaceMode_On()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].PlaceMode_On();
        }

        data.placeMode = true;

        // close shop menu if it is open
        if (controller.shopMenu.data.menuOn)
        {
            controller.shopMenu.Close();
        }
    }
    public void AllFrame_PlaceMode_Off()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].PlaceMode_Off();
        }

        data.placeMode = false;
    }
    public void AllFrame_Load_FrameSprite()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].Load_FrameSprite();
        }
    }
    public void AllFrame_Frame_Tier_Update()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].Frame_Tier_Update();
        }
    }

    // current buttons functions
    public void AllButton_UnSelect()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            if (currentButtons[i].data.buttonPressed)
            {
                currentButtons[i].UnSelect_Collectable();
            }
        }
    }
    public void AllButton_Select_Available_Check()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].Select_Available_Check();
        }
    }
    public void AllButton_UnlockCheck()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].Unlock_Check();
        }
    }
    public void AllButton_NewIcon_Check()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].NewIcon_Check();
        }
    }
    public void AllButton_Amount_Text_Update()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].Amount_Text_Update();
        }
    }
    public void AllButton_Frame_Tier_Update()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].Frame_Tier_Update();
        }
    }

    // Find collectables by tier functions
    public void Reset_Find()
    {
        data.sortNum = 0;
        data.sortMode = Collectable_Rare_level.all;
        ui.findIcon.sprite = allCollectableTierData[data.sortNum].findIcon;
    }
    public void Find()
    {
        // settings
        data.sortNum++;
        if (data.sortNum > 4) { Reset_Find(); }
        data.sortMode = allCollectableTierData[data.sortNum].colorLevel;
        ui.findIcon.sprite = allCollectableTierData[data.sortNum].findIcon;

        // find function
        bool tierFound = false;
        while (!tierFound)
        {
            pageController.NextPage();
            Set_New_CurrentButtonPage();

            // search
            for (int i = 0; i < currentButtons.Count; i++)
            {
                if (currentButtons[i].data.thisCollectable.colorLevel == data.sortMode)
                {
                    tierFound = true;
                    break;
                }
            }

            // if search is complete or cant find
            if (tierFound || pageController.currentPageNum == pageController.pages.Length)
            {
                break;
            }
        }
    }
}