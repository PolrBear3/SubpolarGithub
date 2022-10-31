using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CollectableRoom_Menu_UI
{
    [HideInInspector]
    public bool menuOn = false;
    public RectTransform collectableFramesPanel, collectableRoomMenu;
}

[System.Serializable]
public class CollectableRoom_Menu_Data
{
    public bool placeMode = false;
    public Collectable_ScrObj selectedCollectable;
}

[System.Serializable]
public class Collectable
{
    public Collectable_ScrObj collectable;
    public bool unLocked = false;
    public int currentAmount;
}

[System.Serializable]
public class Collectable_Tier_Data
{
    public Collectable_Rare_level colorLevel;
    public float tierPercentage;
    public Sprite colorButtonFrameSprite;
    public Sprite colorFrameFrameSprite;
    public AnimatorOverrideController fireworks;
}

[System.Serializable]
public class CollectableRoom_Menu_ButtonPage
{
    public List<Collectable_Button> buttonPage;
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
    private List<Collectable_Button> currentButtonPage;

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
    private void Center_Position()
    {
        ui.collectableRoomMenu.anchoredPosition = new Vector2(0f, -125f);
    }
    
    private void AllFrameButton_ButtonShield_On()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].Button_Shield(true);
        }
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].Button_Shield(true);
        }
    }
    private void AllFrameButton_ButtonShield_Off()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].Button_Shield(false);
        }
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].Button_Shield(false);
        }
    }

    private void Open()
    {
        ui.menuOn = true;
        // close all menus that are opened
        controller.Reset_All_Menu();
        // buttons available
        Button_Shield(false);
        AllFrameButton_ButtonShield_Off();
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
        UnlockCheck_CurrentButtonPage();
        // button available check from amount
        AllButton_Select_Available_Check();
        // update amount text
        AllButton_Amount_Text_Update();
        // button frame tier color set
        AllButton_Frame_Tier_Update();
        // frame tier color set
        AllFrame_Frame_Tier_Update();
    }
    public void Close()
    {
        ui.menuOn = false;
        // buttons unavailable
        Button_Shield(true);
        AllFrameButton_ButtonShield_On();
        // lean tween collectableFramesPanel
        LeanTween.move(ui.collectableFramesPanel, new Vector2(360.04f, 62.50972f), 0.75f).setEase(tweenType);
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
        Reset_Collectable_Selection();
        // reset frame button availability
        AllFrame_PlaceMode_Off();
    }
    public void Open_Close()
    {
        // close if menu is open, open if menu is closed
        if (!ui.menuOn) { Open(); }
        else { Close(); }
    }

    // distinctive functions
    private void Set_Start_CurrentButtonPage()
    {
        currentButtonPage = allButtonPages[0].buttonPage;
    }
    public void Set_New_CurrentButtonPage()
    {
        Reset_Collectable_Selection();

        // set new current button page
        currentButtonPage = allButtonPages[pageController.currentPageNum - 1].buttonPage;
        // collectables unlock check
        UnlockCheck_CurrentButtonPage();
        // amount check
        AllButton_Select_Available_Check();
        // update amount text
        AllButton_Amount_Text_Update();
        // button frame tier color set
        AllButton_Frame_Tier_Update();
    }

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

    public void AllButton_UnSelect()
    {
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            if (currentButtonPage[i].data.buttonPressed)
            {
                currentButtonPage[i].UnSelect_Collectable();
            }
        }
    }
    public void AllButton_Select_Available_Check()
    {
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].Select_Available_Check();
        }
    }
    public void AllButton_Amount_Text_Update()
    {
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].Amount_Text_Update();
        }
    }
    public void AllButton_Frame_Tier_Update()
    {
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].Frame_Tier_Update();
        }
    }

    public void Reset_Collectable_Selection()
    {
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].UnSelect_Collectable();
        }
    }
    public void UnlockCheck_CurrentButtonPage()
    {
        for (int i = 0; i < currentButtonPage.Count; i++)
        {
            currentButtonPage[i].Unlock_Check();
        }
    }
}