using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudyStun_Shield : MonoBehaviour, IBuff, IBuffResetable
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
        Activate_CloudyStun_Shield();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool Is_Weather_Cloudy()
    {
        if (b.controller.eventSystem.currentWeather.weatherID == 1) return true;
        else return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the farmtile is cloudy stunned
        if (!farmTile.Find_Status(2)) return false;

        // if the farmtile has cloudy stun shield buff
        if (!farmTile.Find_Buff(0)) return false;

        return true;
    }

    private void Activate_CloudyStun_Shield()
    {
        // if event is not activated
        if (data.activated) return;
        // activate event
        data.activated = true;

        // if the weather is cloudy
        if (!Is_Weather_Cloudy()) return;

        var farmTiles = b.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // remove cloudy stun status
            farmTiles[i].Remove_Status(2, false);
            
            // use cloudy stun shield buff from current buffs
            farmTiles[i].Remove_Buff(0, false);
        }
    }
}
