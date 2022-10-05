using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Save_System : MonoBehaviour
{
    public MainGame_Controller controller;

    private void Start()
    {
        LoadAll_Tiles();
        Load_Money();
    }
    private void OnApplicationQuit()
    {
        SaveAll_Tiles();
        Save_Money();
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

    // season, weather, day
}
