using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Sunny : MonoBehaviour, IBuff, IBuffResetable
{
    private Buff_System b;

    public Event_Data data;

    private void Awake()
    {
        b = gameObject.transform.parent.GetComponent<Buff_System>();
    }
    private void Start()
    {
        data.activated = true;
    }
    public void Activate_Buff()
    {
        Activate_Golden_Sunny();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool Is_Weather_Sunny()
    {
        if (b.controller.eventSystem.data.currentWeather.weatherID == 0) return true;
        else return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // check if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // check if the farmtile is sunny buffed
        if (!farmTile.Find_Status(1)) return false;

        // check if the farmtile has golden sunny buff
        if (!farmTile.Find_Buff(2)) return false;

        return true;
    }

    private void Activate_Golden_Sunny()
    {
        // activation 
        if (data.activated) return;
        data.activated = true;

        if (!Is_Weather_Sunny()) return;

        var farmTiles = b.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // remove sunny buffed status
            farmTiles[i].Remove_Status(1, false);

            // assign golden sunny buffed status
            farmTiles[i].Add_Status(6);

            // remove golden sunny buff from current buffs
            farmTiles[i].Remove_Buff(2, false);

            // golden sunny buffed amount
            farmTiles[i].tileSeedStatus.health += data.health;
            farmTiles[i].tileSeedStatus.watered += data.watered;
            farmTiles[i].tileSeedStatus.dayPassed += data.dayPassed;
            farmTiles[i].tileSeedStatus.bonusPoints += data.bonusPoints;
        }
    }
}
