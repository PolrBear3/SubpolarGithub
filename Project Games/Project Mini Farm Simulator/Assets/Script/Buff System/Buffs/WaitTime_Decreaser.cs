using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTime_Decreaser : MonoBehaviour, IBuff, IBuffResetable
{
    private Buff_System b;

    public Event_Data data;
    public float subtractTime;

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
        Use_WaitTime_Decreaser();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool Farmtile_Condition_Check(FarmTile farmTile)
    {
        // if the farmtile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the farmTile has a time decreaser buff
        if (!farmTile.Find_Buff(1)) return false;

        return true;
    }

    private void Use_WaitTime_Decreaser()
    {
        // if buff is not activated ??
        if (data.activated) return;
        // activate buff
        data.activated = true;

        var farmTiles = b.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // condition check
            if (!Farmtile_Condition_Check(farmTiles[i])) continue;

            // activate time decreaser buff
            b.controller.timeSystem.Subtract_MyTime(subtractTime);

            // remove time decreaser buff
            farmTiles[i].Remove_Buff(1, false);

            // check for more time decreaser buffs
            i -= 1;
        }
    }
}
