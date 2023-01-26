using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Options_Menu_Data
{
    public bool menuOn = false;
}

[System.Serializable]
public class Options_Menu_UI
{
    public RectTransform optionsPanel, bottomPanel;
    public GameObject[] menuButtonIcons;
    
    public Sprite[] buttonImages;
    public Options_Menu_TopButton[] topButtons;

    public GameObject[] settingsPages;
    public GameObject[] bottomSettingsPages;
}

[System.Serializable]
public class Options_Menu_TopButton
{
    public Button button;
    public Image buttonImage;
    public RectTransform buttonIcon;
    public Image buttonIconImage;
}

public class Options_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public Resolution_Controller resController;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Options_Menu_Data data;
    public Options_Menu_UI ui;

    private void Start()
    {
        BottomPanel_Center_Position();
    }

    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    // main option panel
    private void Open()
    {
        if (controller.tutorial == null)
        {
            controller.Reset_All_Menu();
        }

        Button_Shield(false);

        // default settings page
        Open_GameSettings();

        data.menuOn = true;
        LeanTween.move(ui.optionsPanel, new Vector2(0f, 62.50972f), 0.75f).setEase(tweenType);
        LeanTween.move(ui.bottomPanel, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
        ui.menuButtonIcons[0].SetActive(false);
        ui.menuButtonIcons[1].SetActive(true);

        // button shield on for nextday button
        controller.defaultMenu.menuUI.nextDayButton.enabled = false;

        // hide and activate button shield for all farmtiles
        controller.Hide_All_Tiles(true);
    }
    public void Close()
    {
        Button_Shield(true);

        data.menuOn = false;
        LeanTween.move(ui.optionsPanel, new Vector2(-500f, 62.50972f), 0.75f).setEase(tweenType);
        LeanTween.move(ui.bottomPanel, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
        ui.menuButtonIcons[0].SetActive(true);
        ui.menuButtonIcons[1].SetActive(false);

        // button shield on for nextday button
        controller.defaultMenu.menuUI.nextDayButton.enabled = true;

        // show and disactivate button shield for all farmtiles
        controller.Hide_All_Tiles(false);
    }
    public void Open_Close()
    {
        if (!data.menuOn) { Open(); }
        else { Close(); }
    }

    // bottom options panel
    private void BottomPanel_Center_Position()
    {
        ui.bottomPanel.anchoredPosition = new Vector2(0f, -125f);
    }

    // top main buttons
    private void Unpress_All_TopButtons()
    {
        for (int i = 0; i < ui.topButtons.Length; i++)
        {
            ui.topButtons[i].button.enabled = true;
            ui.topButtons[i].buttonImage.sprite = ui.buttonImages[0];
            ui.topButtons[i].buttonIcon.anchoredPosition = new Vector2(0f, 3.2f);
        }
    }
    private void Press_TopButton(int buttonNum)
    {
        Unpress_All_TopButtons();
        ui.topButtons[buttonNum].button.enabled = false;
        ui.topButtons[buttonNum].buttonImage.sprite = ui.buttonImages[1];
        ui.topButtons[buttonNum].buttonIcon.anchoredPosition = new Vector2(0f, -3.2f);
    }

    private void Reset_All_MenuPages()
    {
        for (int i = 0; i < ui.settingsPages.Length; i++)
        {
            ui.settingsPages[i].SetActive(false);
            ui.bottomSettingsPages[i].SetActive(false);
        }
    }
    private void Open_MenuPage(int menuPageNum)
    {
        Reset_All_MenuPages();
        ui.settingsPages[menuPageNum].SetActive(true);
        ui.bottomSettingsPages[menuPageNum].SetActive(true);
    }

    public void Open_GameSettings()
    {
        Press_TopButton(0);
        Open_MenuPage(0);
    }
    public void Open_HowtoPlay()
    {
        Press_TopButton(1);
        Open_MenuPage(1);
    }
    public void Open_AllSeedBuffInfo()
    {
        Press_TopButton(2);
        Open_MenuPage(2);
    }
    public void Open_AllStatusInfo()
    {
        Press_TopButton(3);
        Open_MenuPage(3);
    }
}
