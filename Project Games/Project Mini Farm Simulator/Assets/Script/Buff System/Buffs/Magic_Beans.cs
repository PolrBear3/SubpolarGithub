using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Beans : MonoBehaviour, IBuff, IBuffResetable
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
        Activate_Magic_Beans();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // check if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        // check if the farmtile has Jack's magic beans
        if (!farmTile.Find_Buff(7)) return false;

        return true;
    }

    private void Activate_Magic_Beans()
    {
        // activation 
        if (data.activated) return;
        data.activated = true;

        var farmTiles = b.controller.farmTiles;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // save repeat amount
            int repeatAmount = farmTiles[i].Amount_Buff(7);

            // current magic beans disapears
            farmTiles[i].Remove_Buff(7, true);

            // repeat magic bean buff percentage according to buff amount
            Magic_Bean_Repeat(farmTiles[i], repeatAmount);
        }
    }
    private void Magic_Bean_Repeat(FarmTile farmTile, int repeatAmount)
    {
        bool percentageActivated = false;
        
        for (int i = 0; i < repeatAmount; i++)
        {
            if (percentageActivated || !b.controller.eventSystem.Percentage_Setter(data.percentage))
            {
                // add dead magic bean status
                farmTile.Add_Status(18);
                continue;
            }

            percentageActivated = true;

            // add magic beanstalk status
            farmTile.Add_Status(17);

            // activation
            farmTile.tileSeedStatus.dayPassed = farmTile.tileSeedStatus.fullGrownDay;
        }
    }
}
