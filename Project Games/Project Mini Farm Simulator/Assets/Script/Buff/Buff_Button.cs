using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_Button : MonoBehaviour
{
    public Buff_Menu menu;
    public Buff_ScrObj buffInfo;

    private bool buttonPressed;

    public Image buttonImage;
    public Button button, frameButton;
    public Sprite[] buttonSprites;

    public Image buffImage;
    public Text buffPriceText;

    public void Button_Shield(bool activate)
    {
        if (activate) { button.enabled = false; frameButton.enabled = false; }
        else if (!activate) { button.enabled = true; frameButton.enabled = true; }
    }

    public void Set_Buff_Info()
    {
        buffImage.sprite = buffInfo.sprite;
        buffPriceText.text = "$ " + buffInfo.buffPrice.ToString();
    }
    public void Select_This_Buff()
    {
        if (!buttonPressed)
        {
            menu.Unpress_All_Buttons();
            buttonPressed = true;

            buttonImage.sprite = buttonSprites[1];

            menu.Hide_Buff_ToolTip();
            menu.Select_Buff(buffInfo);
        }
        else
        {
            UnPress_This_Button();
        }
    }
    public void UnPress_This_Button()
    {
        buttonPressed = false;
        buttonImage.sprite = buttonSprites[0];

        menu.Reset_Selections();
    }
}
