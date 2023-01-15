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
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

    private void Activate_Boar_Attack()
    {
        // event activation check
        if (data.activated) return;
        data.activated = true;

        // check if season is fall or winter
        if (!Season_Check()) return;

        var farmTiles = e.controller.farmTiles;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            // basic farmtile condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // percentage activation
            if (!e.Percentage_Setter(data.percentage)) continue;

            farmTiles[i].Add_Status(7);

            if (farmTiles[i].Find_Buff(5)) continue;

            // data activation
            farmTiles[i].tileSeedStatus.health += data.health;
            farmTiles[i].tileSeedStatus.dayPassed += data.dayPassed;
            farmTiles[i].tileSeedStatus.bonusPoints += data.watered;

            Activate_Additional_Boar_Attacks(farmTiles[i]);
        }

        Additional_Boar_Attacks_Data_Activation();
    }
    private void Activate_Additional_Boar_Attacks(FarmTile farmTile)
    {
        var farmTiles = e.controller.farmTiles;

        List<int> cornerTileNums = new List<int>();
        
        // left top 
        cornerTileNums.Add(farmTile.data.tileNum - 6);
        // right top
        cornerTileNums.Add(farmTile.data.tileNum - 4);
        // left bottom
        cornerTileNums.Add(farmTile.data.tileNum + 6);
        // right bottom
        cornerTileNums.Add(farmTile.data.tileNum + 4);

        for (int i = 0; i < cornerTileNums.Count; i++)
        {
            // if there is a tile
            if (cornerTileNums[i] < 0 || cornerTileNums[i] > 24) continue;

            // if there is a tile on bottom corners
            if (farmTiles[cornerTileNums[i]].data.tileRow - farmTile.data.tileRow >= 2) continue;

            // if there is a tile on bottom corners
            if (farmTile.data.tileRow - farmTiles[cornerTileNums[i]].data.tileRow >= 2) continue;


            // percentage activation
            if (!e.Percentage_Setter(subData.percentage)) continue;

            var cornerTiles = farmTiles[cornerTileNums[i]];

            cornerTiles.Add_Status(8);
        }
    }
    
    private void Additional_Boar_Attacks_Data_Activation()
    {
        var farmTiles = e.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // basic farmtile condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            if (!farmTiles[i].Find_Status(8)) continue;

            Data_Calculation(farmTiles[i]);
        }
    }
    private void Data_Calculation(FarmTile farmTile)
    {
        int repeatAmount = 0;

        if (farmTile.Amount_Status(8) > farmTile.Amount_Buff(5))
        {
            repeatAmount += farmTile.Amount_Status(8) - farmTile.Amount_Buff(5);
        }
        else return;

        for (int i = 0; i < repeatAmount; i++)
        {
            // data activation
            farmTile.tileSeedStatus.health += subData.health;
            farmTile.tileSeedStatus.dayPassed += subData.dayPassed;
            farmTile.tileSeedStatus.bonusPoints += subData.watered;
        }
    }
}
