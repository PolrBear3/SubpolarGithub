using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy_Rain : MonoBehaviour, IEvent, IEventResetable
{
    private Event_System e;

    public Event_Data data;

    private void Awake()
    {
        e = gameObject.transform.parent.GetComponent<Event_System>();
    }
    private void Start()
    {
        data.activated = true;
    }
    public void Activate_Event()
    {
        Activate_Heavy_Rain();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Weather_Is_Stormy()
    {
        // if the weather is stormy
        if (e.data.currentWeather.weatherID != 3) return false;
        
        return true;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

    private void Activate_Rain()
    {
        var farmTiles = e.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // if the tile has a seed planted
            if (!farmTiles[i].data.seedPlanted) continue;

            // if the seed is not watered
            if (farmTiles[i].tileSeedStatus.currentDayWatered) continue;

            // give water
            farmTiles[i].Add_Status(0);
            farmTiles[i].tileSeedStatus.currentDayWatered = true;
            farmTiles[i].tileSeedStatus.daysWithoutWater = 0;
        }
    }
    private void Activate_Heavy_Rain()
    {
        if (!Weather_Is_Stormy()) return;

        // tile is always watered
        Activate_Rain();

        // event activation check
        if (data.activated) return;
        data.activated = true;

        var farmTiles = e.controller.farmTiles;
        
        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // add drown damaged 
            farmTiles[i].Add_Status(12);
            
            // if has umbrella buff, continue
            if (farmTiles[i].Find_Buff(6)) continue;

            // drown damaged
            farmTiles[i].tileSeedStatus.health += data.health;
            farmTiles[i].deathData.damageCount += data.health;
            farmTiles[i].tileSeedStatus.bonusPoints += data.bonusPoints;

            if (!e.Percentage_Setter(data.percentage)) continue;

            // drowned death
            farmTiles[i].Remove_Status(12, false);
            farmTiles[i].Add_Status(13);
            farmTiles[i].deathData.damageCount -= farmTiles[i].tileSeedStatus.health;
            farmTiles[i].tileSeedStatus.health = 0;
        }
    }
}
