using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Player_Controller _playerController;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _defaultMoveSpeed;
    public float defaultMoveSpeed => _defaultMoveSpeed;

    private float _moveSpeed;
    public float moveSpeed => _moveSpeed;

    private Vector2 _currentDirection;
    public Vector2 currentDirection => _currentDirection;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller playerController)) { _playerController = playerController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }

        _moveSpeed += _defaultMoveSpeed;
    }

    private void Update()
    {
        _playerController.animationController.Idle_Move(Is_Moving());
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }


    // Player Input
    private void OnMovement(InputValue value)
    {
        if (Main_Controller.gamePaused)
        {
            _currentDirection = Vector2.zero;
            return;
        }

        Vector2 input = value.Get<Vector2>();
        _currentDirection = input;

        _playerController.animationController.Flip_Sprite(_currentDirection.x);
    }


    // Movement
    public bool Is_Moving()
    {
        if (_rigidBody.velocity != Vector2.zero) return true;
        else return false;
    }

    private void Movement_Update()
    {
        _rigidBody.velocity = new Vector2(_currentDirection.x * _moveSpeed, _currentDirection.y * _moveSpeed);
    }


    public void Set_MoveSpeed(float setValue)
    {
        _moveSpeed = Mathf.Clamp(setValue, _defaultMoveSpeed, 100f);
    }
}
