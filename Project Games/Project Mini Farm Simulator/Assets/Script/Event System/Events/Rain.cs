using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour, IEvent, IEventResetable
{
    private Event_System es;

    public Event_Data data;

    private void Awake()
    {
        es = gameObject.transform.parent.GetComponent<Event_System>();
    }
    private void Start()
    {
        data.activated = true;
    }
    public void Activate_Event()
    {
        Activate_Rain();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Is_Raining()
    {
        if (es.currentWeather.weatherID == 2) return true;
        else return false;
    }
    private bool FarmTile_Event_Check(FarmTile farmTile)
    {
        // if the tile is unlocked
        if (farmTile.data.tileLocked)
        {
            return false;
        }
        // if the tile is currently watered
        if (farmTile.tileSeedStatus.currentDayWatered)
        {
            return false;
        }

        return true;
    }

    private void Activate_Rain()
    {
        if (!Is_Raining()) return;

        // if event is not activated
        if (data.activated) return;
        // activate event
        data.activated = true;

        var farmTiles = es.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (FarmTile_Event_Check(farmTiles[i]))
            {
                farmTiles[i].tileSeedStatus.currentDayWatered = true;

                if (!farmTiles[i].tileSeedStatus.harvestReady)
                {
                    farmTiles[i].tileSeedStatus.watered += 1;
                }

                farmTiles[i].tileSeedStatus.daysWithoutWater = 0;
                farmTiles[i].statusIconIndicator.Assign_Status(0);
            }
        }
    }
}
