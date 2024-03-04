using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLauncher_Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;

    private ItemLauncher _itemLauncher;

    [Header("Default Data")]
    [SerializeField] private float _speed;

    private Vector3 _direction;

    public delegate void MovementEvent();
    public event MovementEvent movementEvent;



    // UnityEngine
    protected virtual void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        movementEvent?.Invoke();
    }



    // Settings
    public void Set_ItemLauncher(ItemLauncher launcher)
    {
        _itemLauncher = launcher;
    }

    public void Set_Direction(Vector2 direction)
    {
        _direction = direction;
    }



    // Movement Types
    public void Basic_Movement()
    {
        Vector3 direction = _direction - _itemLauncher.transform.position;

        _rb.velocity = new Vector2(direction.x, direction.y).normalized * _speed;

        Destroy(gameObject, _itemLauncher.range);
    }

    public void Parabola_Movement()
    {
        // calculation
        float horizontalVelocity = _speed * Mathf.Cos(_itemLauncher.angle * Mathf.Deg2Rad);
        float verticalVelocity = _speed * Mathf.Sin(_itemLauncher.angle * Mathf.Deg2Rad);

        // direction update
        if (_direction.x < 0) horizontalVelocity *= -1f;

        // Set initial velocity of the Rigidbody2D
        _rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);

        StartCoroutine(Parabola_Movement_Coroutine());

        Destroy(gameObject, _itemLauncher.range);
    }
    private IEnumerator Parabola_Movement_Coroutine()
    {
        while (true)
        {
            _rb.velocity += Vector2.up * - _itemLauncher.gravity * Time.deltaTime;

            yield return null;
        }
    }
}