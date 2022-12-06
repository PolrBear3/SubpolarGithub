using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public struct AfterSeedStatus
{
    public int health;
    
    public int dayPassed;
    public int fullGrownDay;

    public int watered;
    public int daysWithoutWater;
    public bool currentDayWatered;
    public bool harvestReady;

    public int bonusPoints;
}

[System.Serializable]
public class FarmTile_Basic_Data
{
    public Seed_ScrObj plantedSeed = null;
    public int tileNum, tilePrice;
    public bool tileSelected = false, tileLocked = false, seedPlanted = false;

    public Sprite lockedTile;
    public Sprite unplantedTile;
}

public class FarmTile : MonoBehaviour
{
    public MainGame_Controller controller;
    public Button button;
    public Status_Icon_Indicator statusIconIndicator;
    
    [HideInInspector]
    public Image image;

    public string saveName;
    public FarmTile_Basic_Data data;
    public AfterSeedStatus tileSeedStatus;

    public List<Buff_ScrObj> currentBuffs = new List<Buff_ScrObj>();

    public void Awake()
    {
        image = GetComponent<Image>();
    }

    // tile lock status
    public void Lock_Tile()
    {
        data.tileLocked = true;

        // locked tile image and status indicator
        image.sprite = data.lockedTile;
        statusIconIndicator.gameObject.SetActive(false);
    }
    public void Unlock_Tile()
    {
        data.tileLocked = false;

        // unlocked tile image and status indicator
        image.sprite = data.unplantedTile;
        statusIconIndicator.gameObject.SetActive(true);
    }
    private void Unlock_Check()
    {
        if (!data.tileLocked)
        {
            Unlock_Tile();
        }
        else
        {
            Lock_Tile();
        }
    }

    // tile selecting
    public void Highlight_Tile()
    {
        data.tileSelected = true;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void UnHighlight_Tile()
    {
        data.tileSelected = false;
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void Open_Menu(FarmTile farmTile)
    {
        // if the tile is already selected
        if (data.tileSelected)
        {
            UnHighlight_Tile();

            // close opened menu
            controller.Reset_All_Menu();
        }
        // if the tile is not selected
        else
        {
            // detecting which tile was pressed
            controller.Set_OpenTileNum(farmTile);

            // reset 
            controller.Reset_All_Menu();
            controller.Reset_All_Tile_Highlights();

            Highlight_Tile();

            if (data.tileLocked)
            {
                controller.tileBuyButton.Open();
            }
            else if (!data.seedPlanted)
            {
                controller.unPlantedMenu.Open();
            }
            else if (data.seedPlanted)
            {
                controller.plantedMenu.Open();
            }
        }
    }

    // seed plant start system 
    public void Seed_Planted_Start_Set()
    {
        tileSeedStatus.health = data.plantedSeed.seedHealth;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.currentDayWatered = false;
        tileSeedStatus.harvestReady = false;
        tileSeedStatus.bonusPoints = 0;
        tileSeedStatus.fullGrownDay = Random.Range(data.plantedSeed.minFinishDays, data.plantedSeed.maxFinishDays);

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
        if (tileSeedStatus.daysWithoutWater >= data.plantedSeed.waterHealth)
        {
            tileSeedStatus.health = 0;
        }
    }
    private void Health_Check()
    {
        if (tileSeedStatus.health <= 0)
        {
            image.sprite = data.unplantedTile;
            data.plantedSeed = null;
            data.seedPlanted = false;
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

        if (data.seedPlanted)
        {
            Watering_Check();
            Health_Check();

            // reset next day
            tileSeedStatus.dayPassed += 1;

            TileSprite_Update_Check();
        }
    }
    public void LoadDay_Seed_Status_Update()
    {
        if (data.seedPlanted)
        {
            TileSprite_Update_Check();
        }
    }

    // pulbic systems
    public void TileSprite_Update_Check()
    {
        if (data.seedPlanted)
        {
            // early stage of grow
            if (tileSeedStatus.dayPassed < tileSeedStatus.fullGrownDay / 2)
            {
                image.sprite = data.plantedSeed.sprites[0];
            }
            // half grown complete check
            if (tileSeedStatus.dayPassed >= tileSeedStatus.fullGrownDay / 2)
            {
                image.sprite = data.plantedSeed.sprites[1];
            }
            // full grown complete check
            if (tileSeedStatus.dayPassed >= tileSeedStatus.fullGrownDay)
            {
                image.sprite = data.plantedSeed.sprites[2];
                tileSeedStatus.harvestReady = true;
            }
        }
    }
    public void Reset_Tile()
    {
        statusIconIndicator.Reset_All_Icons();
        currentBuffs.Clear();

        image.sprite = data.unplantedTile;
        data.plantedSeed = null;
        data.seedPlanted = false;
        tileSeedStatus.health = 0;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        tileSeedStatus.currentDayWatered = false;
        tileSeedStatus.harvestReady = true;
        tileSeedStatus.bonusPoints = 0;
        
        controller.plantedMenu.Close();
        controller.unPlantedMenu.Open();
        Highlight_Tile();
    }

    public void Add_Buff(Buff_ScrObj buff)
    {
        currentBuffs.Add(buff);
    }
    public void Remove_Buff(Buff_ScrObj buff)
    {
        for (int i = 0; i < currentBuffs.Count; i++)
        {
            if (buff == currentBuffs[i])
            {
                currentBuffs.Remove(currentBuffs[i]);
                break;
            }
        }
    }

    // save load systems
    public void Load_Update_Tile()
    {
        Unlock_Check();

        // seed grow image load 
        if (data.seedPlanted)
        {
            TileSprite_Update_Check();
        }
    }
}
