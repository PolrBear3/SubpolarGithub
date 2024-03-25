using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLauncher_Coin : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Animator _anim;
    private Rigidbody2D _rb;

    private CoinLauncher _itemLauncher;

    [Header("Default Data")]
    [SerializeField] private float _speed;

    private Vector3 _direction;

    public delegate void MovementEvent();
    public event MovementEvent movementEvent;



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _anim = gameObject.GetComponent<Animator>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        movementEvent?.Invoke();
    }



    // Settings
    public void Set_CoinData(CoinLauncher launcher, Coin_ScrObj coinType, Vector2 direction)
    {
        _itemLauncher = launcher;

        _sr.sprite = coinType.sprite;
        _anim.runtimeAnimatorController = coinType.spinAnim;

        _direction = direction;
    }

    public void Set_CustomData(CoinLauncher launcher, Sprite sprite, Vector2 direction)
    {
        _itemLauncher = launcher;

        _sr.sprite = sprite;
        _anim.enabled = false;

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