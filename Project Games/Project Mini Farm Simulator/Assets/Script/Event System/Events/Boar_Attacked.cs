using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar_Attacked : MonoBehaviour, IEvent, IEventResetable
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
        Activate_Boar_Attack();
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

    private bool FarmTile_Has_Buff(FarmTile farmTile)
    {
        if (farmTile.Find_Buff(5)) return true;
        else return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

    private void Activate_Boar_Attack()
    {
        // event activation
        if (data.activated) return;
        data.activated = true;

        // if the current season is fall or winter
        if (!Season_Check()) return;

        var farmTiles = e.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // percentage activation check
            if (!e.Percentage_Setter(data.percentage)) continue;

            // assign boar attacked status
            farmTiles[i].Add_Status(7);

            // buff check
            if (FarmTile_Has_Buff(farmTiles[i])) continue;

            // data activation
            farmTiles[i].tileSeedStatus.dayPassed -= data.dayPassed;
            farmTiles[i].tileSeedStatus.health -= data.health;
            farmTiles[i].tileSeedStatus.watered -= data.watered;
            farmTiles[i].tileSeedStatus.bonusPoints -= data.bonusPoints;

            // additional baor attack activation
            Activate_Additional_Boar_Attack(farmTiles[i]);
        }
    }
    private void Activate_Additional_Boar_Attack(FarmTile farmTile)
    {
        var farmTiles = e.controller.farmTiles;

        List<int> cornerTileNums = new List<int>();

        // left top corner tile
        cornerTileNums.Add(farmTile.data.tileNum - 6);
        // right top corner tile
        cornerTileNums.Add(farmTile.data.tileNum + 4);
        // left bottom corner tile
        cornerTileNums.Add(farmTile.data.tileNum - 4);
        // right bottom corner tile
        cornerTileNums.Add(farmTile.data.tileNum + 6);

        for (int i = 0; i < cornerTileNums.Count; i++)
        {
            // check for out of range tiles
            if (cornerTileNums[i] > 24 || cornerTileNums[i] < 0) continue;

            var cornerTiles = farmTiles[cornerTileNums[i]];

            // check if there is a tile in the top corners
            if (farmTile.data.tileRow - cornerTiles.data.tileRow >= 2) continue;

            // check if there is a tile in the bottom corners
            if (cornerTiles.data.tileRow - farmTile.data.tileRow >= 2) continue;


            // corner tile condition check
            if (!FarmTile_Condition_Check(cornerTiles)) continue;

            // percentage activation check
            if (!e.Percentage_Setter(data.percentage)) continue;


            // assign additional attack status icon
            cornerTiles.Add_Status(8);
        }
    }

    private void Additional_Boar_Attack_Data_Calculation() //??
    {

    }
}
