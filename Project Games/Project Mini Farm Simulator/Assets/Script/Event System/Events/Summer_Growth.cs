using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summer_Growth : MonoBehaviour, IEvent, IEventResetable
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

    }
    public void Reset_Event()
    {
        data.activated = false;
    }
}
