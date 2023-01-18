using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rapid_Growth : MonoBehaviour, IEvent, IEventResetable
{
    private Event_System e;

    public Event_Data data;
    [SerializeField] Seed_ScrObj[] rapidGrowthSeeds;

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
        Activate_Rapid_Growth();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }
    private bool Planted_Seed_Check(FarmTile farmTile)
    {
        for (int i = 0; i < rapidGrowthSeeds.Length; i++)
        {
            if (farmTile.data.plantedSeed == rapidGrowthSeeds[i])
            {
                return true;
            }
        }
        return false;
    }
    private bool Cross_Surrounding_FarmTile_Check(FarmTile farmTile)
    {
        var farmTiles = e.controller.farmTiles;
        int farmTileNum = farmTile.data.tileNum;
        List<int> surroundingNums = new List<int>();

        // left
        surroundingNums.Add(farmTileNum - 1);
        // right
        surroundingNums.Add(farmTileNum + 1);
        // top
        surroundingNums.Add(farmTileNum - 5);
        // bottom
        surroundingNums.Add(farmTileNum + 5);

        for (int i = 0; i < surroundingNums.Count; i++)
        {
            // min and max tile location
            if (surroundingNums[i] < 0 || surroundingNums[i] > 24) continue;

            var surroundingTile = farmTiles[surroundingNums[i]];

            // left right check
            if (i == 0 || i == 1)
            {
                if (surroundingTile.data.tileRow != farmTile.data.tileRow) continue;
            }

            // top bottom check
            if (i == 2 || i == 3)
            {
                if (farmTile.data.tileRow - surroundingTile.data.tileRow >= 2) continue;
                if (surroundingTile.data.tileRow - farmTile.data.tileRow >= 2) continue;
            }

            // check if surrounding tiles have growth ability seed planted
            if (!Planted_Seed_Check(surroundingTile)) continue;

            return true;
        }

        return false;
    }

    private void Activate_Rapid_Growth()
    {
        // activation check
        if (data.activated) return;
        data.activated = true;

        var farmTiles = e.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // basic tile condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // if the tile has a seed planted that has a rapid growth ability
            if (!Planted_Seed_Check(farmTiles[i])) continue;

            // if there is a rapid growth ability seed planted tile cross surrounding this tile
            if (!Cross_Surrounding_FarmTile_Check(farmTiles[i])) continue;

            // if percentage activates
            if (!e.Percentage_Setter(data.percentage)) continue;


            // add status rapid growth
            farmTiles[i].Add_Status(11);

            // data activation
            farmTiles[i].tileSeedStatus.dayPassed += data.dayPassed;
            farmTiles[i].tileSeedStatus.health += data.health;
            farmTiles[i].deathData.damageCount += data.health;
        }
    }
}
