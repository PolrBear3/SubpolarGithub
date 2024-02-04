# Position Values
Left end of camera point
```C#
Vector2 leftEnd = mainCamera.ViewportToWorldPoint(new Vector2(0, 0.5f));
``` 

Right end of camera point
```C#
Vector2 rightEnd = mainCamera.ViewportToWorldPoint(new Vector2(1, 0.5f));
``` 

Top end of camera point
```C#
Vector2 rightEnd = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 1));
``` 

Bottom end of camera point
```C#
Vector2 rightEnd = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 0));
``` 