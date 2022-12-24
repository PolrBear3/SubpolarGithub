using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seed_Button : MonoBehaviour
{
    public UnPlanted_Menu menu;
    public Seed_ScrObj seedInfo;

    private bool buttonPressed;

    public Image buttonImage;
    public Button button, frameButton;
    public Sprite[] buttonSprites;

    public Image seedImage;
    public Text seedPriceText;
    public RectTransform seedPriceTextRT;

    public void Button_Shield(bool activate)
    {
        if (activate) { button.enabled = false; frameButton.enabled = false; }
        else if (!activate) { button.enabled = true; frameButton.enabled = true; }
    }

    public void Set_Seed_Info()
    {
        seedImage.sprite = seedInfo.sprites[3];
        seedPriceText.text = "$ " + seedInfo.seedBuyPrice.ToString();
    }
    public void Select_This_Seed()
    {
        if (!buttonPressed)
        {
            menu.Unpress_All_Buttons();
            buttonPressed = true;

            buttonImage.sprite = buttonSprites[1];
            seedPriceTextRT.anchoredPosition = new Vector2(0f, -2.65f);

            menu.Hide_Seed_ToolTip();
            menu.Select_Seed(seedInfo);
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
        seedPriceTextRT.anchoredPosition = new Vector2(0f, 3.58f);

        menu.Reset_Selections();
    }
}