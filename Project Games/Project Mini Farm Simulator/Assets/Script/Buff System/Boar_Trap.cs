using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar_Trap : MonoBehaviour, IBuff, IBuffResetable
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
        Activate_Boar_Trap();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // if the farmtile has a boar trap buff
        if (!farmTile.Find_Buff(5)) return false;

        // if the farmtile has a boar attacked status or additional boar attacked status
        if (!farmTile.Find_Status(7) || !farmTile.Find_Status(8)) return false;

        return true;
    }

    private void Activate_Boar_Trap()
    {
        // buff activation
        if (data.activated) return;
        data.activated = true;

        var farmTiles = b.controller.farmTiles;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // tile condition check
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            farmTiles[i].Remove_Status(7, false);
            farmTiles[i].Remove_Buff(5, false);

            Additional_Boar_Trap_Repeat(farmTiles[i]);
        }
    }

    private void Additional_Boar_Trap_Repeat(FarmTile farmTile)
    {
        int repeatAmount = 0;

        // if there are more additional boar attacks than the amount of traps 
        if (farmTile.Amount_Buff(5) <= farmTile.Amount_Status(8)) repeatAmount += farmTile.Amount_Buff(5);

        // if there are enough or more traps to protect additional boar attacks
        else if (farmTile.Amount_Buff(5) >= farmTile.Amount_Status(8)) repeatAmount += farmTile.Amount_Status(8);

        for (int i = 0; i < repeatAmount; i++)
        {
            farmTile.Remove_Status(8, false);
            farmTile.Remove_Buff(5, false);
        }
    }
}
