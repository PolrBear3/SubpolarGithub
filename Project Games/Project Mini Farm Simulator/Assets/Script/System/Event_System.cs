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

    public void All_Events_Check()
    {
        Rainy_AllTileSeed_Watering();
    }

    // all events for weathers
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
}
