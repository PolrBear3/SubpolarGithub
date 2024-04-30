# Delegates and Events

### Reference
https://gamedevbeginner.com/events-and-delegates-in-unity/

### Variable Setting
```C#
// basic type
public delegate void On_Event();
public event On_Event onEvent;

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
    onEvent += Function;
}

private void OnDisable()
{
    // unsubscribe
    onEvent -= Function;
}

// always unsubscribe on destroy for reload scene
private void OnDestroy()
{
    // unsubscribe
    onEvent -= Function;
}

//
private void Function()
{
    Debug.Log("function activated");
}
```

### Triggering the Event
This will run all the functions that are subscribed to onEvent.
```C#
onEvent?.Invoke();
```