using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AfterSeedStatus
{
    public int dayPassed;
    public int fullGrownDay;
    public int watered;
    public int daysWithoutWater;
    public bool currentDayWatered;
}

public class FarmTile : MonoBehaviour
{
    MainGame_Controller controller;

    [HideInInspector]
    public Image image;

    public int tileNum, tilePrice;
    public bool tileLocked = false, seedPlanted = false;
    public Sprite[] defaultTileSprites;

    public Seed_ScrObj plantedSeed = null;
    public AfterSeedStatus tileSeedStatus;

    public void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Controller>();
        image = GetComponent<Image>();
    }

    public void Lock_Tile()
    {
        tileLocked = true;

        // locked tile image
        image.sprite = defaultTileSprites[0];
    }
    public void Unlock_Tile()
    {
        tileLocked = false;

        image.sprite = defaultTileSprites[1];
    }

    public void Highlight_Tile()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void UnHighlight_Tile()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Open_Menu(FarmTile farmTile)
    {
        // detecting which tile was pressed
        controller.Set_OpenTileNum(farmTile);

        // reset 
        controller.Reset_All_Menu();
        controller.Reset_All_Tile_Highlights();
        
        // highlight pressed tile
        Highlight_Tile();
        
        if (tileLocked)
        {
            controller.lockedMenu.Open();
        }
        else if (!seedPlanted)
        {
            controller.unPlantedMenu.Open();
        }
        else if (seedPlanted)
        {
            controller.plantedMenu.Open();
        }
    }

    // after seed plant system 
    public void Seed_Planted_Start_Set()
    {
        tileSeedStatus.fullGrownDay = Random.Range(plantedSeed.minFinishDays, plantedSeed.maxFinishDays);
        tileSeedStatus.dayPassed = 0;
    }
    public void Seed_Planted_Status_Update()
    {
        if (seedPlanted)
        {
            Watering_Check();

            // reset next day
            tileSeedStatus.dayPassed += 1;
            tileSeedStatus.currentDayWatered = false;
            
            // half grown complete check
            if (tileSeedStatus.dayPassed >= tileSeedStatus.fullGrownDay / 2)
            {
                image.sprite = plantedSeed.sprites[1];
            }

            // full grown complete check
            if (tileSeedStatus.dayPassed >= tileSeedStatus.fullGrownDay)
            {
                image.sprite = plantedSeed.sprites[2];
            }
        }
    }
    private void Watering_Check()
    {
        if (!tileSeedStatus.currentDayWatered)
        {
            tileSeedStatus.daysWithoutWater += 1;
        }
        else if (tileSeedStatus.currentDayWatered)
        {
            tileSeedStatus.daysWithoutWater = 0;
        }

        // 2 days without water
        if (tileSeedStatus.daysWithoutWater >= 3)
        {
            tileSeedStatus.dayPassed -= 1;
        }

        // 3 days without water
        if (tileSeedStatus.daysWithoutWater >= 4)
        {
            image.sprite = defaultTileSprites[1];
            plantedSeed = null;
            seedPlanted = false;
            tileSeedStatus.dayPassed = 0;
            tileSeedStatus.watered = 0;
            tileSeedStatus.daysWithoutWater = 0;
            controller.Reset_All_Menu();
            controller.Reset_All_Tile_Highlights();
        }
    }

    // pulbic system
    public void Reset_Tile()
    {
        image.sprite = defaultTileSprites[1];
        plantedSeed = null;
        seedPlanted = false;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        controller.Reset_All_Menu();
        controller.unPlantedMenu.Open();
        Highlight_Tile();
    }
}
