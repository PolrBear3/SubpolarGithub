using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow_Attacked : MonoBehaviour, IEvent
{
    private Event_System e;

    public Event_Amount amount;

    private void Awake()
    {
        e = gameObject.transform.parent.GetComponent<Event_System>();
    }
    public void Activate_Event()
    {
        Activate_Crow_Attack();
    }

    private bool Season_Check()
    {
        // if the current season is spring, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 0) return false;

        // if the current season is summer, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 1) return false;

        return true;
    }
    private bool Current_FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the seed is grown more than day passed subtraction amount
        if (farmTile.tileSeedStatus.dayPassed < amount.dayPassed) return false;

        // if the tile is not crow attacked
        if (farmTile.statusIconIndicator.Find_Status(4)) return false;

        return true;
    }

    private void Activate_Crow_Attack()
    {
        // if the season is fall or winter
        if (!Season_Check()) return;

        var farmTiles = e.controller.farmTiles; 
        for (int i = 0; i < farmTiles.Length; i++)
        {
            // percentage activates
            if (!e.Percentage_Setter(amount.percentage)) continue;

            // if the farmTile is in condition for event
            if (!Current_FarmTile_Condition_Check(farmTiles[i])) continue;

            // if the farmTile is not crow attacked
            if (farmTiles[i].Find_Buff(3)) continue;

            // assign crow attacked icon
            farmTiles[i].statusIconIndicator.Assign_Status(4);
            // attack value
            farmTiles[i].tileSeedStatus.health -= amount.health;
            farmTiles[i].tileSeedStatus.watered -= amount.watered;
            farmTiles[i].tileSeedStatus.dayPassed -= amount.dayPassed;
            farmTiles[i].tileSeedStatus.bonusPoints -= amount.bonusPoints;
        }
    }
}
