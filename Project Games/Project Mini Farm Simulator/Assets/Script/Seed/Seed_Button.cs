using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seed_Button : MonoBehaviour
{
    public UnPlanted_Menu menu;
    public Seed_ScrObj seedInfo;

    public Image seedImage;
    public Button button, frameButton;
    public Text seedPriceText;

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
        menu.hide_Seed_ToolTip();
        menu.Select_Seed(seedInfo);
    }
}
