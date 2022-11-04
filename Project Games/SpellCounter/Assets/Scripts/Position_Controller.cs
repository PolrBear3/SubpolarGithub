using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Position_Controller_Data
{
    public bool hasRune;
    public bool hasBoots;
}

[System.Serializable]
public class Position_Controller_UI
{
    public Image runeFrame;
    public Image bootsFrame;

    public Sprite defaultFrame;
    public Sprite selectFrame;
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
        }
        else
        {
            data.hasRune = false;
            ui.runeFrame.sprite = ui.defaultFrame;
        }
    }
    public void Boots_On_Off()
    {
        if (!data.hasBoots)
        {
            data.hasBoots = true;
            ui.bootsFrame.sprite = ui.selectFrame;
        }
        else
        {
            data.hasBoots = false;
            ui.bootsFrame.sprite = ui.defaultFrame;
        }
    }
}
