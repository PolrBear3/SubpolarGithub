using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowy_Stun_Shield : MonoBehaviour, IBuff, IBuffResetable
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
        Activate_Snowy_Stun_Shield();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool Is_Weather_Snowy()
    {
        // if the weather is snowy
        if (b.controller.eventSystem.currentWeather.weatherID == 4) return true;
        else return false;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the tile is snowy stunned
        if (!farmTile.Find_Status(3)) return false;

        // if the tile has a snowy stun shield buff
        if (!farmTile.Find_Buff(4)) return false;

        return true;
    }

    private void Activate_Snowy_Stun_Shield()
    {
        // buff activation
        if (data.activated) return;
        data.activated = true;

        // if the weather is snowy
        if (!Is_Weather_Snowy()) return;

        var farmTiles = b.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // remove snowy stunned status
            farmTiles[i].Remove_Status(3, false);

            // use snowy stun shield buff
            farmTiles[i].Remove_Buff(4, false);
        }
    }
}
