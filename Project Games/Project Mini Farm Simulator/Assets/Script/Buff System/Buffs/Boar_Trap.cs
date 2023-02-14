using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar_Trap : MonoBehaviour, IBuff, IBuffResetable
{
    private Buff_System b;

    public Event_Data data;
    public Event_Data subData;

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
        Use_Boar_Trap();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool Season_Check()
    {
        // if the current season is spring, return false
        if (b.controller.timeSystem.currentSeason.seasonID == 0) return false;

        // if the current season is summer, return false
        if (b.controller.timeSystem.currentSeason.seasonID == 1) return false;

        return true;
    }
    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

    private bool FarmTile_Has_BoarTrap(FarmTile farmTile)
    {
        if (farmTile.Find_Buff(5)) return true;
        else return false;
    }
    private bool FarmTile_Found_Boar(FarmTile farmTile)
    {
        if (farmTile.Find_Status(7)) return true;
        else return false;
    }
    private bool FarmTile_Found_AdditionalBoar(FarmTile farmTile)
    {
        if (farmTile.Find_Status(8)) return true;
        else return false;
    }

    private void Use_Boar_Trap()
    {
        // event activation check
        if (data.activated) return;
        data.activated = true;

        // check if season is fall or winter
        if (!Season_Check()) return;

        // reset boar sell calculation
        data.bonusPoints = 0;

        var farmTiles = b.controller.farmTiles;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            if (!FarmTile_Has_BoarTrap(farmTiles[i])) continue;

            if (FarmTile_Found_Boar(farmTiles[i]))
            {
                farmTiles[i].Remove_Status(7, false);
                farmTiles[i].Remove_Buff(5, false);

                // boar caught and ready to be sold
                farmTiles[i].Add_Status(9);
                data.bonusPoints += subData.bonusPoints;
            }

            // traps for additional boars
            Use_Trap_for_AdditionalBoars(farmTiles[i]);
        }

        // check if there are any boars caught to sell
        if (data.bonusPoints <= 0) return;

        // sell all boars caught
        b.controller.Add_Money_NonBlink(data.bonusPoints, 0);
    }
    private void Use_Trap_for_AdditionalBoars(FarmTile farmTile)
    {
        while (FarmTile_Has_BoarTrap(farmTile) && FarmTile_Found_AdditionalBoar(farmTile))
        {
            farmTile.Remove_Status(8, false);
            farmTile.Remove_Buff(5, false);

            // boar caught and ready to be sold
            farmTile.Add_Status(9);
            data.bonusPoints += subData.bonusPoints;
        }
    }
}
