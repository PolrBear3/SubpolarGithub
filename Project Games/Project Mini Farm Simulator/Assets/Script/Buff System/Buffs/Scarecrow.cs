using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour, IBuff
{
    private Buff_System b;

    private void Awake()
    {
        b = gameObject.transform.parent.GetComponent<Buff_System>();
    }
    public void Activate_Buff()
    {
        Activate_Scarecrow();
    }

    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;
        
        // if the farmtile has a scarecrow buff
        if (!farmTile.Find_Buff(3)) return false;

        // if the farmtile has a crow attacked status or additional crow attacked status
        if (!farmTile.statusIconIndicator.Find_Status(4) || !farmTile.statusIconIndicator.Find_Status(5)) return false;

        return true;
    }

    private void Activate_Scarecrow()
    {
        var farmTiles = b.controller.farmTiles;
        
        for (int i = 0; i < farmTiles.Length; i++)
        {
            // if the farmtile is a condition for scarecrow buff use
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // if it was crow attacked
            if (farmTiles[i].statusIconIndicator.Find_Status(4))
            {
                // use scarecrow buff from current buffs
                farmTiles[i].Remove_Buff(b.controller.ID_Buff_Search(3));
            }
            // remove crow attacked status
            farmTiles[i].statusIconIndicator.UnAssign_Status(4);
            // remove all additional crow attacked statuses
            farmTiles[i].statusIconIndicator.UnAssign_Status_NonBreak(5);
        }
    }
}
