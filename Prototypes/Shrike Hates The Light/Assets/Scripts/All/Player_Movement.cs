using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController { get => _gameController; set => _gameController = value; }

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody;
    private Animator _animator;

    private Tile _detectedTile;

    [SerializeField] private SpriteRenderer _thinkBox;

    [SerializeField] private Transform _holdPoint;
    public Transform holdPoint { get => _holdPoint; set => _holdPoint = value; }

    [SerializeField] private float _moveSpeed;

    private Vector2 _moveDirection;

    private Basic_Gear _currentGear;
    public Basic_Gear currentGear { get => _currentGear; set => _currentGear = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) _spriteRenderer = spriteRenderer;
        if (gameObject.TryGetComponent(out Rigidbody2D rigidBody)) _rigidBody = rigidBody;
        if (gameObject.TryGetComponent(out Animator animator)) _animator = animator;
    }
    private void FixedUpdate()
    {
        Basic_Move();
    }
    public void OnMovement(InputValue value)
    {
        _moveDirection = value.Get<Vector2>();
    }
    public void OnInteract()
    {
        Hold_Drop_Gear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Tile detectTile)) return;
        _detectedTile = detectTile;

        if (_detectedTile.currentGear != null && _detectedTile.currentGear.goldGear != null && _gameController.currentLevelNum == 0)
        {
            _thinkBox.color = Color.white;
        }

        if (_detectedTile.isDefaultTile || _currentGear == null || _detectedTile.currentGear != null) return;

        _detectedTile.indicator.Gear_Indication(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Tile detectTile)) return;

        _detectedTile.indicator.Gear_Indication(false);
        _detectedTile = null;

        _thinkBox.color = Color.clear;
    }

    // Action
    private void Basic_Move()
    {
        _rigidBody.velocity = _moveDirection * _moveSpeed;

        if (_moveDirection.x > 0) Face_Right(true);
        if (_moveDirection.x < 0) Face_Right(false);

        if (_rigidBody.velocity == Vector2.zero)
        {
            _animator.SetBool("isMoving", false);
        }
        else
        {
            _animator.SetBool("isMoving", true);
        }
    }
    private void Hold_Drop_Gear()
    {
        if (_detectedTile == null) return;
        if (_detectedTile.isDefaultTile) return;
        if (_detectedTile.currentGear != null && _detectedTile.currentGear.unHoldable) return;

        // hold
        if (_currentGear == null && _detectedTile.currentGear != null)
        {
            _detectedTile.indicator.Gear_Indication(true);

            _currentGear = _detectedTile.currentGear;
            _detectedTile.currentGear.Move_toPlayer();
        }
        // drop
        else if (_currentGear != null && _detectedTile.currentGear == null)
        {
            _detectedTile.indicator.Gear_Indication(false);

            _detectedTile.currentGear = currentGear;
            _detectedTile.currentGear.Move_toTile(_detectedTile);
            _currentGear = null;
        }

        _gameController.currentLevel.AllGears_Spin_Activation_Check();
    }
    public void Die()
    {
        LeanTween.cancelAll();

        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        _animator.SetTrigger("death");

        _gameController.currentLevel.timeController.timeRunning = false;
        _gameController.Acivate_DeathCuratin();
        _gameController.currentLevel.Reload_Level(2.2f);

        _gameController.soundController.Play_Sound(4);
    }

    // Basic Functions
    private void Face_Right(bool activate)
    {
        _spriteRenderer.flipX = activate;
    }
}
