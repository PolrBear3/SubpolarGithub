using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seed_Button : MonoBehaviour
{
    public UnPlanted_Menu menu;
    public Seed_ScrObj seedInfo;

    public Image seedImage;
    public Text seedPriceText;

    private void Awake()
    {
        Set_Seed_Info();
    }

    private void Set_Seed_Info()
    {
        seedImage.sprite = seedInfo.sprites[3];
        seedPriceText.text = "$ " + seedInfo.seedBuyPrice.ToString();
    }

    public void Plant_This_Seed()
    {
        menu.Plant_Seed(seedInfo);
    }
}
