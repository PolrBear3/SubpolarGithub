using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour, IEvent, IEventResetable
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
        Activate_Thief();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

    private void Activate_Thief()
    {
        // activation check
        if (data.activated) return;
        data.activated = true;

        var farmTiles = e.controller.farmTiles;
        int subtractCount = 0;

        for (int i = 0; i < farmTiles.Length; i++)
        {
            // percentage activates
            if (!e.Percentage_Setter(data.percentage)) continue;

            // if the farmTile is in condition for event
            if (!FarmTile_Condition_Check(farmTiles[i])) continue;

            // assign thief icon
            farmTiles[i].Add_Status(19);

            // doesn't have buff check


            // attack value
            farmTiles[i].deathData.damageCount -= farmTiles[i].tileSeedStatus.health;
            farmTiles[i].tileSeedStatus.health = 0;
            subtractCount += data.bonusPoints;
        }


        if (subtractCount <= 0) return;

        e.controller.Subtract_Money_NonBlink(subtractCount);
    }
}
