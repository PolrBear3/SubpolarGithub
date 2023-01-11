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
    public int tileNum, tileRow, tilePrice;
    public bool tileSelected = false, tileLocked = false, seedPlanted = false;

    public Sprite lockedTile;
    public Sprite unplantedTile;
}

[System.Serializable]
public class FarmTile_Death_Data
{
    public bool died = false;

    public Seed_ScrObj previousSeed;
    public int previousHealth;
}

public class FarmTile : MonoBehaviour
{
    public MainGame_Controller controller;
    public Button button;
    public Animator harvestBorderAnim, farmTileAnim;
    
    [HideInInspector]
    public Image image;
    public Image tileBorder;
    public GameObject tileHighlighter;
    public GameObject deathIcon;

    public FarmTile_Basic_Data data;
    public FarmTile_Death_Data deathData;
    public AfterSeedStatus tileSeedStatus;

    public List<Status> currentStatuses = new List<Status>();
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
    }
    public void Unlock_Tile()
    {
        data.tileLocked = false;

        // unlocked tile image and status indicator
        image.sprite = data.unplantedTile;
    }
    public void Unlock_Check()
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
            else if (deathData.died)
            {
                controller.deathMenu.Open();
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
        currentStatuses.Clear();
        currentBuffs.Clear();

        tileSeedStatus.health = data.plantedSeed.seedHealth;
        tileSeedStatus.watered = 0;
        tileSeedStatus.daysWithoutWater = 0;
        tileSeedStatus.dayPassed = 0;
        tileSeedStatus.currentDayWatered = false;
        tileSeedStatus.harvestReady = false;
        tileSeedStatus.bonusPoints = 0;
        tileSeedStatus.fullGrownDay = Random.Range(data.plantedSeed.minFinishDays, data.plantedSeed.maxFinishDays);
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
            Add_Status(10);
        }
    }
    public void Health_Check()
    {
        // if the tile has no health, reset
        if (data.seedPlanted && tileSeedStatus.health <= 0)
        {
            deathData.died = true;
            deathIcon.SetActive(true);

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

        Get_Previous_Data();

        // if the tile is not ready to be harvested
        if (tileSeedStatus.harvestReady)
        {
            return;
        }

        tileSeedStatus.dayPassed += 1;
    }

    // pulbic systems
    public void Tile_Progress_Update()
    {
        if (!data.tileLocked && data.seedPlanted)
        {
            // day passed limit
            if (tileSeedStatus.dayPassed <= 0)
            {
                tileSeedStatus.dayPassed = 0;
            }
            // bonus point limit
            if (tileSeedStatus.bonusPoints <= 0)
            {
                tileSeedStatus.bonusPoints = 0;
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
                harvestBorderAnim.SetBool("harvestReady", false);
            }
            // early stage of grow
            else if (tileSeedStatus.dayPassed < tileSeedStatus.fullGrownDay / 2)
            {
                image.sprite = data.plantedSeed.sprites[0];
                tileSeedStatus.harvestReady = false;
                harvestBorderAnim.SetBool("harvestReady", false);
            }
        }
    }
    public void Reset_Tile()
    {
        currentStatuses.Clear();
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

    // status sysem
    public bool Find_Status(int statusID)
    {
        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if (currentStatuses[i] == null) break;
            if (currentStatuses[i].statusID != statusID) continue;

            return true;
        }
        return false;
    }
    public int Amount_Status(int statusID)
    {
        int statusAmount = 0;

        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if (currentStatuses[i] == null) break;
            if (Find_Status(statusID))
            {
                statusAmount ++;
            }
        }

        return statusAmount;
    }
    public void Add_Status(int statusID)
    {
        Status statusToAdd = controller.ID_Status_Search(statusID);
        currentStatuses.Add(statusToAdd);
    }
    public void Remove_Status(int statusID, bool nonBreak)
    {
        Status statusToRemove = controller.ID_Status_Search(statusID);

        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if (currentStatuses[i] == null) break;
            if (currentStatuses[i] != statusToRemove) continue;

            currentStatuses.Remove(currentStatuses[i]);

            if (!nonBreak) break;
        }
    }

    // buff system
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
    public int Amount_Buff(int buffID)
    {
        int buffAmount = 0;

        for (int i = 0; i < currentBuffs.Count; i++)
        {
            if (currentBuffs[i] == null) break;
            if (Find_Buff(buffID))
            {
                buffAmount++;
            }
        }

        return buffAmount;
    }
    public void Add_Buff(int buffID)
    {
        Buff_ScrObj buffToAdd = controller.ID_Buff_Search(buffID);
        currentBuffs.Add(buffToAdd);
    }
    public void Remove_Buff(int buffID, bool nonBreak)
    {
        Buff_ScrObj buffToAdd = controller.ID_Buff_Search(buffID);

        for (int i = 0; i < currentBuffs.Count; i++)
        {
            if (currentBuffs[i] == null) break;
            if (currentBuffs[i] != buffToAdd) continue;

            currentBuffs.Remove(currentBuffs[i]);

            if (!nonBreak) break;
        }
    }

    // death system
    private void Get_Previous_Data()
    {
        deathData.previousSeed = data.plantedSeed;
        deathData.previousHealth = tileSeedStatus.health;
    }
    public void Death_Data_Update()
    {
        if (deathData.died)
        {
            deathIcon.SetActive(true);
        }
        else
        {
            deathIcon.SetActive(false);
        }
    }
}
