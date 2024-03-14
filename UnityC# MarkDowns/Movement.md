# All Movement Types

## Basic Values
```C#
private Vector2 _direction;
private float _speed
```

## Transform
- ignores physics
- good for performance
- ex) ingame UI element movements

```C#
private void Update()
{
    transform.Translate(_direction * _speed * Time.deltaTime);
}
```

## Rigidbody 2D
- uses physics
- high performance required

```C#
private Rigidbody2D _rb;

private void FixedUpdate()
{
    // basic velocity movement
    _rb.veloctiy = new Vector2(_direction.x * _speed * Time.deltaTime, _direction.y * _speed * Time.deltaTime);

    // gradual AddForce movement
    _rb.AddForce(_direction * _speed * Time.deltaTime);
}
```