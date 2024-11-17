# Delegates and Events

### Reference
https://gamedevbeginner.com/events-and-delegates-in-unity/

### Variable Setting
```C#
// basic type
public delegate void EventHandler();
public event EventHandler OnEvent;

// constructor type
public delegate void On_ConstuctorEvent(bool eventTriggered);
public event On_Event onConstuctorEvent;
```

### Subscription
```C#
// UnityEngine
private void OnEnable()
{
    // subscribe
    OnEvent += Function;
}

private void OnDisable()
{
    // unsubscribe
    OnEvent -= Function;
}

// always unsubscribe on destroy for reload scene
private void OnDestroy()
{
    // unsubscribe
    OnEvent -= Function;
}

//
private void Function()
{
    Debug.Log("function activated");
}
```

### Triggering the Event
This will run all the functions that are subscribed to OnEvent.
```C#
OnEvent?.Invoke();
```