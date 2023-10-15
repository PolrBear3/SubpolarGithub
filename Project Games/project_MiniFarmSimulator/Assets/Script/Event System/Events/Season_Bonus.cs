using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Season_Bonus: MonoBehaviour, IEvent, IEventResetable
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
        Activate_Season_Bonus();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Season_Seed_Check(Seed_ScrObj seed)
    {
        for (int i = 0; i < seed.bonusSeasons.Length; i++)
        {
            if (seed.bonusSeasons[i] == e.controller.timeSystem.currentSeason) return true;
        }

        return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the farm tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the seed is is planted on bonus season
        if (!Season_Seed_Check(farmTile.data.plantedSeed)) return false;

        // if the farm tile is not ready to be harvested
        if (farmTile.tileSeedStatus.harvestReady) return false;

        return true;
    }
    private void Add_Season_Growth_Status(FarmTile farmTile)
    {
        var seedSeasons = farmTile.data.plantedSeed.bonusSeasons;

        for (int i = 0; i < seedSeasons.Length; i++)
        {
            if (seedSeasons[i] == e.controller.timeSystem.currentSeason)
            {
                var seasonToAdd = e.controller.timeSystem.currentSeason;

                // spring
                if (seasonToAdd.seasonID == 0)
                {
                    farmTile.Add_Status(14);
                }
                // summer
                else if (seasonToAdd.seasonID == 1)
                {
                    farmTile.Add_Status(15);
                }
                // fall
                else if (seasonToAdd.seasonID == 2)
                {
                    farmTile.Add_Status(16);
                }

                break;
            }
        }
    }

    private void Activate_Season_Bonus()
    {
        // activation check
        if (data.activated) return;
        data.activated = true;

        var farmTiles = e.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // farm condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // percentage activation check
            e.Percentage_Setter(data.percentage);

            // add certain season growth icon
            Add_Season_Growth_Status(farmTiles[i]);

            // data activation
            farmTiles[i].tileSeedStatus.bonusPoints += data.bonusPoints;
        }
    }
}
