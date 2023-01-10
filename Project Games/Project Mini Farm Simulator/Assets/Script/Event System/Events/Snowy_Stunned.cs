using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowy_Stunned : MonoBehaviour, IEvent, IEventResetable
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
        Activate_Snowy_Stun();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Is_Season_Winter()
    {
        if (e.controller.timeSystem.currentSeason.seasonID == 3) return true;
        else return false;
    }
    private bool Is_Weather_Snowy()
    {
        if (e.data.currentWeather.weatherID == 4) return true;
        else return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }
    private bool FarmTile_Has_SnowyStunShield(FarmTile farmTile)
    {
        // if the seeded tile has a snowy stun shield
        if (farmTile.Find_Buff(4)) return true;
        else return false;
    }

    private void Activate_Snowy_Stun()
    {
        // event activation check
        if (data.activated) return;
        data.activated = true;

        // if the current season is winter
        if (!Is_Season_Winter()) return;

        // if the current weather is snowy
        if (!Is_Weather_Snowy()) return;

        var farmTiles = e.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // farmtile condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // percentage activation
            if (!e.Percentage_Setter(data.percentage)) continue;

            // assign snowy stun status
            farmTiles[i].Add_Status(3);

            // if the farmtile doesn't have buff
            if (FarmTile_Has_SnowyStunShield(farmTiles[i])) continue;

            // data activation 1
            farmTiles[i].tileSeedStatus.dayPassed -= data.dayPassed;

            // percentage activation
            if (!e.Percentage_Setter(data.percentage)) continue;

            // data activation 2
            farmTiles[i].tileSeedStatus.health -= data.health;
            farmTiles[i].tileSeedStatus.bonusPoints -= data.bonusPoints;
        }
    }
}
