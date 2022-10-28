using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Shop_Menu_Data
{
    public bool menuOn;
}

[System.Serializable]
public class Shop_Menu_UI
{
    public RectTransform shopMenuPanel;
    public Animator anim;
    public GameObject rollAnimFrame, stopAnimFrame;
    public Text gachaPriceText;
}

public class Shop_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Shop_Menu_Data data;
    public Shop_Menu_UI ui;

    private void Start()
    {
        Center_Position();
        Set_GachaPrice_Text();
    }

    // basic ui functions
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
        ui.shopMenuPanel.anchoredPosition = new Vector2(-252.6f, 300f);
    }
    private void Open()
    {
        data.menuOn = true;
        Button_Shield(false);
        LeanTween.move(ui.shopMenuPanel, new Vector2(-107.44f, 300f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        data.menuOn = false;
        Button_Shield(true);
        LeanTween.move(ui.shopMenuPanel, new Vector2(-252.6f, 300f), 0.75f).setEase(tweenType);
    }
    public void Open_Close()
    {
        if (data.menuOn)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Set_GachaPrice_Text()
    {
        var x = controller.gachaSystem.gachaPrice;
        ui.gachaPriceText.text = "$ " + x.ToString();
    }

    // distinctive functions
    public void Roll_Frame()
    {
        ui.anim.SetBool("gachaPressed", false);
        ui.rollAnimFrame.SetActive(true);
        ui.stopAnimFrame.SetActive(false);
        // buy gacha button enabled true
    }
    public void Stop_Roll_Frame()
    {
        ui.rollAnimFrame.SetActive(false);
        ui.stopAnimFrame.SetActive(true);
        // buy gacha button enabled false

        // firework anim
        // +1 text lean tween effect
    }
}
