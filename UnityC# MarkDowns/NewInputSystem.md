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