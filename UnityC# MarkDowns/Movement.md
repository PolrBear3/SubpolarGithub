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

    // move towards
    Vector2 setPosition;
    transform.position = Vector2.MoveTowards(transform.position, setPosition, _speed * Time.deltaTime);
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
    _rb.velocity = new Vector2(_direction.x * _speed, _direction.y * _speed);

    // gradual AddForce movement
    _rb.AddForce(_direction * _speed * Time.deltaTime);
}
```