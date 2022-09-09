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
    public Button[] allAvailableButtons;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        Button_Shield(false);
        Seed_Information_Update();
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        Button_Shield(true);
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    private void HarvestButton_Available()
    {
        harvestButtons[1].SetActive(true);
        harvestButtons[0].SetActive(false);
    }
    private void HarvestButton_UnAvailable()
    {
        harvestButtons[1].SetActive(false);
        harvestButtons[0].SetActive(true);
    }

    private void Seed_Information_Update()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        plantedDayPassed.text = currentFarmTile.tileSeedStatus.dayPassed.ToString();
        plantedSeedImage.sprite = currentFarmTile.plantedSeed.sprites[3];

        if (currentFarmTile.tileSeedStatus.dayPassed >= currentFarmTile.tileSeedStatus.fullGrownDay)
        {
            HarvestButton_Available();
        }
        else
        {
            HarvestButton_UnAvailable();
        }
    }

    public void Water_Seed()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        if (!currentFarmTile.tileSeedStatus.currentDayWatered)
        {
            currentFarmTile.tileSeedStatus.currentDayWatered = true;
            currentFarmTile.tileSeedStatus.watered += 1;
            currentFarmTile.statusIconIndicator.Assign_Status(StatusType.watered);

            // if the seed is fully grown to crop, watering doesn't count
            if (currentFarmTile.tileSeedStatus.dayPassed >= currentFarmTile.tileSeedStatus.fullGrownDay)
            {
                currentFarmTile.tileSeedStatus.watered -= 1;
            }
        }
    }

    private int Harvest_Bonus_Check()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum].tileSeedStatus;

        // tier 1
        if (currentFarmTile.watered >= currentFarmTile.fullGrownDay)
        {
            return 3;
        }
        // tier 2
        else if (currentFarmTile.watered >= currentFarmTile.fullGrownDay * 0.75f)
        {
            return 2;
        }
        // tier 3
        else if (currentFarmTile.watered >= currentFarmTile.fullGrownDay * 0.5f)
        {
            return 1;
        }
        // tier 4
        else return 0;
    }
    public void Harvest_Sell()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];

        // calculate total
        if (Harvest_Bonus_Check() > 0)
        {
            controller.Add_Money_withBonus(currentFarmTile.plantedSeed.harvestSellPrice, Harvest_Bonus_Check());
        }
        else if (Harvest_Bonus_Check() == 0)
        {
            controller.Add_Money(currentFarmTile.plantedSeed.harvestSellPrice);
        }
        currentFarmTile.Reset_Tile();
    }
}
