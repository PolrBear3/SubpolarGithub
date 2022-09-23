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
        CloudyStunned();
    }
    public void All_Events_Update_Check()
    {
        Rainy_AllTileSeed_Watering();
    }
    
    private bool Event_Percentage_Setter(float percentage)
    {
        var value = (100 - percentage) * 0.01f;
        if (Random.value > value)
        {
            return true;
        }
        else return false;
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
                if (!allTiles[i].data.tileLocked)
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
                    if (allTiles[i].data.seedPlanted)
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
            if (Event_Percentage_Setter(40f))
            {
                for (int i = 0; i < allTiles.Length; i++)
                {
                    if (allTiles[i].data.seedPlanted)
                    {
                        // add sunnyBuffed icon
                        allTiles[i].statusIconIndicator.Assign_Status(StatusType.sunnyBuffed);
                        
                        // give +1 extra watered and dayPassed int
                        allTiles[i].tileSeedStatus.watered += 1;
                        allTiles[i].tileSeedStatus.dayPassed += 1;

                        // if the tile is not full grown, addup +n bonus int
                        if (allTiles[i].tileSeedStatus.dayPassed < allTiles[i].tileSeedStatus.fullGrownDay)
                        {
                            allTiles[i].tileSeedStatus.bonusPoints += 1;
                        }

                        // if the seed's health is less than it's max health, give +n health int
                        if (allTiles[i].tileSeedStatus.health < allTiles[i].data.plantedSeed.seedHealth)
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
    private void CloudyStunned()
    {
        if (currentWeather.weatherID == 1 && Event_Percentage_Setter(20f))
        {
            var allTiles = controller.farmTiles;
            for (int i = 0; i < allTiles.Length; i++)
            {
                if (allTiles[i].data.seedPlanted && !allTiles[i].tileSeedStatus.harvestReady)
                {
                    // add sunnyBuffed icon
                    allTiles[i].statusIconIndicator.Assign_Status(StatusType.cloudyStunned);

                    // reduce 1 dayPassed int
                    allTiles[i].tileSeedStatus.dayPassed -= 1;

                    // update tile
                    allTiles[i].TileSprite_Update_Check();
                    controller.plantedMenu.Seed_Information_Update();
                }
            }
        }
    }
}
