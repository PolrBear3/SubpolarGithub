using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : MonoBehaviour, IBuff, IBuffResetable
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
        Activate_Umbrella();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the farmtile has umbrella
        if (!farmTile.Find_Buff(6)) return false;

        // if the farmtile is drown damaged
        if (!farmTile.Find_Status(12)) return false;

        return true;
    }

    private void Activate_Umbrella()
    {
        // buff activation
        if (data.activated) return;
        data.activated = true;

        var farmTiles = b.controller.farmTiles;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // remove status
            farmTiles[i].Remove_Status(12, false);
            // use buff
            farmTiles[i].Remove_Buff(6, false);
        }
    }
}
