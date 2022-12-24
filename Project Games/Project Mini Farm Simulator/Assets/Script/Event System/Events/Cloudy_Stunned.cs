using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloudy_Stunned : MonoBehaviour, IEvent
{
    private Event_System e;

    public Event_Amount amount;

    private void Awake()
    {
        e = gameObject.transform.parent.GetComponent<Event_System>();
    }
    public void Activate_Event()
    {
        Activate_Cloudy_Stunned();
    }

    private bool Is_Weather_Cloudy()
    {
        if (e.currentWeather.weatherID == 1) return true;
        else return false;
    }
    private bool FarmTile_Event_Available(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the seed is grown more than 1 day
        if (farmTile.tileSeedStatus.dayPassed < 1) return false;

        // if the tile is not ready to be harvested
        if (farmTile.tileSeedStatus.harvestReady) return false;

        // if the seed is not cloudy stunned
        if (farmTile.statusIconIndicator.Find_Status(2)) return false;

        return true;
    }
    private bool FarmTile_Has_CloudyStunShield(FarmTile farmTile) // bug ??
    {
        // if the seeded tile has a cloudy buff
        if (farmTile.Find_Buff(0))
        {
            // get rid of cloudy stun shield buff 
            farmTile.Remove_Buff(e.controller.ID_Buff_Search(0));
            
            return true;
        }
        return false;
    }

    private void Activate_Cloudy_Stunned()
    {
        // if the weather is cloudy
        if (!Is_Weather_Cloudy()) return;

        // if the percentage activates
        if (!e.Percentage_Setter(amount.percentage)) return;

        var farmTiles = e.controller.farmTiles;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            // check if farmtile is ready for event
            if (!FarmTile_Event_Available(farmTiles[i])) continue;

            // check if the farmtile has a cloudy stun shield buff
            if (FarmTile_Has_CloudyStunShield(farmTiles[i])) continue;

            // assign icon
            farmTiles[i].statusIconIndicator.Assign_Status(2);

            // cloudy stunned events
            farmTiles[i].tileSeedStatus.dayPassed -= amount.dayPassed;
        }
    }
}
