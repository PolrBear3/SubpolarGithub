using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Save_System : MonoBehaviour
{
    public MainGame_Controller controller;

    private void Start()
    {
        Load_All();
    }
    private void OnApplicationQuit()
    {
        Save_All();
    }

    private void Save_All()
    {
        SaveAll_Tiles();
        Save_Money();
        Save_CurrentWeather();
        Save_CurrentInGameDay();
    }
    private void Load_All()
    {
        LoadAll_Tiles();
        Load_Money();
        Load_CurrentInGameDay();
        Load_CurrentWeather();
        controller.timeSystem.Load_Day();
    }

    // farm tiles
    private void SaveAll_Tiles()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            Save_FarmTile(controller.farmTiles[i]);
        }
    }

    private void Save_FarmTile(FarmTile farmTile)
    {
        ES3.Save("farmTile" + farmTile.saveName + " tileLocked", farmTile.data.tileLocked);
        ES3.Save("farmTile" + farmTile.saveName + " seedPlanted", farmTile.data.seedPlanted);

        if (farmTile.data.seedPlanted)
        {
            ES3.Save("farmTile" + farmTile.saveName + " plantedSeedID", farmTile.data.plantedSeed.seedID);

            ES3.Save("farmTile" + farmTile.saveName + " fullGrownDay", farmTile.tileSeedStatus.fullGrownDay);
            ES3.Save("farmTile" + farmTile.saveName + " dayPassed", farmTile.tileSeedStatus.dayPassed);
            ES3.Save("farmTile" + farmTile.saveName + " health", farmTile.tileSeedStatus.health);
            ES3.Save("farmTile" + farmTile.saveName + " daysWithoutWater", farmTile.tileSeedStatus.daysWithoutWater);
            ES3.Save("farmTile" + farmTile.saveName + " currentDayWatered", farmTile.tileSeedStatus.currentDayWatered);
            ES3.Save("farmTile" + farmTile.saveName + " watered", farmTile.tileSeedStatus.watered);
        }

        Save_FarmTile_Status(farmTile);
        Save_FarmTile_Buffs(farmTile);
    }
    private void Save_FarmTile_Status(FarmTile farmTile)
    {
        ES3.Save("farmTile" + farmTile.saveName + " status", farmTile.statusIconIndicator.statusIcons);
    }
    private void Save_FarmTile_Buffs(FarmTile farmTile)
    {
        ES3.Save("farmTile" + farmTile.saveName + " buffs", farmTile.currentBuffs);
    }

    private void LoadAll_Tiles()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            Load_FarmTile(controller.farmTiles[i]);
            controller.farmTiles[i].Load_Update_Tile();
        }
    }

    private void Load_FarmTile(FarmTile farmTile)
    {
        if (ES3.KeyExists("farmTile" + farmTile.saveName + " tileLocked"))
        {
            // tile unlock status load
            farmTile.data.tileLocked = ES3.Load<bool>("farmTile" + farmTile.saveName + " tileLocked");

            if (ES3.KeyExists("farmTile" + farmTile.saveName + " seedPlanted"))
            {
                // farmtile anything planted status load
                farmTile.data.seedPlanted = ES3.Load<bool>("farmTile" + farmTile.saveName + " seedPlanted");

                if (farmTile.data.seedPlanted)
                {
                    var seedSearchID = ES3.Load<int>("farmTile" + farmTile.saveName + " plantedSeedID");
                    for (int i = 0; i < controller.allSeeds.Length; i++)
                    {
                        if (seedSearchID == controller.allSeeds[i].seedID)
                        {
                            // planted seed ScrObj load
                            farmTile.data.plantedSeed = controller.allSeeds[i];
                        }
                    }

                    // full grown day
                    farmTile.tileSeedStatus.fullGrownDay = ES3.Load<int>("farmTile" + farmTile.saveName + " fullGrownDay");
                    // day passed
                    farmTile.tileSeedStatus.dayPassed = ES3.Load<int>("farmTile" + farmTile.saveName + " dayPassed");
                    // health
                    farmTile.tileSeedStatus.health = ES3.Load<int>("farmTile" + farmTile.saveName + " health");
                    // days without water
                    farmTile.tileSeedStatus.daysWithoutWater = ES3.Load<int>("farmTile" + farmTile.saveName + " daysWithoutWater");
                    // current day watered
                    farmTile.tileSeedStatus.currentDayWatered = ES3.Load<bool>("farmTile" + farmTile.saveName + " currentDayWatered");
                    // overall watering amount
                    farmTile.tileSeedStatus.watered = ES3.Load<int>("farmTile" + farmTile.saveName + " watered");
                }
            }

            // farmtile status icons
            Load_FarmTile_Status(farmTile);
            // farmtile current buffs
            Load_FarmTile_Buffs(farmTile);
        }
    }
    private void Load_FarmTile_Status(FarmTile farmTile)
    {
        if (ES3.KeyExists("farmTile" + farmTile.saveName + " status"))
        {
            var savedStatusArray = ES3.Load("farmTile" + farmTile.saveName + " status", farmTile.statusIconIndicator.statusIcons);
            farmTile.statusIconIndicator.statusIcons = savedStatusArray;

            for (int i = 0; i < farmTile.statusIconIndicator.statusIcons.Length; i++)
            {
                farmTile.statusIconIndicator.statusIcons[i].Load_Icon();
            }
        }
    }
    private void Load_FarmTile_Buffs(FarmTile farmTile)
    {
        if (ES3.KeyExists("farmTile" + farmTile.saveName + " buffs"))
        {
            var savedBuffList = ES3.Load("farmTile" + farmTile.saveName + " buffs", farmTile.currentBuffs);
            farmTile.currentBuffs = savedBuffList;
        }
    }

    // money
    private void Save_Money()
    {
        ES3.Save("money", controller.money);
    }
    private void Load_Money()
    {
        if (ES3.KeyExists("money"))
        {
            controller.Load_Money(ES3.Load<int>("money"));
        }
    }

    // day
    private void Save_CurrentInGameDay()
    {
        ES3.Save("currentInGameDay", controller.timeSystem.currentInGameDay);
    }
    private void Load_CurrentInGameDay()
    {
        if (ES3.KeyExists("currentInGameDay"))
        {
            controller.timeSystem.currentInGameDay = ES3.Load<int>("currentInGameDay");
        }
    }

    // weather
    private void Save_CurrentWeather()
    {
        ES3.Save("currentWeather", controller.eventSystem.currentWeather.weatherID);
    }
    private void Load_CurrentWeather()
    {
        if (ES3.KeyExists("currentWeather"))
        {
            int savedNum = ES3.Load<int>("currentWeather");
            for (int i = 0; i < controller.allWeathers.Length; i++)
            {
                if (savedNum == controller.allWeathers[i].weatherID)
                {
                    controller.eventSystem.currentWeather = controller.allWeathers[i];
                }
            }
        }
    }
}