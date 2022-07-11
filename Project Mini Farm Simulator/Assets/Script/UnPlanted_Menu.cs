using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnPlanted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Button[] allSeedButtons;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Button_Shield(bool activate)
    {
        for (int i = 0; i < allSeedButtons.Length; i++)
        {
            if (activate) { allSeedButtons[i].enabled = false; }
            else if (!activate) { allSeedButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        Button_Shield(false);
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        Button_Shield(true);
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }
    public void PlantSeed_Close()
    {
        Button_Shield(true);
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    public void Plant_Seed(Seed_ScrObj seedtype)
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].tileNum)
            {
                if (controller.money >= seedtype.seedBuyPrice)
                {
                    PlantSeed_Close();
                    controller.Subtract_Money(seedtype.seedBuyPrice);
                    controller.farmTiles[i].image.sprite = seedtype.sprites[0];
                    controller.farmTiles[i].seedPlanted = true;
                    controller.farmTiles[i].plantedSeed = seedtype;
                    controller.farmTiles[i].Seed_Planted_Start_Set();
                    controller.plantedMenu.Open();
                    break;
                }
                else
                {
                    Debug.Log("Not Enough Money!");
                }
            }
        }
    }
}
