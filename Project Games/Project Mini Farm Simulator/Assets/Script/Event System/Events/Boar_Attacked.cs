using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar_Attacked : MonoBehaviour, IEvent, IEventResetable
{
    private Event_System e;

    public Event_Data data;
    public Event_Data subData;
    
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
        Activate_Boar_Attack();
    }
    public void Reset_Event()
    {
        data.activated = false;
    }

    private bool Season_Check()
    {
        // if the current season is spring, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 0) return false;

        // if the current season is summer, return false
        if (e.controller.timeSystem.currentSeason.seasonID == 1) return false;

        return true;
    }

    private bool FarmTile_Has_Buff(FarmTile farmTile)
    {
        return true;
    }
    private bool Current_FarmTile_Condition_Check(FarmTile farmTile)
    {
        // if the tile has a seed planted
        if (!farmTile.data.seedPlanted) return false;

        return true;
    }

    private void Activate_Boar_Attack()
    {

    }
}
