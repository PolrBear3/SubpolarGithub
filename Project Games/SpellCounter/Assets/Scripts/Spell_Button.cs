using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Spell_Button_Data
{
    [HideInInspector]
    public bool hasSpell;
    public bool selected;
    public bool onCool;
    public float coolTime;
    public Spell_ScrObj currentSpell;
}

[System.Serializable]
public class Spell_Button_UI
{
    public Image frame;
    public Image spellImage;
    public TextMeshProUGUI coolTime;

    public Sprite defaultFrame;
    public Sprite selectFrame;

    public GameObject spellPanel;
}

public class Spell_Button : MonoBehaviour
{
    public Position_Controller controller;
    public Spell_Button_Data data;
    public Spell_Button_UI ui;

    private void Update()
    {
        CoolTime_Update();
    }

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
        Reset_CoolTime();

        data.hasSpell = true;
        data.currentSpell = spell;
        ui.spellImage.sprite = spell.sprite;

        UnSelect();
    }
    public void Clear_Spell()
    {
        data.hasSpell = false;
        data.currentSpell = null;
        ui.spellImage.sprite = ui.defaultFrame;
        ui.coolTime.enabled = false;
    }

    // cool time functions
    private void Set_CoolTime()
    {
        // default
        if (!controller.data.hasRune && !controller.data.hasBoots)
        {
            data.coolTime = data.currentSpell.defaultCoolTime;
        }
        // rune
        else if (controller.data.hasRune && !controller.data.hasBoots)
        {
            data.coolTime = data.currentSpell.runeCoolTime;
        }
        // boots
        else if (!controller.data.hasRune && controller.data.hasBoots)
        {
            data.coolTime = data.currentSpell.bootsCoolTime;
        }
        // both
        else if (controller.data.hasRune && controller.data.hasBoots)
        {
            data.coolTime = data.currentSpell.bothCoolTime;
        }
    }
    private void CoolTime_Update()
    {
        if (data.onCool)
        {
            data.coolTime -= Time.deltaTime;
            ui.coolTime.text = data.coolTime.ToString("0");
        }

        if (data.coolTime <= 0)
        {
            data.onCool = false;
            ui.coolTime.enabled = false;
        }
    }

    public void Run_CoolTime()
    {
        if (data.hasSpell)
        {
            Set_CoolTime();
            ui.coolTime.enabled = true;
            data.onCool = true;
        }
    }
    public void Reset_CoolTime()
    {
        ui.coolTime.enabled = false;
        data.onCool = false;
    }
}
