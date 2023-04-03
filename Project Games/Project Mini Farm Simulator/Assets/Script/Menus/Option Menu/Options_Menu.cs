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
    public RectTransform optionsPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;
}

public class Options_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public Resolution_Controller resController;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Options_Menu_Data data;
    public Options_Menu_UI ui;

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

        data.menuOn = true;
        LeanTween.move(ui.optionsPanel, new Vector2(0f, 62.50972f), 0.75f).setEase(tweenType);

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

    // volume adjust
    public void Adjust_BGM_Volume()
    {
        controller.soundController.BGM_Volume_Control(ui.bgmSlider.value);
    }
    public void Adjust_SFX_Volume()
    {
        controller.soundController.SFX_Volume_Control(ui.sfxSlider.value);
    }

    // volume slider adjust
    public void Adjust_BGM_Volume_Slider(float value)
    {
        ui.bgmSlider.value = value;
    }
    public void Adjust_SFX_Volume_Slider(float value)
    {
        ui.sfxSlider.value = value;
    }

    // mute option
    public void Mute_BGM_Volume()
    {
        Adjust_BGM_Volume_Slider(0.0001f);
        Adjust_BGM_Volume();
    }
    public void Mute_SFX_Volume()
    {
        Adjust_SFX_Volume_Slider(0.0001f);
        Adjust_SFX_Volume();
    }
}
