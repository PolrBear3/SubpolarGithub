using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Spell_Button_Data
{
    [HideInInspector]
    public bool hasSpell;
    public bool selected;
    public Spell_ScrObj currentSpell;
}

[System.Serializable]
public class Spell_Button_UI
{
    public Image frame;
    public Image spellImage;

    public Sprite defaultFrame;
    public Sprite selectFrame;

    public GameObject spellPanel;
}

public class Spell_Button : MonoBehaviour
{
    public Position_Controller controller;
    public Spell_Button_Data data;
    public Spell_Button_UI ui;

    // basic select and deselect functions
    public void Select()
    {
        controller.mainController.UnSelect_All_SpellButtons();
        data.selected = true;
        ui.frame.sprite = ui.selectFrame;

        ui.spellPanel.SetActive(true);
    }
    public void UnSelect()
    {
        data.selected = false;
        ui.frame.sprite = ui.defaultFrame;

        ui.spellPanel.SetActive(false);
    }
    public void Click()
    {
        if (data.selected)
        {
            UnSelect();
        }
        else
        {
            Select();
        }
    }

    // spell setting functions
    public void Select_Spell(Spell_ScrObj spell)
    {
        data.hasSpell = true;
        data.currentSpell = spell;
        ui.spellImage.sprite = spell.sprite;

        UnSelect();
    }
    public void Clear_Spell()
    {
        data.hasSpell = false;
        data.currentSpell = null;
        ui.spellImage.sprite = null;
    }
}
