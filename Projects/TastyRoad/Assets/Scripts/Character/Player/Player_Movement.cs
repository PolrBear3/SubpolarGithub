using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Player_Controller _playerController;


    private Vector2 _currentDirection;
    public Vector2 currentDirection => _currentDirection;

    [Header("")]
    [SerializeField][Range(0, 10)] private float _defaultMoveSpeed;

    private float _additionalMoveSpeed;
    public float additionalMoveSpeed => _additionalMoveSpeed;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller playerController)) { _playerController = playerController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }
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


    // FixedUpdate Movement Update
    public bool Is_Moving()
    {
        if (_rigidBody.velocity != Vector2.zero) return true;
        else return false;
    }

    private void Movement_Update()
    {
        float moveSpeed = _defaultMoveSpeed + _additionalMoveSpeed;

        _rigidBody.velocity = new Vector2(_currentDirection.x * moveSpeed, _currentDirection.y * moveSpeed);
    }


    // Data Control
    public void Update_MoveSpeed(float updateValue)
    {
        _additionalMoveSpeed += updateValue;
    }
}
