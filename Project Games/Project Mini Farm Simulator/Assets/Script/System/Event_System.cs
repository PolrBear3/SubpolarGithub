using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public Weather_ScrObj currentWeather;

    public void Set_Today_Weather()
    {
        var x = controller.timeSystem.currentSeason;
        
        int randomWeatherNum = Random.Range(0, 9);
        currentWeather = x.weatherPercentages[randomWeatherNum];
    }

    public void All_Events_Single_Check()
    {
        GoldenSunny_HealthBuff();
    }
    public void All_Events_Update_Check()
    {
        Rainy_AllTileSeed_Watering();
    }

    // update check events
    private void Rainy_AllTileSeed_Watering()
    {
        if (currentWeather.weatherID == 2)
        {
            var allTiles = controller.farmTiles;

            for (int i = 0; i < allTiles.Length; i++)
            {
                // if the tile is unlocked
                if (!allTiles[i].tileLocked)
                {
                    // if the seed is not currently watered
                    if (!allTiles[i].tileSeedStatus.currentDayWatered)
                    {
                        // refresh status icon
                        allTiles[i].statusIconIndicator.UnAssign_Status(StatusType.watered);

                        // water the seeded tile
                        allTiles[i].tileSeedStatus.currentDayWatered = true;
                        allTiles[i].tileSeedStatus.watered += 1;
                        allTiles[i].statusIconIndicator.Assign_Status(StatusType.watered);
                    }

                    // check water condition for seeded tile 
                    if (allTiles[i].seedPlanted)
                    {
                        allTiles[i].Watering_Check();
                    }
                }
            }
        }
    }

    // single check events
    private void GoldenSunny_HealthBuff()
    {
        if (currentWeather.weatherID == 0)
        {
            var allTiles = controller.farmTiles;

            // 40% chance buff
            if (Random.value > 0.6)
            {
                for (int i = 0; i < allTiles.Length; i++)
                {
                    if (allTiles[i].seedPlanted)
                    {
                        allTiles[i].statusIconIndicator.Assign_Status(StatusType.sunnyBuffed);
                        allTiles[i].tileSeedStatus.watered += 1;
                        allTiles[i].tileSeedStatus.dayPassed += 1;

                        if (allTiles[i].tileSeedStatus.health < allTiles[i].plantedSeed.seedHealth)
                        {
                            allTiles[i].tileSeedStatus.health += 1;
                        }

                        allTiles[i].TileSprite_Update_Check();

                        controller.plantedMenu.Seed_Information_Update();
                    }
                }
            }
        }
    }
}
