using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private SpriteRenderer _sr;
    [HideInInspector] public Rigidbody2D rb;

    private Player_Controller _playerController;

    private Vector2 _moveDirection;
    private bool _facingRight;

    public float moveSpeed;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) _sr = sr;
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
        rb.velocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);
    }

    private void Flip()
    {
        Vector2 currentDirection = gameObject.transform.localScale;
        currentDirection.x *= -1;
        gameObject.transform.localScale = currentDirection;

        _facingRight = !_facingRight;
    }
    private void Flip_Sprite()
    {
        _facingRight = !_facingRight;

        _sr.flipX = _facingRight;
    }
    private void Flip_Update()
    {
        if (_moveDirection.x > 0 && _facingRight) Flip_Sprite();
        if (_moveDirection.x < 0 && !_facingRight) Flip_Sprite();
    }
}