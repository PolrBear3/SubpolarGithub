using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunny_Buffed : MonoBehaviour, IEvent, IEventResetable
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
        Activate_Sunny_Buffed();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Is_Weather_Sunny()
    {
        if (e.data.currentWeather.weatherID == 0) return true;
        else return false;
    }
    private bool Golden_Sunny_Buff_Detected(FarmTile farmTile)
    {
        if (farmTile.Find_Buff(2)) return true;
        else return false;
    }
    private bool Sunny_Buffed_Available(FarmTile farmTile)
    {
        // if the farm tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the farm tile is not ready to be harvested
        if (farmTile.tileSeedStatus.harvestReady) return false;

        return true;
    }

    private void Activate_Sunny_Buffed()
    {
        // activation 
        if (data.activated) return;
        data.activated = true;

        // if the weather is sunny
        if (!Is_Weather_Sunny()) return;

        // if sunny buff percentage event activates
        if (!e.Percentage_Setter(data.percentage)) return;

        var farmTile = e.controller.farmTiles;

        for (int i = 0; i < farmTile.Length; i++)
        {
            if (!Sunny_Buffed_Available(farmTile[i])) continue;

            // assign status icon
            farmTile[i].Add_Status(1);

            // if the farmtile has a golden sunny buff
            if (Golden_Sunny_Buff_Detected(farmTile[i])) continue;

            // activate event
            farmTile[i].tileSeedStatus.health += data.health;
            farmTile[i].tileSeedStatus.dayPassed += data.dayPassed;
            farmTile[i].tileSeedStatus.bonusPoints += data.bonusPoints;
        }
    }
}
