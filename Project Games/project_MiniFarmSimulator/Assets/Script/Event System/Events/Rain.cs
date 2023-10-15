using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour, IEvent
{
    private Event_System es;

    public Event_Data data;

    private void Awake()
    {
        es = gameObject.transform.parent.GetComponent<Event_System>();
    }
    public void Activate_Event()
    {
        Activate_Rain();
    }

    private bool Is_Raining()
    {
        if (es.data.currentWeather.weatherID == 2) return true;
        else return false;
    }
    private bool FarmTile_Event_Check(FarmTile farmTile)
    {
        // if the tile is has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the tile is currently watered
        if (farmTile.tileSeedStatus.currentDayWatered) return false;

        return true;
    }

    private void Activate_Rain()
    {
        if (!Is_Raining()) return;

        var farmTiles = es.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // farmtile condition check
            if (!FarmTile_Event_Check(farmTiles[i])) continue;

            farmTiles[i].tileSeedStatus.currentDayWatered = true;
            farmTiles[i].tileSeedStatus.daysWithoutWater = 0;
            farmTiles[i].Add_Status(0);
        }
    }
}
