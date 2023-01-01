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
        
    }
    public void Reset_Buff()
    {
        data.activated = false;
    }
}
