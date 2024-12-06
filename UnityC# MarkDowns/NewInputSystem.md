# New Input System
```C#
using UnityEngine.InputSystem;
```

### ActionType Value, ControlType Vector2
Action is named "Movement". OnMovement is the usage name.

```C#
Vector2 direction;

private void OnMovement(InputValue value)
{
    direction = value.Get<Vector2>();
}
```

### Press Hold Input
Action is named "Select".

```C#
public float _setHoldTime_;

private float _pressStartTime;
private Coroutine _pressDelayCoroutine;


private void Start()
{
    _controller.Player_Input().actions["Interact"].started += ctx => OnPressStart();
    _controller.Player_Input().actions["Interact"].canceled += ctx => OnPressEnd();
}


private void OnPressStart()
{
    _pressDelayCoroutine = StartCoroutine(OnPressStart_Coroutine());
}
private IEnumerator OnPressStart_Coroutine()
{
    _pressStartTime = Time.time;
    _controller.timer.Set_Time((int)_holdTime);
    _controller.timer.Run_Time();
}

private void OnPressEnd()
{
    StopCoroutine(_pressDelayCoroutine);
    _pressDelayCoroutine = null;

    float pressDuration = Time.time - _pressStartTime;

    _controller.timer.Stop_Time();
    _controller.timer.Toggle_Transparency(true);

    if (pressDuration >= _holdTime)
    {
       // press hold input activated
    }

    // press input activated
}
```