using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Shop_Menu_Data
{
    public bool shopPanelOn = false;
}

[System.Serializable]
public class Shop_Menu_UI
{
    public RectTransform shopPanel;
    public Animator collectableRandomAnim;
    public Text gachaPriceText;
}

public class Shop_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    public LeanTweenType tweenType;

    public Shop_Menu_Data data;
    public Shop_Menu_UI ui;

    private void Start()
    {
        Center_Position();
        Set_Gacha_PriceText();
    }

    // basic ui options
    private void Center_Position()
    {
        ui.shopPanel.anchoredPosition = new Vector2(-251.56f, 308f);
    }

    private void Open()
    {
        data.shopPanelOn = true;
        LeanTween.move(ui.shopPanel, new Vector2(-108.48f, 308f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        data.shopPanelOn = false;
        LeanTween.move(ui.shopPanel, new Vector2(-251.56f, 308f), 0.75f).setEase(tweenType);
    }
    public void Open_Close()
    {
        if (data.shopPanelOn)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Set_Gacha_PriceText()
    {
        var x = controller.gachaSystem.gachaPrice;
        ui.gachaPriceText.text = "$ " + x.ToString();
    }
}
