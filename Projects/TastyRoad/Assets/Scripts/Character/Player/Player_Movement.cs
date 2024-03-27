using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private Player_Controller _playerController;

    private Rigidbody2D _rigidBody;

    private Vector2 _currentDirection;
    public Vector2 currentDirection => _currentDirection;

    [SerializeField] private float _moveSpeed;

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
        Rigidbody_Move();
    }

    // Player Input
    private void OnMovement(InputValue value)
    {
        if (Main_Controller.gamePaused) return;

        Vector2 input = value.Get<Vector2>();
        _currentDirection = input;

        _playerController.animationController.Flip_Sprite(_currentDirection.x);
    }

    // Check
    public bool Is_Moving()
    {
        if (_rigidBody.velocity != Vector2.zero) return true;
        else return false;
    }

    // Fixed Update Move
    private void Rigidbody_Move()
    {
        _rigidBody.velocity = new Vector2(_currentDirection.x * _moveSpeed, _currentDirection.y * _moveSpeed);
    }
}
