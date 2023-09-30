using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    private Rigidbody2D _rb;
    private BoxCollider2D _bc;
    private Animator _anim;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    private Vector2 _moveDirection;
    private Vector2 _faceDirection;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb)) _rb = rb;
        if (gameObject.TryGetComponent(out BoxCollider2D bc)) _bc = bc;
        if (gameObject.TryGetComponent(out Animator anim)) _anim = anim;
    }
    private void Update()
    {
        Animation();
    }
    private void FixedUpdate()
    {
        Movement();
    }

    // Input
    private void OnMovement(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        _moveDirection = direction;

        if (direction == Vector2.zero) return;
        _faceDirection = direction;
    }
    private void OnInteract()
    {

    }

    // Custom
    private void Movement()
    {
        _rb.velocity = new Vector2(_moveDirection.x * _moveSpeed, _moveDirection.y * _moveSpeed);
    }
    private void Animation() // move this OnMovement
    {
        _anim.SetFloat("horizontal", _faceDirection.x);
        _anim.SetFloat("vertical", _faceDirection.y);

        if (_rb.velocity == Vector2.zero)
        {
            _anim.Play("Idle");
        }
        else
        {
            _anim.Play("Move");
        }
    }
}
