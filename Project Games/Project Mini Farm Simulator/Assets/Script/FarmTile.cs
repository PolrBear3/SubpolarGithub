using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
    public Animator harvestBorderAnim, harvestCoinsAnim;
    public Status_Icon_Indicator statusIconIndicator;
    
    [HideInInspector]
    public Image image;
    public Image tileBorder;
    public GameObject tileHighlighter;

    public string saveName;
    public FarmTile_Basic_Data data;
    public AfterSeedStatus tileSeedStatus;

    public List<Buff_ScrObj> currentBuffs = new List<Buff_ScrObj>();

    private void Awake()
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
        tileHighlighter.SetActive(true);
    }
    public void UnHighlight_Tile()
    {
        data.tileSelected = false;
        tileHighlighter.SetActive(false);
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
        statusIconIndicator.Reset_All_Icons();

        tileSeedStatus.health = data.plantedSeed.seedHealth;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.currentDayWatered = false;
        tileSeedStatus.harvestReady = false;
        tileSeedStatus.bonusPoints = 0;
        tileSeedStatus.fullGrownDay = Random.Range(data.plantedSeed.minFinishDays, data.plantedSeed.maxFinishDays);
        harvestCoinsAnim.SetBool("harvest", false);
    }
    
    // seed plant update system
    public void Watering_Check()
    {
        // watering min limit
        if (tileSeedStatus.watered < 0)
        {
            tileSeedStatus.watered = 0;
        }

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
    public void Health_Check()
    {
        // if the tile has no health, reset
        if (data.seedPlanted && tileSeedStatus.health <= 0)
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
            tileSeedStatus.harvestReady = false;
            harvestBorderAnim.SetBool("harvestReady", false);
            tileSeedStatus.bonusPoints = 0;
        }
    }

    public void NextDay_Seed_Status_Update()
    {
        tileSeedStatus.currentDayWatered = false;

        // if the tile has a seed planted
        if (!data.seedPlanted)
        {
            return;
        }
        // if the tile is not ready to be harvested
        if (tileSeedStatus.harvestReady)
        {
            return;
        }

        tileSeedStatus.dayPassed += 1;
    }

    // pulbic systems
    public void TileSprite_Update()
    {
        if (!data.tileLocked && data.seedPlanted)
        {
            if (tileSeedStatus.dayPassed <= 0)
            {
                tileSeedStatus.dayPassed = 0;
            }

            // full grown complete check
            if (tileSeedStatus.dayPassed >= tileSeedStatus.fullGrownDay)
            {
                image.sprite = data.plantedSeed.sprites[2];
                tileSeedStatus.harvestReady = true;
                harvestBorderAnim.SetBool("harvestReady", true);
            }
            // half grown complete check
            else if (tileSeedStatus.dayPassed >= tileSeedStatus.fullGrownDay / 2)
            {
                image.sprite = data.plantedSeed.sprites[1];
                tileSeedStatus.harvestReady = false;
            }
            // early stage of grow
            else if (tileSeedStatus.dayPassed < tileSeedStatus.fullGrownDay / 2)
            {
                image.sprite = data.plantedSeed.sprites[0];
                tileSeedStatus.harvestReady = false;
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
        tileSeedStatus.harvestReady = false;
        harvestBorderAnim.SetBool("harvestReady", false);
        tileSeedStatus.bonusPoints = 0;
    }

    public bool Find_Buff(int buffID)
    {
        for (int i = 0; i < currentBuffs.Count; i++)
        {
            if (currentBuffs[i] == null) break;
            if (currentBuffs[i].buffID != buffID) continue;

            return true;
        }
        return false;
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
    public void Remove_Buff_NonBreak(Buff_ScrObj buff)
    {
        for (int i = 0; i < currentBuffs.Count; i++)
        {
            if (buff == currentBuffs[i])
            {
                currentBuffs.Remove(currentBuffs[i]);
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
            TileSprite_Update();
        }
    }
}
