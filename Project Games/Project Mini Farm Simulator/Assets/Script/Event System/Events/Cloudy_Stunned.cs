using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloudy_Stunned : MonoBehaviour, IEvent, IEventResetable
{
    private Event_System e;

    public Event_Data data;

    private void Start()
    {
        data.activated = true;
    }
    private void Awake()
    {
        e = gameObject.transform.parent.GetComponent<Event_System>();
    }
    public void Activate_Event()
    {
        Activate_Cloudy_Stunned();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Is_Weather_Cloudy()
    {
        if (e.data.currentWeather.weatherID == 1) return true;
        else return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the tile is not ready to be harvested
        if (farmTile.tileSeedStatus.harvestReady) return false;

        return true;
    }
    private bool FarmTile_Has_CloudyStunShield(FarmTile farmTile)
    {
        // if the seeded tile has a cloudy buff
        if (farmTile.Find_Buff(0)) return true;
        else return false;
    }

    private void Activate_Cloudy_Stunned()
    {
        // if event is not activated
        if (data.activated) return;
        // activate event
        data.activated = true;

        // if the weather is cloudy
        if (!Is_Weather_Cloudy()) return;

        // if the percentage activates
        if (!e.Percentage_Setter(data.percentage)) return;

        var farmTiles = e.controller.farmTiles;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            // check if farmtile is ready for event
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // assign icon
            farmTiles[i].Add_Status(2);

            // check if the farmtile has a cloudy stun shield buff
            if (FarmTile_Has_CloudyStunShield(farmTiles[i])) continue;

            // cloudy stunned events
            farmTiles[i].tileSeedStatus.dayPassed -= data.dayPassed;
            farmTiles[i].tileSeedStatus.watered -= data.watered;
        }
    }
}
