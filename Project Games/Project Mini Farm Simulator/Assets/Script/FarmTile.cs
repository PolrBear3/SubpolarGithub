using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AfterSeedStatus
{
    public int health;
    
    public int dayPassed;
    public int fullGrownDay;

    public int watered;
    public int daysWithoutWater;
    public bool currentDayWatered;
}

[System.Serializable]
public class FarmTile_Default_Sprite
{
    public Sprite lockedTile;
    public Sprite unplantedTile;
}

public class FarmTile : MonoBehaviour
{
    MainGame_Controller controller;

    public Status_Icon_Indicator statusIconIndicator;

    [HideInInspector]
    public Image image;

    public int tileNum, tilePrice;
    public bool tileLocked = false, seedPlanted = false;

    public Seed_ScrObj plantedSeed = null;
    public Buff_ScrObj selectedBuff = null;

    public FarmTile_Default_Sprite farmTileDefaultSprite;
    public AfterSeedStatus tileSeedStatus;

    public void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Controller>();
        image = GetComponent<Image>();
    }

    public void Lock_Tile()
    {
        tileLocked = true;

        // locked tile image and status indicator
        image.sprite = farmTileDefaultSprite.lockedTile;
        statusIconIndicator.gameObject.SetActive(false);
    }
    public void Unlock_Tile()
    {
        tileLocked = false;

        // unlocked tile image and status indicator
        image.sprite = farmTileDefaultSprite.unplantedTile;
        statusIconIndicator.gameObject.SetActive(true);
    }

    public void Highlight_Tile()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void UnHighlight_Tile()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
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

    // seed plant start system 
    public void Seed_Planted_Start_Set()
    {
        tileSeedStatus.health = plantedSeed.seedHealth;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.currentDayWatered = false;
        tileSeedStatus.fullGrownDay = Random.Range(plantedSeed.minFinishDays, plantedSeed.maxFinishDays);

        controller.eventSystem.All_Events_Update_Check();
    }
    
    // seed plant update system
    public void Watering_Check()
    {
        // watering check
        if (!tileSeedStatus.currentDayWatered)
        {
            tileSeedStatus.daysWithoutWater += 1;
        }
        else if (tileSeedStatus.currentDayWatered)
        {
            tileSeedStatus.daysWithoutWater = 0;
        }

        // watering fail health 0
        if (tileSeedStatus.daysWithoutWater >= plantedSeed.waterHealth)
        {
            tileSeedStatus.health = 0;
        }
    }
    private void Health_Check()
    {
        if (tileSeedStatus.health <= 0)
        {
            image.sprite = farmTileDefaultSprite.unplantedTile;
            plantedSeed = null;
            seedPlanted = false;
            tileSeedStatus.dayPassed = 0;
            tileSeedStatus.watered = 0;
            tileSeedStatus.daysWithoutWater = 0;
            controller.Reset_All_Menu();
            controller.Reset_All_Tile_Highlights();
        }
    }

    public void NextDay_Seed_Status_Update()
    {
        tileSeedStatus.currentDayWatered = false;
        statusIconIndicator.Reset_All_Icons();

        if (seedPlanted)
        {
            Watering_Check();
            Health_Check();

            // reset next day
            tileSeedStatus.dayPassed += 1;

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

    // pulbic systems
    public void TileSprite_Update_Check()
    {
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
    public void Reset_Tile()
    {
        statusIconIndicator.Reset_All_Icons();

        image.sprite = farmTileDefaultSprite.unplantedTile;
        plantedSeed = null;
        selectedBuff = null;
        seedPlanted = false;
        tileSeedStatus.health = 0;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        tileSeedStatus.currentDayWatered = false;
        controller.Reset_All_Menu();
        controller.unPlantedMenu.Open();
        Highlight_Tile();
    }
}
