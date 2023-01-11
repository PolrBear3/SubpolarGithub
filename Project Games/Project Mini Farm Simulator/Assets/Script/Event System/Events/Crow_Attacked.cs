using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow_Attacked : MonoBehaviour, IEvent, IEventResetable
{
    private Event_System e;

    public Event_Data data;
    public Event_Data subData;

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
        Activate_Crow_Attack();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Season_Check()
    {
        // if the current season is fall, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 2) return false;

        // if the current season is winter, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 3) return false;

        return true;
    }
    private bool FarmTile_Has_Scarecrow(FarmTile farmTile)
    {
        if (farmTile.Find_Buff(3))
        {
            return true;
        }
        return false;
    }
    
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

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
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // assign crow attacked icon
            farmTiles[i].Add_Status(4);

            // if the farmTile does not have a scarecrow buff
            if (FarmTile_Has_Scarecrow(farmTiles[i])) continue;

            // attack value
            farmTiles[i].tileSeedStatus.health += data.health;
            farmTiles[i].tileSeedStatus.watered += data.watered;
            farmTiles[i].tileSeedStatus.dayPassed += data.dayPassed;
            farmTiles[i].tileSeedStatus.bonusPoints += data.bonusPoints;

            // additional crow attack
            Activate_Additional_Crow_Attack(farmTiles[i]);
        }
    }
    private void Activate_Additional_Crow_Attack(FarmTile farmTile)
    {
        var farmTiles = e.controller.farmTiles;
        
        List<int> surroundingNums = new List<int>();
        
        // find top tile
        surroundingNums.Add(farmTile.data.tileNum - 5);
        // find bottom tile
        surroundingNums.Add(farmTile.data.tileNum + 5);
        // find left tile
        surroundingNums.Add(farmTile.data.tileNum - 1);
        // find right tile
        surroundingNums.Add(farmTile.data.tileNum + 1);

        // for the top, bottom, left, right tiles
        for (int i = 0; i < surroundingNums.Count; i++)
        {
            // if there is a tile
            if (surroundingNums[i] < 0 || surroundingNums[i] > 24) continue;
            
            var surroundingFarmTile = farmTiles[surroundingNums[i]];

            // if there is a tile on left or right side
            if (farmTile.data.tileNum - surroundingNums[i] == 1 || surroundingNums[i] - farmTile.data.tileNum == 1)
            {
                // if the tile is on the same row
                if (farmTile.data.tileRow != surroundingFarmTile.data.tileRow) continue;
            }

            // if the tile has a seed planted
            if (!FarmTile_Condition_Check(surroundingFarmTile)) continue;

            // if additional crow attack of sub data percentage activates
            if (!e.Percentage_Setter(subData.percentage)) continue;

            // assign additional crow attacked icon
            surroundingFarmTile.Add_Status(5);

            // if the tile does not have a scarecrow buff
            if (FarmTile_Has_Scarecrow(surroundingFarmTile)) continue;

            // attack value
            surroundingFarmTile.tileSeedStatus.health += subData.health;
            surroundingFarmTile.tileSeedStatus.watered += subData.watered;
            surroundingFarmTile.tileSeedStatus.dayPassed += subData.dayPassed;
            surroundingFarmTile.tileSeedStatus.bonusPoints += subData.bonusPoints;
        }
    }
}
