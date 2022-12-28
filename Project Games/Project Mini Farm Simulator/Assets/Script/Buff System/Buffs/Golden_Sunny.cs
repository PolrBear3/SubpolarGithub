using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Sunny : MonoBehaviour, IBuff, IBuffResetable
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
        Activate_Golden_Sunny();
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }

    private bool Is_Weather_Sunny()
    {
        return true;
    }
    private bool FarmTile_Condition_Check()
    {
        // check if the farmtile is sunny buffed

        return true;
    }

    private void Activate_Golden_Sunny()
    {

    }
}
