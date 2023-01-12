using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Save_System_Data
{
    public bool gameSaved;
}

[System.Serializable]
public class Save_System_UI
{
    public GameObject resetGamePanel;
    public Animator warningBox;
}

public class Save_System : MonoBehaviour
{
    public MainGame_Controller controller;

    [SerializeField] private Save_System_Data data;
    [SerializeField] private Save_System_UI ui;

    private void Start()
    {
        Load_All();
    }
    private void OnApplicationQuit()
    {
        Save_All();
    }
    
    /*private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save_All();
        }    
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Save_All();
        }
    }*/

    private void Save_All()
    {
        Save_New_Game();

        Save_Current_Day();
        Save_Current_Weather();
        Save_All_FarmTiles();
        Save_Money_Amount();
        Save_All_Collectable_Datas();
        Save_All_CollectableFrames();

        Debug.Log("Game Saved");
    }
    private void Load_All()
    {
        Load_Game();

        Load_Current_Day();
        Load_Current_Weather();
        Load_All_FarmTiles();
        Load_Money_Amount();
        Load_All_Collectable_Datas();
        Load_All_CollectableFrames();

        Debug.Log("Game Loaded");
    }

    // game reset system
    public void Open_Reset_Game_Panel()
    {
        controller.optionsMenu.Button_Shield(true);
        controller.defaultMenu.Button_Shield(true);

        ui.resetGamePanel.SetActive(true);

        ui.warningBox.SetBool("panelOn", true);
    }
    public void Close_Reset_Game_Panel()
    {
        ui.warningBox.SetBool("panelOn", false);

        controller.optionsMenu.Button_Shield(false);
        controller.defaultMenu.Button_Shield(false);

        ui.resetGamePanel.SetActive(false);
    }
    public void Reset_Game()
    {
        // delete all saved data
        ES3.DeleteFile("SaveFile.es3");
        // relaunch scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // check systems
    public bool Game_Saved()
    {
        if (data.gameSaved) return true;
        else return false;
    }

    // Game
    private void Save_New_Game()
    {
        data.gameSaved = true;
        ES3.Save("gameSaved", data.gameSaved);
    }
    private void Load_Game()
    {
        if (!ES3.KeyExists("gameSaved")) return;

        data.gameSaved = ES3.Load("gameSaved", data.gameSaved);
    }

    // farm tiles
    private void Save_All_FarmTiles()
    {
        var farmTiles = controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            Save_FarmTile_Data(farmTiles[i], i);
            Save_FarmTile_Status(farmTiles[i], i);
            Save_FarmTile_DeathData(farmTiles[i], i);
            Save_FarmTile_EventStatus(farmTiles[i], i);
            Save_FarmTile_Buff(farmTiles[i], i);
        }
    }
    private void Save_FarmTile_Data(FarmTile farmTile, int farmTileNum)
    {
        ES3.Save("tileLocked" + farmTileNum.ToString(), farmTile.data.tileLocked);
        ES3.Save("seedPlanted" + farmTileNum.ToString(), farmTile.data.seedPlanted);
        ES3.Save("plantedSeed" + farmTileNum.ToString(), farmTile.data.plantedSeed);
    }
    private void Save_FarmTile_Status(FarmTile farmTile, int farmTileNum)
    {
        ES3.Save("health" + farmTileNum.ToString(), farmTile.tileSeedStatus.health);
        ES3.Save("dayPassed" + farmTileNum.ToString(), farmTile.tileSeedStatus.dayPassed);
        ES3.Save("fullGrownDay" + farmTileNum.ToString(), farmTile.tileSeedStatus.fullGrownDay);
        ES3.Save("watered" + farmTileNum.ToString(), farmTile.tileSeedStatus.watered);
        ES3.Save("daysWithoutWater" + farmTileNum.ToString(), farmTile.tileSeedStatus.daysWithoutWater);
        ES3.Save("currentDayWatered" + farmTileNum.ToString(), farmTile.tileSeedStatus.currentDayWatered);
        ES3.Save("harvestReady" + farmTileNum.ToString(), farmTile.tileSeedStatus.harvestReady);
        ES3.Save("bonusPoints" + farmTileNum.ToString(), farmTile.tileSeedStatus.bonusPoints);
    }
    private void Save_FarmTile_DeathData(FarmTile farmTile, int farmTileNum)
    {
        ES3.Save("died" + farmTileNum.ToString(), farmTile.deathData.died);
        ES3.Save("previousSeed" + farmTileNum.ToString(), farmTile.deathData.previousSeed);
        ES3.Save("previousHealth" + farmTileNum.ToString(), farmTile.deathData.previousHealth);
    }
    private void Save_FarmTile_EventStatus(FarmTile farmTile, int farmTileNum)
    {
        ES3.Save("currentStatuses" + farmTileNum.ToString(), farmTile.currentStatuses);
    }
    private void Save_FarmTile_Buff(FarmTile farmTile, int farmTileNum)
    {
        ES3.Save("currentBuffs" + farmTileNum.ToString(), farmTile.currentBuffs);
    }

