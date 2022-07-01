using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnPlanted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Open()
    {
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }
    public void PlantSeed_Close()
    {
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    public void Plant_Seed(Seed_ScrObj seedtype)
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].tileNum)
            {
                controller.farmTiles[i].image.sprite = seedtype.sprites[0];
                controller.farmTiles[i].seedPlanted = true;
                controller.farmTiles[i].plantedSeed = seedtype;
                controller.farmTiles[i].Seed_Planted_Start_Set();
                // set farm tile seed image to seed type image
                PlantSeed_Close();
                controller.plantedMenu.Open();
                break;
            }
        }
    }
}
