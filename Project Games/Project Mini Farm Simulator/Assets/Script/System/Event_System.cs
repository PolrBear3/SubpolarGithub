using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_System : MonoBehaviour
{
    public MainGame_Controller controller;

    public Weather_ScrObj currentWeather;

    // tool systems
    private bool Event_Percentage_Setter(float percentage)
    {
        var value = (100 - percentage) * 0.01f;
        if (Random.value > value)
        {
            return true;
        }
        else return false;
    }
    
    // overall check update
    public void Set_Today_Weather()
    {
        var x = controller.timeSystem.currentSeason;
        
        int randomWeatherNum = Random.Range(0, 9);
        currentWeather = x.weatherPercentages[randomWeatherNum];
    }

    public void All_Events_Single_Check()
    {
        SunnyBuffed();
        CloudyStunned();
        CrowAttacked();
    }
    public void All_Events_Update_Check()
    {
        Rainy_AllTileSeed_Watering();
    }

    // single check events
    private void SunnyBuffed()
    {
        if (currentWeather.weatherID == 0)
        {
            var allTiles = controller.farmTiles;

            // 40% chance buff
            if (Event_Percentage_Setter(controller.ID_Status_Search(1).eventPercentage))
            {
                for (int i = 0; i < allTiles.Length; i++)
                {
                    if (allTiles[i].data.seedPlanted)
                    {
                        // add sunnyBuffed icon
                        allTiles[i].statusIconIndicator.Assign_Status(1);
                        
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
                    }
                }
            }
        }
    }
    private void CloudyStunned()
    {
        if (currentWeather.weatherID == 1 && Event_Percentage_Setter(controller.ID_Status_Search(2).eventPercentage))
        {
            var allTiles = controller.farmTiles;
            for (int i = 0; i < allTiles.Length; i++)
            {
                if (allTiles[i].data.seedPlanted && !allTiles[i].tileSeedStatus.harvestReady)
                {
                    // add cloudyStunned icon
                    allTiles[i].statusIconIndicator.Assign_Status(2);

                    // reduce 1 dayPassed int
                    allTiles[i].tileSeedStatus.dayPassed -= 1;
                }
            }
        }
    }
    private void CrowAttacked()
    {
        // if current season is fall or winter
        if (controller.timeSystem.currentSeason.seasonID == 2 || controller.timeSystem.currentSeason.seasonID == 3)
        {
            var allTiles = controller.farmTiles;

            for (int i = 0; i < allTiles.Length; i++)
            {
                // if the tile is seed planted
                if (allTiles[i].data.seedPlanted)
                {
                    // if corw attack percentage activates
                    if (Event_Percentage_Setter(controller.ID_Status_Search(4).eventPercentage))
                    {
                        // assign crow attack status icon
                        allTiles[i].statusIconIndicator.Assign_Status(4);

                        // divide current health and decrease 1
                        allTiles[i].tileSeedStatus.health = (allTiles[i].tileSeedStatus.health / 2) - 1;
                        // seed day passed decrease to half and -1 day
                        allTiles[i].tileSeedStatus.dayPassed = (allTiles[i].tileSeedStatus.dayPassed / 2) - 1;
                        // seed watered points to 0
                        allTiles[i].tileSeedStatus.watered = 0;
                        // set tile bonus points -1 if it is not 0
                        if (allTiles[i].tileSeedStatus.bonusPoints > 0)
                        {
                            allTiles[i].tileSeedStatus.bonusPoints -= 1;
                        }

                        // for the tiles that are cross positioned from this tile
                        controller.Set_Cross_Surrounding_FarmTiles(allTiles[i].data.tileNum);

                        var x = controller.crossSurroundingFarmTiles;
                        for (int j = 0; j < x.Count; j++)
                        {
                            // if crossFarmTiles is not null & seed is planted
                            if (x[j].data.seedPlanted)
                            {
                                // if event of 10& activates
                                if (Event_Percentage_Setter(controller.ID_Status_Search(5).eventPercentage))
                                {
                                    // add crow surround attacked icon
                                    x[j].statusIconIndicator.Assign_Status(5);

                                    // set seed current health to 1
                                    x[j].tileSeedStatus.health -= 1;
                                    // seed day passed decrease 1 day
                                    x[j].tileSeedStatus.dayPassed -= 1;
                                    // seed watered points decrease 1
                                    x[j].tileSeedStatus.watered -= 1;
                                }
                            }
                        }
                    }
                }
            }    
        }
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
                        allTiles[i].statusIconIndicator.UnAssign_Status(0);

                        // water the seeded tile
                        allTiles[i].tileSeedStatus.currentDayWatered = true;
                        allTiles[i].tileSeedStatus.watered += 1;
                        allTiles[i].statusIconIndicator.Assign_Status(0);
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
}
