using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunny_Buffed : MonoBehaviour, IEvent
{
    private Event_System e;

    public Event_Data data;

    private void Awake()
    {
        e = gameObject.transform.parent.GetComponent<Event_System>();
    }
    public void Activate_Event()
    {
        Activate_Sunny_Buffed();
    }

    private bool Is_Weather_Sunny()
    {
        if (e.currentWeather.weatherID == 0) return true;
        else return false;
    }
    private bool Sunny_Buffed_Available(FarmTile farmTile)
    {
        // if the farm tile does not have a sunny buff
        if (farmTile.statusIconIndicator.Find_Status(1)) return false;
        // if the farm tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;
        // if the farm tile is not ready to be harvested
        if (farmTile.tileSeedStatus.harvestReady) return false;
        // if the tile was seed planted for more than 1 day
        if (farmTile.tileSeedStatus.dayPassed < 1) return false;

        return true;
    }

    private void Activate_Sunny_Buffed()
    {
        // if the weather is sunny
        if (!Is_Weather_Sunny()) return;

        // if sunny buff percentage event activates
        if (!e.Percentage_Setter(data.percentage)) return;

        var farmTile = e.controller.farmTiles;

        for (int i = 0; i < farmTile.Length; i++)
        {
            if (!Sunny_Buffed_Available(farmTile[i])) continue;

            // assign status icon
            farmTile[i].statusIconIndicator.Assign_Status(1);

            // activate event
            farmTile[i].tileSeedStatus.health += data.health;
            farmTile[i].tileSeedStatus.watered += data.watered;
            farmTile[i].tileSeedStatus.dayPassed += data.dayPassed;
            farmTile[i].tileSeedStatus.bonusPoints += data.bonusPoints;
        }
    }
}
