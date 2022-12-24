using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow_Attacked : MonoBehaviour, IEvent, IEventResetable
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
        Activate_Crow_Attack();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Season_Check()
    {
        // if the current season is spring, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 0) return false;

        // if the current season is summer, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 1) return false;

        return true;
    }
    private bool FarmTile_Has_Scarecrow(FarmTile farmTile)
    {
        if (farmTile.Find_Buff(3))
        {
            farmTile.Remove_Buff(e.controller.ID_Buff_Search(3));
            return true;
        }
        return false;
    }
    private bool Current_FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the seed is grown more than day passed subtraction amount
        if (farmTile.tileSeedStatus.dayPassed < data.dayPassed) return false;

        return true;
    }
    // cross surrounding farmtile condition check(FarmTile farmTile)

    private void Activate_Crow_Attack()
    {
        // if event is not activated
        if (data.activated) return;
        // activate event
        data.activated = true;

        // if the season is fall or winter
        if (!Season_Check()) return;

        var farmTiles = e.controller.farmTiles; 
        for (int i = 0; i < farmTiles.Length; i++)
        {
            // percentage activates
            if (!e.Percentage_Setter(data.percentage)) continue;

            // if the farmTile is in condition for event
            if (!Current_FarmTile_Condition_Check(farmTiles[i])) continue;

            // if the farmTile does not have a scarecrow buff
            if (FarmTile_Has_Scarecrow(farmTiles[i])) continue;

            // assign crow attacked icon
            farmTiles[i].statusIconIndicator.Assign_Status(4);
            // attack value
            farmTiles[i].tileSeedStatus.health -= data.health;
            farmTiles[i].tileSeedStatus.watered -= data.watered;
            farmTiles[i].tileSeedStatus.dayPassed -= data.dayPassed;
            farmTiles[i].tileSeedStatus.bonusPoints -= data.bonusPoints;

            // additional crow attack
            Activate_Additional_Crow_Attack(farmTiles[i]);
        }
    }
    private void Activate_Additional_Crow_Attack(FarmTile farmTile)
    {
        // for the tiles that are cross surrounding this farmTile

        // if the tile has a seed planted
        
        // if the tile does not have a scarecrow buff (id 3)

        // if additional crow attack percentage activates

        // assign additional crow attacked icon
        // attack value
    }
}
