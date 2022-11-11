using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Options_Menu_Data
{
    [HideInInspector]
    public bool menuOn = false;
}

[System.Serializable]
public class Options_Menu_UI
{
    public RectTransform optionsPanel;
    public GameObject[] menuButtonIcons;
}

public class Options_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Options_Menu_Data data;
    public Options_Menu_UI ui;

    private void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    private void Open()
    {
        controller.Reset_All_Menu();
        Button_Shield(false);

        data.menuOn = true;
        LeanTween.move(ui.optionsPanel, new Vector2(0f, 62.50972f), 0.75f).setEase(tweenType);
        ui.menuButtonIcons[0].SetActive(false);
        ui.menuButtonIcons[1].SetActive(true);
    }
    public void Close()
    {
        Button_Shield(true);

        data.menuOn = false;
        LeanTween.move(ui.optionsPanel, new Vector2(-500f, 62.50972f), 0.75f).setEase(tweenType);
        ui.menuButtonIcons[0].SetActive(true);
        ui.menuButtonIcons[1].SetActive(false);
    }
    public void Open_Close()
    {
        if (!data.menuOn) { Open(); }
        else { Close(); }
    }
}