    public void Load_All_FarmTiles()
    {
        if (!Game_Saved()) return;

        var farmTiles = controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            Load_FarmTile_Data(farmTiles[i], i);
            Load_FarmTile_Status(farmTiles[i], i);
            Load_FarmTile_DeatData(farmTiles[i], i);
            Load_FarmTile_EventStatus(farmTiles[i], i);
            Load_FarmTile_Buff(farmTiles[i], i);
        }
    } // activates at time system
    private void Load_FarmTile_Data(FarmTile farmTile, int farmTileNum)
    {
        farmTile.data.tileLocked = ES3.Load("tileLocked" + farmTileNum.ToString(), farmTile.data.tileLocked);
        farmTile.data.seedPlanted = ES3.Load("seedPlanted" + farmTileNum.ToString(), farmTile.data.seedPlanted);
        farmTile.data.plantedSeed = ES3.Load("plantedSeed" + farmTileNum.ToString(), farmTile.data.plantedSeed);

        farmTile.Unlock_Check();
    }
    private void Load_FarmTile_Status(FarmTile farmTile, int farmTileNum)
    {
        farmTile.tileSeedStatus.health = ES3.Load("health" + farmTileNum.ToString(), farmTile.tileSeedStatus.health);
        farmTile.tileSeedStatus.dayPassed = ES3.Load("dayPassed" + farmTileNum.ToString(), farmTile.tileSeedStatus.dayPassed);
        farmTile.tileSeedStatus.fullGrownDay = ES3.Load("fullGrownDay" + farmTileNum.ToString(), farmTile.tileSeedStatus.fullGrownDay);
        farmTile.tileSeedStatus.watered = ES3.Load("watered" + farmTileNum.ToString(), farmTile.tileSeedStatus.watered);
        farmTile.tileSeedStatus.daysWithoutWater = ES3.Load("daysWithoutWater" + farmTileNum.ToString(), farmTile.tileSeedStatus.daysWithoutWater);
        farmTile.tileSeedStatus.currentDayWatered = ES3.Load("currentDayWatered" + farmTileNum.ToString(), farmTile.tileSeedStatus.currentDayWatered);
        farmTile.tileSeedStatus.harvestReady = ES3.Load("harvestReady" + farmTileNum.ToString(), farmTile.tileSeedStatus.harvestReady);
        farmTile.tileSeedStatus.bonusPoints = ES3.Load("bonusPoints" + farmTileNum.ToString(), farmTile.tileSeedStatus.bonusPoints);

        farmTile.Tile_Progress_Update();
    }
    private void Load_FarmTile_DeatData(FarmTile farmTile, int farmTileNum)
    {
        farmTile.deathData.died = ES3.Load("died" + farmTileNum.ToString(), farmTile.deathData.died);
        farmTile.deathData.previousSeed = ES3.Load("previousSeed" + farmTileNum.ToString(), farmTile.deathData.previousSeed);
        farmTile.deathData.previousHealth = ES3.Load("previousHealth" + farmTileNum.ToString(), farmTile.deathData.previousHealth);

        farmTile.Death_Data_Update();
    }
    private void Load_FarmTile_EventStatus(FarmTile farmTile, int farmTileNum)
    {
        farmTile.currentStatuses = ES3.Load("currentStatuses" + farmTileNum.ToString(), farmTile.currentStatuses);
    }
    private void Load_FarmTile_Buff(FarmTile farmTile, int farmTileNum)
    {
        farmTile.currentBuffs = ES3.Load("currentBuffs" + farmTileNum.ToString(), farmTile.currentBuffs);
    }

    // money
    private void Save_Money_Amount()
    {
        ES3.Save("money", controller.money);
    }
    private void Load_Money_Amount()
    {
        if (!Game_Saved()) return;

        controller.Add_Money(ES3.Load("money", controller.money), 0);
    }

    // current day
    private void Save_Current_Day()
    {
        ES3.Save("currentInGameDay", controller.timeSystem.currentInGameDay);
    }
    private void Load_Current_Day()
    {
        if (!Game_Saved()) return;

        controller.timeSystem.currentInGameDay = ES3.Load("currentInGameDay", controller.timeSystem.currentInGameDay);

        controller.timeSystem.Check_End0f_Year();
        controller.timeSystem.Check_Season();

        controller.defaultMenu.Current_InGameDay_Text_Update();
        controller.defaultMenu.Season_UI_Update();
    }

    // weather
    private void Save_Current_Weather()
    {
        ES3.Save("currentWeatherID", controller.eventSystem.data.currentWeather.weatherID);
    }
    private void Load_Current_Weather()
    {
        if (!Game_Saved()) return;

        int currentWeatherID = ES3.Load<int>("currentWeatherID");
        var currentWeather = controller.ID_Weather_Search(currentWeatherID);
        controller.eventSystem.data.currentWeather = currentWeather;

        controller.defaultMenu.Weather_UI_Update();
        // fade lean tween
    }

    // collectables
    private void Save_All_Collectable_Datas()
    {
        var collectableDatas = controller.collectableRoomMenu.allCollectables;

        for (int i = 0; i < collectableDatas.Length; i++)
        {
            ES3.Save("unlocked" + i.ToString(), collectableDatas[i].unLocked);
            ES3.Save("isNew" + i.ToString(), collectableDatas[i].isNew);
            ES3.Save("goldMode" + i.ToString(), collectableDatas[i].goldModeAvailable);
            ES3.Save("currentAmount" + i.ToString(), collectableDatas[i].currentAmount);
            ES3.Save("maxAmount" + i.ToString(), collectableDatas[i].maxAmount);
        }
    }
    private void Load_All_Collectable_Datas()
    {
        if (!Game_Saved()) return;

        var collectableDatas = controller.collectableRoomMenu.allCollectables;

        for (int i = 0; i < collectableDatas.Length; i++)
        {
            collectableDatas[i].unLocked = ES3.Load("unlocked" + i.ToString(), collectableDatas[i].unLocked);
            collectableDatas[i].isNew = ES3.Load("isNew" + i.ToString(), collectableDatas[i].isNew);
            collectableDatas[i].goldModeAvailable = ES3.Load("goldMode" + i.ToString(), collectableDatas[i].goldModeAvailable);
            collectableDatas[i].currentAmount = ES3.Load("currentAmount" + i.ToString(), collectableDatas[i].currentAmount);
            collectableDatas[i].maxAmount = ES3.Load("maxAmount" + i.ToString(), collectableDatas[i].maxAmount);
        }
    }

    // collectable frame statuses
    private void Save_All_CollectableFrames()
    {
        var allFrames = controller.collectableRoomMenu.allFrames;

        for (int i = 0; i < allFrames.Length; i++)
        {
            ES3.Save("collectablePlaced" + i.ToString(), allFrames[i].data.collectablePlaced);
            ES3.Save("currentCollectable" + i.ToString(), allFrames[i].data.currentCollectable);
        }
    }
    private void Load_All_CollectableFrames()
    {
        if (!Game_Saved()) return;

        var allFrames = controller.collectableRoomMenu.allFrames;

        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].data.collectablePlaced = ES3.Load("collectablePlaced" + i.ToString(), allFrames[i].data.collectablePlaced);
            allFrames[i].data.currentCollectable = ES3.Load("currentCollectable" + i.ToString(), allFrames[i].data.currentCollectable);

            allFrames[i].Load_FrameSprite();
        }
    }
}