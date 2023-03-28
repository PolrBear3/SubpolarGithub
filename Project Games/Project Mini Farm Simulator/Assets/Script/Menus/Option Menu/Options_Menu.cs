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
}
