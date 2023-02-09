using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Position_Controller_Data
{
    public bool hasRune = false;
    public bool hasBoots = false;
}

[System.Serializable]
public class Position_Controller_UI
{
    public Image runeImage, runeFrame;
    public Image bootsImage, bootsFrame;

    public Sprite defaultFrame;
    public Sprite selectFrame;

    public Sprite UnSelectedRune;
    public Sprite SelectedRune;

    public Sprite UnSelectedBoots;
    public Sprite SelectedBoots;
}


public class Position_Controller : MonoBehaviour
{
    public Controller mainController;
    public Position_Controller_Data data;
    public Position_Controller_UI ui;

    public Spell_Button buttonD;
    public Spell_Button buttonF;

    public void Rune_On_Off()
    {
        if (!data.hasRune)
        {
            data.hasRune = true;
            ui.runeFrame.sprite = ui.selectFrame;
            ui.runeImage.sprite = ui.SelectedRune;
        }
        else
        {
            data.hasRune = false;
            ui.runeFrame.sprite = ui.defaultFrame;
            ui.runeImage.sprite = ui.UnSelectedRune;
        }
    }
    public void Boots_On_Off()
    {
        if (!data.hasBoots)
        {
            data.hasBoots = true;
            ui.bootsFrame.sprite = ui.selectFrame;
            ui.bootsImage.sprite = ui.SelectedBoots;
        }
        else
        {
            data.hasBoots = false;
            ui.bootsFrame.sprite = ui.defaultFrame;
            ui.bootsImage.sprite = ui.UnSelectedBoots;

        }
    }
}
