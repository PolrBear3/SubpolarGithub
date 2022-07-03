using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;

    public Image plantedSeedImage;
    public Text plantedDayPassed;

    public GameObject[] harvestButtons;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Open()
    {
        Seed_Information_Update();
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    private void Seed_Information_Update()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        plantedDayPassed.text = currentFarmTile.tileSeedStatus.dayPassed.ToString();
        plantedSeedImage.sprite = currentFarmTile.plantedSeed.sprites[1];

        if (currentFarmTile.tileSeedStatus.dayPassed >= currentFarmTile.tileSeedStatus.fullGrownDay)
        {
            harvestButtons[1].SetActive(true);
            harvestButtons[0].SetActive(false);
        }
        else
        {
            harvestButtons[1].SetActive(false);
            harvestButtons[0].SetActive(true);
        }
    }

    public void Harvest_Sell()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        controller.Add_Money(currentFarmTile.plantedSeed.harvestSellPrice);
        currentFarmTile.Harvest_Tile();
    }
}
