using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    private Player_Controller _playerController;

    private Vector2 _moveDirection;
    private bool _facingRight;

    [SerializeField] private float _moveSpeed;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb)) this.rb = rb;
        if (gameObject.TryGetComponent(out Player_Controller playerController)) _playerController = playerController;
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void OnMovement(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        _moveDirection = input;
        Flip_Update();
    }

    //
    private void Move()
    {
        rb.velocity = new Vector2(_moveDirection.x * _moveSpeed, _moveDirection.y * _moveSpeed);
    }

    private void Flip()
    {
        Vector2 currentDirection = gameObject.transform.localScale;
        currentDirection.x *= -1;
        gameObject.transform.localScale = currentDirection;

        _facingRight = !_facingRight;
    }
    private void Flip_Update()
    {
        if (_moveDirection.x > 0 && _facingRight) Flip();
        if (_moveDirection.x < 0 && !_facingRight) Flip();
    }
}