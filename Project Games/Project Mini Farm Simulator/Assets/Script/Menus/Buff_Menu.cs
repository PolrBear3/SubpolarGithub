using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Buff_Menu_UI
{
    public Image buffPreview;
}

[System.Serializable]
public class Buff_Menu_Buff_ToolTip
{
    public bool toolTipOn = false;
    public GameObject toolTipPanel;
    public Animator toolTipAnimator;
    public Image previewBuffSprite;
    public Text buffName, buffDescription, buffPrice;
}

public class Buff_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;

    public Buff_ScrObj currentSelectedBuff;

    public GameObject[] confirmButtons;
    public Buff_Menu_UI ui;
    public Buff_Menu_Buff_ToolTip tooltipUI;
    public Button[] allAvailableButtons;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        // set to position at the center
        rectTransform.anchoredPosition = new Vector2(0f, -125f);
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
        Hide_Buff_ToolTip();
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

    private void ConfirmButton_Availability()
    {
        if (currentSelectedBuff != null && controller.money >= currentSelectedBuff.buffPrice)
        {
            confirmButtons[0].SetActive(false);
            confirmButtons[1].SetActive(true);
        }
        else
        {
            confirmButtons[0].SetActive(true);
            confirmButtons[1].SetActive(false);
        }
    }
    private void Reset_Selections()
    {
        currentSelectedBuff = null;
        ui.buffPreview.color = Color.clear;
        tooltipUI.toolTipAnimator.SetBool("buffSelected", false);
        ConfirmButton_Availability();
    }
    public void Select_Buff(Buff_ScrObj buffInfo)
    {
        currentSelectedBuff = buffInfo;
        ui.buffPreview.sprite = buffInfo.sprite;
        ui.buffPreview.color = Color.white;
        tooltipUI.toolTipAnimator.SetBool("buffSelected", true);
        ConfirmButton_Availability();
    }

    private void Buff_Price_Calculation()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].data.tileNum)
            {
                Confirm_Close();
                controller.Subtract_Money(currentSelectedBuff.buffPrice);
                controller.farmTiles[i].Add_Buff(currentSelectedBuff);
                Reset_Selections();
                controller.plantedMenu.CurrentBuffs_Button_Check();
                break;
            }
        }
    }
    public void Confirm_Buff()
    {
        Buff_Price_Calculation();
    }

    // tooltip functions
    private void Show_Buff_ToolTip()
    {
        var x = tooltipUI;

        if (currentSelectedBuff != null)
        {
            x.toolTipOn = true;
            x.previewBuffSprite.sprite = currentSelectedBuff.sprite;
            x.buffName.text = currentSelectedBuff.buffName;
            x.buffDescription.text = currentSelectedBuff.description;
            x.buffPrice.text = "buff price: $ " + currentSelectedBuff.buffPrice.ToString();
        }
        x.toolTipPanel.SetActive(true);
    }
    public void Hide_Buff_ToolTip()
    {
        var x = tooltipUI;

        x.toolTipOn = false;
        x.toolTipPanel.SetActive(false);
    }

    public void Show_Hide_ToolTip()
    {
        if (!tooltipUI.toolTipOn)
        {
            Show_Buff_ToolTip();
        }
        else
        {
            Hide_Buff_ToolTip();
        }
    }
}