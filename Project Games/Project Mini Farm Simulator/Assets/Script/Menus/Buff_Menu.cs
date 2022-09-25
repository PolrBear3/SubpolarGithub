using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Buff_Menu_UI
{
    public Image buffPreview;
}

public class Buff_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;

    public Buff_ScrObj currentSelectedBuff;
    public Buff_Menu_UI ui;
    public Button[] allAvailableButtons;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        Reset_Selections();
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
        Button_Shield(false);
    }
    public void Close()
    {
        Reset_Selections();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
        Button_Shield(true);
        controller.plantedMenu.Button_Shield(false);
    }
    private void Confirm_Close()
    {
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
        Button_Shield(true);
        controller.plantedMenu.Button_Shield(false);
    }

    private void Reset_Selections()
    {
        currentSelectedBuff = null;
        ui.buffPreview.color = Color.clear;
    }
    public void Select_Buff(Buff_ScrObj buffInfo)
    {
        currentSelectedBuff = buffInfo;
        ui.buffPreview.sprite = buffInfo.sprite;
        ui.buffPreview.color = Color.white;
    }

    private void Buff_Price_Calculation()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].data.tileNum)
            {
                if (controller.money >= currentSelectedBuff.buffPrice)
                {
                    Confirm_Close();
                    controller.Subtract_Money(currentSelectedBuff.buffPrice);
                    controller.farmTiles[i].currentBuffs.Add(currentSelectedBuff);
                    Reset_Selections();
                    break;
                }
                else
                {
                    controller.defaultMenu.NotEnoughMoney_Blink_Tween();
                }
            }
        }
    }
    public void Confirm_Buff()
    {
        if (currentSelectedBuff != null)
        {
            Buff_Price_Calculation();
        }
    }
}
